
using Mundialito.Application.Common;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Jugadores.Queries;

public sealed class ObtenerJugadorPorIdQueryHandler
{
    private readonly IJugadorQueryRepository _jugadorQueryRepository;

    public ObtenerJugadorPorIdQueryHandler(
        IJugadorQueryRepository jugadorQueryRepository)
    {
        _jugadorQueryRepository = jugadorQueryRepository;
    }

    public async Task<Result<JugadorDto>> Handle(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var jugador = await _jugadorQueryRepository.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (jugador is null)
            return Result<JugadorDto>.Failure("El jugador no existe.");

        return Result<JugadorDto>.Success(jugador);
    }
}
