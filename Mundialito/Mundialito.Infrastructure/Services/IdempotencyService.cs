
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Mundialito.Application.Features.Equipos.Commands;
using Mundialito.Application.Features.Jugadores.Commands;
using Mundialito.Application.Features.Goles;
using Mundialito.Application.Interfaces;
using Mundialito.Infrastructure.Persistence;

namespace Mundialito.Infrastructure.Services;

public sealed class IdempotencyService : IIdempotencyService
{
    private const string OperationEquipos = "POST:/api/equipos";
    private const string OperationJugadores = "POST:/api/jugadores";
    private const string OperationGoles = "POST:/api/goles";

    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CrearEquipoCommandHandler _equipoHandler;
    private readonly CrearJugadorCommandHandler _jugadorHandler;
    private readonly RegistrarGolesCommandHandler _golesHandler;

    public IdempotencyService(
        ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        CrearEquipoCommandHandler equipoHandler,
        CrearJugadorCommandHandler jugadorHandler,
        RegistrarGolesCommandHandler golesHandler)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _equipoHandler = equipoHandler;
        _jugadorHandler = jugadorHandler;
        _golesHandler = golesHandler;
    }

    public Task<IdempotencyExecutionResult> CrearEquipoAsync(
        string key,
        CrearEquipoCommand command,
        CancellationToken cancellationToken = default)
    {
        return EjecutarAsync(
            key,
            command,
            OperationEquipos,
            async () =>
            {
                var result = await _equipoHandler.HandleAsync(
                    command,
                    cancellationToken,
                    manageTransaction: false);

                return result.IsFailure
                    ? (false, Guid.Empty, result.ErrorMessage, result.ErrorCode)
                    : (true, result.Value, (string?)null, (string?)null);
            },
            cancellationToken);
    }

    public Task<IdempotencyExecutionResult> CrearJugadorAsync(
        string key,
        CrearJugadorCommand command,
        CancellationToken cancellationToken = default)
    {
        return EjecutarAsync(
            key,
            command,
            OperationJugadores,
            async () =>
            {
                var result = await _jugadorHandler.Handle(
                    command,
                    cancellationToken,
                    manageTransaction: false);

                return result.IsFailure
                    ? (false, Guid.Empty, result.ErrorMessage, result.ErrorCode)
                    : (true, result.Value, (string?)null, (string?)null);
            },
            cancellationToken);
    }

    public Task<IdempotencyExecutionResult> RegistrarGolesAsync(
        string key,
        RegistrarGolesCommand command,
        CancellationToken cancellationToken = default)
    {
        return EjecutarAsync(
            key,
            command,
            OperationGoles,
            async () =>
            {
                // No confirmar una transacción independiente desde el handler.
                // La operación completa se confirma en EjecutarAsync.
                var result = await _golesHandler.Handle(
                    command,
                    cancellationToken,
                    manageTransaction: false);

                return result.IsFailure
                    ? (false, Guid.Empty, result.ErrorMessage, result.ErrorCode)
                    : (true, Guid.Empty, (string?)null, (string?)null);
            },
            cancellationToken);
    }

    private async Task<IdempotencyExecutionResult> EjecutarAsync<TCommand>(
        string key,
        TCommand command,
        string operation,
        Func<Task<(bool Success, Guid Id, string? ErrorMessage, string? ErrorCode)>> execute,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length > 200)
        {
            return JsonResult(400, new
            {
                mensaje = "Idempotency-Key es obligatorio y no debe superar 200 caracteres.",
                codigo = "IDEMPOTENCY_KEY_INVALIDA"
            });
        }

        var requestHash = Convert.ToHexString(
            SHA256.HashData(
                JsonSerializer.SerializeToUtf8Bytes(command)));

        var existing = await BuscarAsync(
            operation,
            key,
            cancellationToken);

        if (existing is not null)
        {
            return ReproducirORechazar(existing, requestHash);
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Segunda comprobación para evitar duplicados concurrentes.
            existing = await BuscarAsync(
                operation,
                key,
                cancellationToken);

            if (existing is not null)
            {
                await _unitOfWork.RollbackTransactionAsync(
                    CancellationToken.None);

                return ReproducirORechazar(existing, requestHash);
            }

            var result = await execute();

            if (!result.Success)
            {
                await _unitOfWork.RollbackTransactionAsync(
                    CancellationToken.None);

                var status = result.ErrorCode switch
                {
                    "EQUIPO_DUPLICADO" => 409,
                    "JUGADOR_DUPLICADO" => 409,
                    "PARTIDO_NO_ENCONTRADO" => 404,
                    "JUGADOR_NO_ENCONTRADO" => 404,
                    "PARTIDO_NO_FINALIZADO" => 409,
                    "JUGADOR_NO_PARTICIPANTE" => 409,
                    _ => 400
                };

                return JsonResult(status, new
                {
                    mensaje = result.ErrorMessage,
                    codigo = result.ErrorCode
                });
            }

            var body = operation == OperationGoles
                ? JsonSerializer.Serialize(new
                {
                    mensaje = "Goles registrados correctamente"
                })
                : JsonSerializer.Serialize(new
                {
                    id = result.Id,
                    mensaje = operation == OperationEquipos
                        ? "Equipo creado correctamente"
                        : "Jugador creado correctamente"
                });

            _context.IdempotencyRecords.Add(new IdempotencyRecord
            {
                Key = key,
                Operation = operation,
                RequestHash = requestHash,
                StatusCode = 201,
                ResponseBody = body
            });

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            string? location = operation switch
            {
                OperationEquipos => $"/api/equipos/{result.Id}",
                OperationJugadores => $"/api/jugadores/{result.Id}",
                _ => null
            };

            return new IdempotencyExecutionResult(
                201,
                body,
                location);
        }
        catch (DbUpdateException)
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);

            var concurrent = await BuscarAsync(
                operation,
                key,
                CancellationToken.None);

            if (concurrent is not null)
            {
                return ReproducirORechazar(concurrent, requestHash);
            }

            throw;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);

            throw;
        }
    }

    private Task<IdempotencyRecord?> BuscarAsync(
        string operation,
        string key,
        CancellationToken cancellationToken)
    {
        return _context.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Operation == operation && x.Key == key,
                cancellationToken);
    }

    private static IdempotencyExecutionResult ReproducirORechazar(
        IdempotencyRecord record,
        string requestHash)
    {
        if (!string.Equals(
                record.RequestHash,
                requestHash,
                StringComparison.Ordinal))
        {
            return JsonResult(409, new
            {
                mensaje = "La clave ya se utilizó con datos diferentes.",
                codigo = "IDEMPOTENCY_KEY_REUTILIZADA"
            });
        }

        string? location = null;

        if (record.Operation != OperationGoles &&
            record.StatusCode == 201)
        {
            using var document = JsonDocument.Parse(record.ResponseBody);

            if (document.RootElement.TryGetProperty("id", out var id))
            {
                location = record.Operation == OperationEquipos
                    ? $"/api/equipos/{id.GetGuid()}"
                    : $"/api/jugadores/{id.GetGuid()}";
            }
        }

        return new IdempotencyExecutionResult(
            record.StatusCode,
            record.ResponseBody,
            location);
    }

    private static IdempotencyExecutionResult JsonResult(
        int statusCode,
        object body)
    {
        return new IdempotencyExecutionResult(
            statusCode,
            JsonSerializer.Serialize(body));
    }
}
