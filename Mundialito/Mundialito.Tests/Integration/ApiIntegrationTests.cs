
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Mundialito.Tests.Integration;

public sealed class ApiIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Raiz_DebeRetornarOk()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var contenido =
            await response.Content.ReadAsStringAsync();

        Assert.Contains(
            "API Mundialito funcionando correctamente",
            contenido);
    }

    [Fact]
    public async Task Get_Posiciones_DebeRetornarOk()
    {
        var response = await _client.GetAsync("/api/posiciones");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Posiciones_DebeRetornarJsonValido()
    {
        var response = await _client.GetAsync("/api/posiciones");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var contenido = await response.Content
            .ReadFromJsonAsync<List<PosicionPruebaDto>>();

        Assert.NotNull(contenido);
    }

    [Fact]
    public async Task Get_Posiciones_DebeContenerPropiedadesEsperadas()
    {
        var response = await _client.GetAsync("/api/posiciones");

        response.EnsureSuccessStatusCode();

        var contenido = await response.Content
            .ReadFromJsonAsync<List<PosicionPruebaDto>>();

        Assert.NotNull(contenido);

        foreach (var equipo in contenido)
        {
            Assert.NotEqual(Guid.Empty, equipo.EquipoId);
            Assert.False(
                string.IsNullOrWhiteSpace(equipo.NombreEquipo));

            Assert.True(equipo.PartidosJugados >= 0);
            Assert.True(equipo.PartidosGanados >= 0);
            Assert.True(equipo.PartidosEmpatados >= 0);
            Assert.True(equipo.PartidosPerdidos >= 0);
            Assert.True(equipo.GolesAFavor >= 0);
            Assert.True(equipo.GolesEnContra >= 0);
            Assert.Equal(
                equipo.GolesAFavor - equipo.GolesEnContra,
                equipo.DiferenciaGoles);

            Assert.True(equipo.Puntos >= 0);
        }
    }

    [Fact]
    public async Task Get_Posiciones_DebeTenerEquiposSinDuplicados()
    {
        var response = await _client.GetAsync("/api/posiciones");

        response.EnsureSuccessStatusCode();

        var contenido = await response.Content
            .ReadFromJsonAsync<List<PosicionPruebaDto>>();

        Assert.NotNull(contenido);

        var ids = contenido
            .Select(equipo => equipo.EquipoId)
            .ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }



    [Fact]
    public async Task Get_Posiciones_DebeCalcularPuntosCorrectamente()
    {
        // Act
        var response = await _client.GetAsync("/api/posiciones");

        // Assert
        response.EnsureSuccessStatusCode();

        var equipos = await response.Content
            .ReadFromJsonAsync<List<PosicionPruebaDto>>();

        Assert.NotNull(equipos);

        // Ningún equipo debe aparecer duplicado.
        Assert.Equal(
            equipos.Count,
            equipos.Select(e => e.EquipoId).Distinct().Count());

        // Las estadísticas no deben contener valores negativos.
        foreach (var equipo in equipos)
        {
            Assert.True(equipo.PartidosJugados >= 0);
            Assert.True(equipo.PartidosGanados >= 0);
            Assert.True(equipo.PartidosEmpatados >= 0);
            Assert.True(equipo.PartidosPerdidos >= 0);
            Assert.True(equipo.GolesAFavor >= 0);
            Assert.True(equipo.GolesEnContra >= 0);
            Assert.True(equipo.Puntos >= 0);

            // 3 puntos por victoria y 1 por empate.
            Assert.Equal(
                equipo.PartidosGanados * 3
                    + equipo.PartidosEmpatados,
                equipo.Puntos);

            // Los partidos jugados deben coincidir con sus resultados.
            Assert.Equal(
                equipo.PartidosGanados
                    + equipo.PartidosEmpatados
                    + equipo.PartidosPerdidos,
                equipo.PartidosJugados);

            Assert.Equal(
                equipo.GolesAFavor - equipo.GolesEnContra,
                equipo.DiferenciaGoles);
        }
    }

}
