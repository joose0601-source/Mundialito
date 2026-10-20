
namespace Mundialito.Domain;

public enum EstadoPartido
{
    Programado = 1,
    EnJuego = 2,
    Finalizado = 3,
    Suspendido = 4
}

public class Partido
{
    public Guid Id { get; private set; }

    public Guid EquipoLocalId { get; private set; }

    public Guid EquipoVisitanteId { get; private set; }

    public DateTime FechaPartido { get; private set; }

    public int? GolesLocal { get; private set; }

    public int? GolesVisitante { get; private set; }

    public EstadoPartido Estado { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public Equipo EquipoLocal { get; private set; } = null!;

    public Equipo EquipoVisitante { get; private set; } = null!;

    private Partido()
    {
    }

    public Partido(
        Guid equipoLocalId,
        Guid equipoVisitanteId,
        DateTime fechaPartido)
    {
        ValidarEquipos(equipoLocalId, equipoVisitanteId);

        if (fechaPartido == default)
            throw new ArgumentException(
                "La fecha del partido es obligatoria.");

        Id = Guid.NewGuid();
        EquipoLocalId = equipoLocalId;
        EquipoVisitanteId = equipoVisitanteId;
        FechaPartido = fechaPartido;
        Estado = EstadoPartido.Programado;
        FechaCreacion = DateTime.UtcNow;
    }


    public void RegistrarResultado(
        int golesLocal,
        int golesVisitante)
    {
        if (golesLocal < 0 || golesVisitante < 0)
            throw new ArgumentException(
                "Los goles no pueden ser negativos.");

        if (Estado == EstadoPartido.Suspendido)
            throw new InvalidOperationException(
                "No se puede registrar el resultado de un partido suspendido.");

        // Evitar modificar un partido que ya finalizó.
        if (Estado == EstadoPartido.Finalizado)
            throw new InvalidOperationException(
                "El partido ya está finalizado y tiene un resultado registrado.");

        GolesLocal = golesLocal;
        GolesVisitante = golesVisitante;
        Estado = EstadoPartido.Finalizado;
    }

    public void CambiarEstado(EstadoPartido estado)
    {
        if (!Enum.IsDefined(estado))
            throw new ArgumentException(
                "El estado del partido no es válido.");

        Estado = estado;
    }

    public void ActualizarFecha(DateTime fechaPartido)
    {
        if (fechaPartido == default)
            throw new ArgumentException(
                "La fecha del partido es obligatoria.");

        if (Estado == EstadoPartido.Finalizado)
            throw new InvalidOperationException(
                "No se puede cambiar la fecha de un partido finalizado.");

        FechaPartido = fechaPartido;
    }

    private static void ValidarEquipos(
        Guid equipoLocalId,
        Guid equipoVisitanteId)
    {
        if (equipoLocalId == Guid.Empty ||
            equipoVisitanteId == Guid.Empty)
        {
            throw new ArgumentException(
                "Los dos equipos son obligatorios.");
        }

        if (equipoLocalId == equipoVisitanteId)
            throw new ArgumentException(
                "Un equipo no puede jugar contra sí mismo.");
    }
}
