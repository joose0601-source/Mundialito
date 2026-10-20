
namespace Mundialito.Application.Features.Equipos.Queries;

public sealed record EquipoDto(
    Guid Id,
    string Nombre,
    string Entrenador,
    DateTime FechaCreacion);
