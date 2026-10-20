
namespace Mundialito.Application.Features.Partidos.Commands;

public sealed record RegistrarResultadoCommand(
    Guid PartidoId,
    int GolesLocal,
    int GolesVisitante);
