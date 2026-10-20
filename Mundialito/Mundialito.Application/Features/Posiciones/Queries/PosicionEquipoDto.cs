
namespace Mundialito.Application.Features.Posiciones.Queries;

public sealed class PosicionEquipoDto
{
    public Guid EquipoId { get; init; }

    public string NombreEquipo { get; init; } = string.Empty;

    public int PartidosJugados { get; init; }

    public int PartidosGanados { get; init; }

    public int PartidosEmpatados { get; init; }

    public int PartidosPerdidos { get; init; }

    public int GolesAFavor { get; init; }

    public int GolesEnContra { get; init; }

    public int DiferenciaGoles { get; init; }

    public int Puntos { get; init; }
}
