
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Equipos.Commands;

public sealed class ActualizarEquipoCommandHandler
{
    private readonly IEquipoRepository _equipoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarEquipoCommandHandler(
        IEquipoRepository equipoRepository,
        IUnitOfWork unitOfWork)
    {
        _equipoRepository = equipoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(
        ActualizarEquipoCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Id == Guid.Empty)
        {
            return Result<Guid>.Failure(
                "El identificador del equipo no es válido.",
                "ID_INVALIDO");
        }

        if (string.IsNullOrWhiteSpace(command.Nombre))
        {
            return Result<Guid>.Failure(
                "El nombre del equipo es obligatorio.",
                "NOMBRE_REQUERIDO");
        }

        if (string.IsNullOrWhiteSpace(command.Entrenador))
        {
            return Result<Guid>.Failure(
                "El nombre del entrenador es obligatorio.",
                "ENTRENADOR_REQUERIDO");
        }

        var equipo = await _equipoRepository.ObtenerPorIdAsync(
            command.Id,
            cancellationToken);

        if (equipo is null)
        {
            return Result<Guid>.Failure(
                "No se encontró el equipo solicitado.",
                "EQUIPO_NO_ENCONTRADO");
        }

        try
        {
            equipo.Actualizar(command.Nombre, command.Entrenador);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<Guid>.Success(equipo.Id);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);

            return Result<Guid>.Failure(
                ex.Message,
                "DATOS_INVALIDOS");
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}
