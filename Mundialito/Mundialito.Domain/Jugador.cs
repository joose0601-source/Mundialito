
namespace Mundialito.Domain;

public class Jugador
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public int NumeroCamiseta { get; private set; }
    public string Posicion { get; private set; } = string.Empty;
    public Guid EquipoId { get; private set; }
    public Equipo Equipo { get; private set; } = null!;
    public DateTime FechaCreacion { get; private set; }

    private Jugador()
    {
    }

    public Jugador(
        string nombre,
        int numeroCamiseta,
        string posicion,
        Guid equipoId)
    {
        Validar(nombre, numeroCamiseta, posicion, equipoId);

        Id = Guid.NewGuid();
        Nombre = nombre.Trim();
        NumeroCamiseta = numeroCamiseta;
        Posicion = posicion.Trim();
        EquipoId = equipoId;
        FechaCreacion = DateTime.UtcNow;
    }

    public void Actualizar(
        string nombre,
        int numeroCamiseta,
        string posicion)
    {
        Validar(nombre, numeroCamiseta, posicion, EquipoId);

        Nombre = nombre.Trim();
        NumeroCamiseta = numeroCamiseta;
        Posicion = posicion.Trim();
    }

    private static void Validar(
        string nombre,
        int numeroCamiseta,
        string posicion,
        Guid equipoId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre del jugador es obligatorio.");

        if (numeroCamiseta < 1 || numeroCamiseta > 99)
            throw new ArgumentException(
                "El número de camiseta debe estar entre 1 y 99.");

        if (string.IsNullOrWhiteSpace(posicion))
            throw new ArgumentException(
                "La posición del jugador es obligatoria.");

        if (equipoId == Guid.Empty)
            throw new ArgumentException(
                "Debe seleccionar un equipo válido.");
    }
}
