
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mundialito.Application.Features.Goles.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Infrastructure.Repositories;

public sealed class GoleadoresQueryRepository : IGoleadoresQueryRepository
{
    private readonly string _connectionString;

    public GoleadoresQueryRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se configuró DefaultConnection.");
    }

    public async Task<IReadOnlyList<GoleadorDto>> ObtenerTablaAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                j.Id AS JugadorId,
                j.Nombre AS NombreJugador,
                e.Nombre AS NombreEquipo,
                SUM(gp.CantidadGoles) AS Goles
            FROM dbo.GolesPartido gp
            INNER JOIN dbo.Jugadores j ON j.Id = gp.JugadorId
            INNER JOIN dbo.Equipos e ON e.Id = j.EquipoId
            INNER JOIN dbo.Partidos p ON p.Id = gp.PartidoId
            WHERE p.Estado = @EstadoFinalizado
            GROUP BY j.Id, j.Nombre, e.Nombre
            ORDER BY SUM(gp.CantidadGoles) DESC, j.Nombre ASC;
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new { EstadoFinalizado = 3 },
            cancellationToken: cancellationToken);

        var resultados = await connection.QueryAsync<GoleadorDto>(command);

        return resultados.AsList();
    }
}
