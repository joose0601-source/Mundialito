using Mundialito.Application.Features.Equipos.Commands;
using Mundialito.Application.Features.Jugadores.Commands;
using Mundialito.Application.Features.Goles;

namespace Mundialito.Application.Interfaces;

public interface IIdempotencyService
{
    Task<IdempotencyExecutionResult> CrearEquipoAsync(
        string key,
        CrearEquipoCommand command,
        CancellationToken cancellationToken = default);

    Task<IdempotencyExecutionResult> CrearJugadorAsync(
        string key,
        CrearJugadorCommand command,
        CancellationToken cancellationToken = default);

    Task<IdempotencyExecutionResult> RegistrarGolesAsync(
        string key,
        RegistrarGolesCommand command,
        CancellationToken cancellationToken = default);
}

public sealed record IdempotencyExecutionResult(
    int StatusCode,
    string ResponseBody,
    string? Location = null);