
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Features.Jugadores.Queries;

namespace Mundialito.Application.Interfaces;

public interface IJugadorQueryRepository
{
    Task<bool> ExisteEquipoAsync(
        Guid equipoId,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteNumeroCamisetaAsync(
        Guid equipoId,
        int numeroCamiseta,
        Guid? excluirJugadorId = null,
        CancellationToken cancellationToken = default);

    Task<JugadorDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PaginacionResult<JugadorDto>> ObtenerTodosAsync(
        int pagina,
        int tamanoPagina,
        Guid? equipoId = null,
        string? nombre = null,
        string? posicion = null,
        string sortBy = "nombre",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default);
}
