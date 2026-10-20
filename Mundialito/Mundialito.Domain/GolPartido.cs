
namespace Mundialito.Domain;

public class GolPartido
{
    public Guid Id { get; private set; }

    public Guid PartidoId { get; private set; }

    public Guid JugadorId { get; private set; }

    public int CantidadGoles { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public Partido Partido { get; private set; } = null!;

    public Jugador Jugador { get; private set; } = null!;

    private GolPartido()
    {
    }

    public GolPartido(
        Guid partidoId,
        Guid jugadorId,
        int cantidadGoles)
    {
        if (partidoId == Guid.Empty)
            throw new ArgumentException(
                "El partido es obligatorio.");

        if (jugadorId == Guid.Empty)
            throw new ArgumentException(
                "El jugador es obligatorio.");

        if (cantidadGoles <= 0)
            throw new ArgumentException(
                "La cantidad de goles debe ser mayor que cero.");

        Id = Guid.NewGuid();
        PartidoId = partidoId;
        JugadorId = jugadorId;
        CantidadGoles = cantidadGoles;
        FechaCreacion = DateTime.UtcNow;
    }

    public void ActualizarCantidadGoles(int cantidadGoles)
    {
        if (cantidadGoles <= 0)
            throw new ArgumentException(
                "La cantidad de goles debe ser mayor que cero.");

        CantidadGoles = cantidadGoles;
    }
}
