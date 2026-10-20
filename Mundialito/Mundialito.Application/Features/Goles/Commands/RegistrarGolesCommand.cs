
namespace Mundialito.Application.Features.Goles;

public sealed record RegistrarGolesCommand(
    Guid PartidoId,
    Guid JugadorId,
    int CantidadGoles);
