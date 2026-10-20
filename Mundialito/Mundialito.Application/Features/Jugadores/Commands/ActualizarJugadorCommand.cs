
namespace Mundialito.Application.Features.Jugadores.Commands;

public sealed record ActualizarJugadorCommand(
    Guid Id,
    string Nombre,
    int NumeroCamiseta,
    string Posicion);
