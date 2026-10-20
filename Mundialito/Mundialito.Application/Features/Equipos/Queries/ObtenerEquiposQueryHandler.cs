
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Equipos.Queries;

public sealed class ObtenerEquiposQueryHandler
{
    private readonly IEquipoQueryRepository _equipoQueryRepository;

    public ObtenerEquiposQueryHandler(
        IEquipoQueryRepository equipoQueryRepository)
    {
        _equipoQueryRepository = equipoQueryRepository;
    }

    public async Task<PaginacionResult<EquipoDto>> HandleAsync(
        int page,
        int pageSize,
        string? nombre = null,
        string? entrenador = null,
        string sortBy = "nombre",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var columnasPermitidas = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "nombre",
            "entrenador",
            "fechaCreacion"
        };

        if (!columnasPermitidas.Contains(sortBy))
        {
            throw new ArgumentException(
                "El campo de ordenamiento no es válido.",
                nameof(sortBy));
        }

        if (sortDirection is not null &&
            !sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "La dirección debe ser 'asc' o 'desc'.",
                nameof(sortDirection));
        }

        return await _equipoQueryRepository.ObtenerTodosAsync(
            page,
            pageSize,
            nombre,
            entrenador,
            sortBy,
            sortDirection,
            cancellationToken);
    }
}
