
using Mundialito.Application.Features.Partidos.Queries;

namespace Mundialito.Application.Interfaces;

public interface IPartidoQueryRepository
{
    Task<IReadOnlyList<PartidoDto>> ObtenerTodosAsync(
        int pagina,
        int tamanoPagina,
        Guid? equipoId = null,
        int? estado = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        string sortBy = "fechaPartido",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default);

    Task<int> ContarAsync(
        Guid? equipoId = null,
        int? estado = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        CancellationToken cancellationToken = default);

    Task<PartidoDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
