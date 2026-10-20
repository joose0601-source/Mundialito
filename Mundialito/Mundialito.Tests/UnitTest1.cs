
using Mundialito.Domain;
using Xunit;

namespace Mundialito.Tests;

public class PartidoTests
{
    private static Partido CrearPartido()
    {
        return new Partido(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1));
    }

    [Fact]
    public void Constructor_DebeCrearPartidoProgramado()
    {
        var partido = CrearPartido();

        Assert.NotEqual(Guid.Empty, partido.Id);
        Assert.Equal(EstadoPartido.Programado, partido.Estado);
        Assert.Null(partido.GolesLocal);
        Assert.Null(partido.GolesVisitante);
    }

    [Fact]
    public void Constructor_DebeRechazarElMismoEquipo()
    {
        var equipoId = Guid.NewGuid();

        var excepcion = Assert.Throws<ArgumentException>(() =>
            new Partido(
                equipoId,
                equipoId,
                DateTime.UtcNow.AddDays(1)));

        Assert.Contains("no puede jugar contra sí mismo", excepcion.Message);
    }

    [Fact]
    public void Constructor_DebeRechazarEquiposVacios()
    {
        Assert.Throws<ArgumentException>(() =>
            new Partido(
                Guid.Empty,
                Guid.NewGuid(),
                DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Constructor_DebeRechazarFechaVacia()
    {
        Assert.Throws<ArgumentException>(() =>
            new Partido(
                Guid.NewGuid(),
                Guid.NewGuid(),
                default));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-1, -1)]
    public void RegistrarResultado_DebeRechazarGolesNegativos(
        int golesLocal,
        int golesVisitante)
    {
        var partido = CrearPartido();

        Assert.Throws<ArgumentException>(() =>
            partido.RegistrarResultado(golesLocal, golesVisitante));
    }

    [Fact]
    public void RegistrarResultado_DebeFinalizarPartido()
    {
        var partido = CrearPartido();

        partido.RegistrarResultado(3, 1);

        Assert.Equal(EstadoPartido.Finalizado, partido.Estado);
        Assert.Equal(3, partido.GolesLocal);
        Assert.Equal(1, partido.GolesVisitante);
    }

    [Fact]
    public void RegistrarResultado_NoDebePermitirSegundoResultado()
    {
        var partido = CrearPartido();
        partido.RegistrarResultado(3, 1);

        Assert.Throws<InvalidOperationException>(() =>
            partido.RegistrarResultado(2, 0));
    }

    [Fact]
    public void RegistrarResultado_NoDebePermitirPartidoSuspendido()
    {
        var partido = CrearPartido();
        partido.CambiarEstado(EstadoPartido.Suspendido);

        Assert.Throws<InvalidOperationException>(() =>
            partido.RegistrarResultado(2, 1));
    }

    [Fact]
    public void ActualizarFecha_NoDebePermitirFechaVacia()
    {
        var partido = CrearPartido();

        Assert.Throws<ArgumentException>(() =>
            partido.ActualizarFecha(default));
    }

    [Fact]
    public void ActualizarFecha_NoDebePermitirCambiarPartidoFinalizado()
    {
        var partido = CrearPartido();
        partido.RegistrarResultado(1, 0);

        Assert.Throws<InvalidOperationException>(() =>
            partido.ActualizarFecha(DateTime.UtcNow.AddDays(2)));
    }

    [Fact]
    public void CambiarEstado_DebeRechazarEstadoInvalido()
    {
        var partido = CrearPartido();

        Assert.Throws<ArgumentException>(() =>
            partido.CambiarEstado((EstadoPartido)999));
    }
}
