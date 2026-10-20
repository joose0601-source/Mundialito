
namespace Mundialito.Application.Features.Partidos.Queries;

public sealed record PartidoDto(
    Guid Id,
    Guid EquipoLocalId,
    string NombreEquipoLocal,
    Guid EquipoVisitanteId,
    string NombreEquipoVisitante,
    DateTime FechaPartido,
    int? GolesLocal,
    int? GolesVisitante,
    string Estado,
    DateTime FechaCreacion);
