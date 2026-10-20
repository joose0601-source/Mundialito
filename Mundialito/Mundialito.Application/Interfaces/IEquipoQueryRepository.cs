
using Mundialito.Application.Features.Equipos.Queries;

namespace Mundialito.Application.Interfaces;

public interface IEquipoQueryRepository
{
    Task<bool> ExisteAsync(
        string nombre,
        CancellationToken cancellationToken = default);

    Task<PaginacionResult<EquipoDto>> ObtenerTodosAsync(
        int page,
        int pageSize,
        string? nombre = null,
        string? entrenador = null,
        string sortBy = "nombre",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default);

    Task<EquipoDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
