
using Mundialito.Application.Common;
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Partidos.Queries;

public sealed class ObtenerPartidosQueryHandler
{
    private readonly IPartidoQueryRepository _repository;

    public ObtenerPartidosQueryHandler(
        IPartidoQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginacionResult<PartidoDto>>> Handle(
        int pagina,
        int tamanoPagina,
        Guid? equipoId = null,
        int? estado = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        string sortBy = "fechaPartido",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        if (pagina < 1)
        {
            return Result<PaginacionResult<PartidoDto>>.Failure(
                "La página debe ser mayor que cero.");
        }

        if (tamanoPagina < 1 || tamanoPagina > 100)
        {
            return Result<PaginacionResult<PartidoDto>>.Failure(
                "El tamaño de página debe estar entre 1 y 100.");
        }

        if (estado.HasValue && (estado.Value < 1 || estado.Value > 4))
        {
            return Result<PaginacionResult<PartidoDto>>.Failure(
                "El estado debe estar entre 1 y 4.");
        }

        if (fechaDesde.HasValue
            && fechaHasta.HasValue
            && fechaDesde.Value.Date > fechaHasta.Value.Date)
        {
            return Result<PaginacionResult<PartidoDto>>.Failure(
                "La fecha inicial no puede ser posterior a la fecha final.");
        }

        var total = await _repository.ContarAsync(
            equipoId,
            estado,
            fechaDesde,
            fechaHasta,
            cancellationToken);

        var items = await _repository.ObtenerTodosAsync(
            pagina,
            tamanoPagina,
            equipoId,
            estado,
            fechaDesde,
            fechaHasta,
            sortBy,
            sortDirection,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(
            total / (double)tamanoPagina);

        var resultado = new PaginacionResult<PartidoDto>(
            items,
            pagina,
            tamanoPagina,
            total,
            totalPages);

        return Result<PaginacionResult<PartidoDto>>.Success(
            resultado);
    }
}
