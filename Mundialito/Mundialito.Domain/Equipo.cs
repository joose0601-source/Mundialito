
namespace Mundialito.Domain;

public class Equipo
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Entrenador { get; private set; } = string.Empty;
    public DateTime FechaCreacion { get; private set; }
    public ICollection<Jugador> Jugadores { get; private set; }
    = new List<Jugador>();
    private Equipo()
    {
    }

    public Equipo(string nombre, string entrenador)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del equipo es obligatorio.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(entrenador))
            throw new ArgumentException("El entrenador es obligatorio.", nameof(entrenador));

        Id = Guid.NewGuid();
        Nombre = nombre.Trim();
        Entrenador = entrenador.Trim();
        FechaCreacion = DateTime.UtcNow;
    }

    public void Actualizar(string nombre, string entrenador)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del equipo es obligatorio.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(entrenador))
            throw new ArgumentException("El entrenador es obligatorio.", nameof(entrenador));

        Nombre = nombre.Trim();
        Entrenador = entrenador.Trim();
    }
}
