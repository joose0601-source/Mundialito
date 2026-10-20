
using Mundialito.Application.Common;
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Application.Features.Jugadores.Queries;

public sealed class ObtenerJugadoresQueryHandler
{
    private readonly IJugadorQueryRepository _jugadorQueryRepository;

    public ObtenerJugadoresQueryHandler(
        IJugadorQueryRepository jugadorQueryRepository)
    {
        _jugadorQueryRepository = jugadorQueryRepository;
    }

    public async Task<Result<PaginacionResult<JugadorDto>>> Handle(
        int pagina = 1,
        int tamanoPagina = 10,
        Guid? equipoId = null,
        string? nombre = null,
        string? posicion = null,
        string sortBy = "nombre",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        if (pagina < 1)
        {
            return Result<PaginacionResult<JugadorDto>>.Failure(
                "La página debe ser mayor que cero.");
        }

        if (tamanoPagina < 1 || tamanoPagina > 100)
        {
            return Result<PaginacionResult<JugadorDto>>.Failure(
                "El tamaño de página debe estar entre 1 y 100.");
        }

        var columnasPermitidas = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "nombre",
            "numeroCamiseta",
            "posicion",
            "fechaCreacion"
        };

        if (string.IsNullOrWhiteSpace(sortBy) ||
            !columnasPermitidas.Contains(sortBy))
        {
            return Result<PaginacionResult<JugadorDto>>.Failure(
                "El campo de ordenamiento no es válido.");
        }

        if (!string.Equals(sortDirection, "asc",
                StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(sortDirection, "desc",
                StringComparison.OrdinalIgnoreCase))
        {
            return Result<PaginacionResult<JugadorDto>>.Failure(
                "La dirección debe ser 'asc' o 'desc'.");
        }

        var resultado = await _jugadorQueryRepository.ObtenerTodosAsync(
            pagina,
            tamanoPagina,
            equipoId,
            nombre,
            posicion,
            sortBy,
            sortDirection,
            cancellationToken);

        return Result<PaginacionResult<JugadorDto>>.Success(resultado);
    }
}
