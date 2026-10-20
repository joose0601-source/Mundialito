
namespace Mundialito.Application.Features.Goles.Queries;

public sealed class GoleadorDto
{
    public Guid JugadorId { get; set; }

    public string NombreJugador { get; set; } = string.Empty;

    public string NombreEquipo { get; set; } = string.Empty;

    public int Goles { get; set; }
}
