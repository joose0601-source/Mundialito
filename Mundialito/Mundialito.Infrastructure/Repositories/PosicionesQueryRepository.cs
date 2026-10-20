
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mundialito.Application.Features.Posiciones.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Infrastructure.Repositories;

public sealed class PosicionesQueryRepository : IPosicionesQueryRepository
{
    private readonly IConfiguration _configuration;

    public PosicionesQueryRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection CrearConexion()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
        }

        return new SqlConnection(connectionString);
    }

    public async Task<IReadOnlyList<PosicionEquipoDto>> ObtenerTablaAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            WITH Estadisticas AS
            (
                SELECT
                    e.Id AS EquipoId,
                    e.Nombre AS NombreEquipo,

                    COUNT(p.Id) AS PartidosJugados,

            SUM(CASE
                WHEN p.EquipoLocalId = e.Id
                 AND p.GolesLocal > p.GolesVisitante
                THEN 1 ELSE 0
            END) AS VictoriasLocal,

            SUM(CASE
                WHEN p.EquipoVisitanteId = e.Id
                 AND p.GolesVisitante > p.GolesLocal
                THEN 1 ELSE 0
            END) AS VictoriasVisitante,

                    SUM(CASE
                        WHEN p.Id IS NOT NULL
                         AND p.GolesLocal = p.GolesVisitante
                        THEN 1 ELSE 0
                    END) AS Empates,

                    SUM(CASE
                        WHEN p.EquipoLocalId = e.Id
                        THEN p.GolesLocal
                        WHEN p.EquipoVisitanteId = e.Id
                        THEN p.GolesVisitante
                        ELSE 0
                    END) AS GolesAFavor,

                    SUM(CASE
                        WHEN p.EquipoLocalId = e.Id
                        THEN p.GolesVisitante
                        WHEN p.EquipoVisitanteId = e.Id
                        THEN p.GolesLocal
                        ELSE 0
                    END) AS GolesEnContra

                FROM Equipos e
                LEFT JOIN Partidos p
                    ON (
                        p.EquipoLocalId = e.Id
                        OR p.EquipoVisitanteId = e.Id
                    )
                    AND p.Estado = 3
                    AND p.GolesLocal IS NOT NULL
                    AND p.GolesVisitante IS NOT NULL

                GROUP BY e.Id, e.Nombre
            )
            SELECT
                EquipoId,
                NombreEquipo,
                PartidosJugados,

                (VictoriasLocal + VictoriasVisitante)
                    AS PartidosGanados,

                Empates AS PartidosEmpatados,

                (PartidosJugados - VictoriasLocal
                    - VictoriasVisitante - Empates)
                    AS PartidosPerdidos,

                GolesAFavor,
                GolesEnContra,
                (GolesAFavor - GolesEnContra)
                    AS DiferenciaGoles,

                ((VictoriasLocal + VictoriasVisitante) * 3
                    + Empates)
                    AS Puntos

            FROM Estadisticas
            ORDER BY
                Puntos DESC,
                DiferenciaGoles DESC,
                GolesAFavor DESC,
                NombreEquipo ASC;
            """;

        await using var conexion = CrearConexion();

        var comando = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        var posiciones =
            await conexion.QueryAsync<PosicionEquipoDto>(comando);

        return posiciones.AsList();
    }
}
