
namespace Mundialito.Application.Features.Equipos.Commands;

public sealed record ActualizarEquipoCommand(
    Guid Id,
    string Nombre,
    string Entrenador);
