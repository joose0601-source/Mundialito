
namespace Mundialito.Application.Features.Jugadores.Commands;

public sealed record CrearJugadorCommand(
    string Nombre,
    int NumeroCamiseta,
    string Posicion,
    Guid EquipoId);
