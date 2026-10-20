
namespace Mundialito.Application.Features.Partidos.Commands;

public sealed record CrearPartidoCommand(
    Guid EquipoLocalId,
    Guid EquipoVisitanteId,
    DateTime FechaPartido);
