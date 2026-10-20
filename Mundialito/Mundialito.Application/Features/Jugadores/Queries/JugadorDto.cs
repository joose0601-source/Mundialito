
namespace Mundialito.Application.Features.Jugadores.Queries;

public sealed class JugadorDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int NumeroCamiseta { get; set; }
    public string Posicion { get; set; } = string.Empty;
    public Guid EquipoId { get; set; }
    public string NombreEquipo { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
