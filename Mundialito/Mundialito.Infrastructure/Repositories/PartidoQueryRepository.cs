
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mundialito.Application.Features.Partidos.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Infrastructure.Repositories;

public sealed class PartidoQueryRepository : IPartidoQueryRepository
{
    private readonly IConfiguration _configuration;

    public PartidoQueryRepository(IConfiguration configuration)
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

    public async Task<IReadOnlyList<PartidoDto>> ObtenerTodosAsync(
        int pagina,
        int tamanoPagina,
        Guid? equipoId = null,
        int? estado = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        string sortBy = "fechaPartido",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        var columnasOrdenamiento = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["fechaPartido"] = "p.FechaPartido",
            ["estado"] = "p.Estado",
            ["fechaCreacion"] = "p.FechaCreacion",
            ["nombreEquipoLocal"] = "el.Nombre",
            ["nombreEquipoVisitante"] = "ev.Nombre",
            ["golesLocal"] = "p.GolesLocal",
            ["golesVisitante"] = "p.GolesVisitante"
        };

        if (!columnasOrdenamiento.TryGetValue(sortBy, out var columnaOrdenamiento))
        {
            throw new ArgumentException(
                "El campo de ordenamiento no está permitido.",
                nameof(sortBy));
        }

        if (!string.Equals(sortDirection, "asc",
                StringComparison.OrdinalIgnoreCase)
            && !string.Equals(sortDirection, "desc",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "La dirección debe ser 'asc' o 'desc'.",
                nameof(sortDirection));
        }

        var direccion = sortDirection.ToUpperInvariant();

        var sql = $"""
            SELECT
                p.Id,
                p.EquipoLocalId,
                el.Nombre AS NombreEquipoLocal,
                p.EquipoVisitanteId,
                ev.Nombre AS NombreEquipoVisitante,
                p.FechaPartido,
                p.GolesLocal,
                p.GolesVisitante,
                CASE p.Estado
                    WHEN 1 THEN 'Programado'
                    WHEN 2 THEN 'EnJuego'
                    WHEN 3 THEN 'Finalizado'
                    WHEN 4 THEN 'Suspendido'
                    ELSE 'Desconocido'
                END AS Estado,
                p.FechaCreacion
            FROM Partidos p
            INNER JOIN Equipos el ON el.Id = p.EquipoLocalId
            INNER JOIN Equipos ev ON ev.Id = p.EquipoVisitanteId
            WHERE (@EquipoId IS NULL
                   OR p.EquipoLocalId = @EquipoId
                   OR p.EquipoVisitanteId = @EquipoId)
              AND (@Estado IS NULL OR p.Estado = @Estado)
              AND (@FechaDesde IS NULL OR p.FechaPartido >= @FechaDesde)
            AND (@FechaHastaExclusiva IS NULL
            OR p.FechaPartido < @FechaHastaExclusiva)
            ORDER BY {columnaOrdenamiento} {direccion}, p.Id
            OFFSET @Offset ROWS
            FETCH NEXT @TamanoPagina ROWS ONLY;
            """;

        await using var conexion = CrearConexion();

        var parametros = new
        {
            EquipoId = equipoId,
            Estado = estado,
            FechaDesde = fechaDesde,
            FechaHastaExclusiva = fechaHasta?.Date.AddDays(1),
            Offset = (pagina - 1) * tamanoPagina,
            TamanoPagina = tamanoPagina
        };

        var comando = new CommandDefinition(
            sql,
            parametros,
            cancellationToken: cancellationToken);

        var partidos = await conexion.QueryAsync<PartidoDto>(comando);

        return partidos.AsList();
    }

    public async Task<int> ContarAsync(
        Guid? equipoId = null,
        int? estado = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM Partidos p
            WHERE (@EquipoId IS NULL
                   OR p.EquipoLocalId = @EquipoId
                   OR p.EquipoVisitanteId = @EquipoId)
              AND (@Estado IS NULL OR p.Estado = @Estado)
              AND (@FechaDesde IS NULL OR p.FechaPartido >= @FechaDesde)
            AND (@FechaHastaExclusiva IS NULL OR p.FechaPartido < @FechaHastaExclusiva);
            """;

        await using var conexion = CrearConexion();

        var parametros = new
        {
            EquipoId = equipoId,
            Estado = estado,
            FechaDesde = fechaDesde,
            FechaHastaExclusiva = fechaHasta?.Date.AddDays(1)
        };

        var comando = new CommandDefinition(
            sql,
            parametros,
            cancellationToken: cancellationToken);

        return await conexion.ExecuteScalarAsync<int>(comando);
    }

    public async Task<PartidoDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                p.Id,
                p.EquipoLocalId,
                el.Nombre AS NombreEquipoLocal,
                p.EquipoVisitanteId,
                ev.Nombre AS NombreEquipoVisitante,
                p.FechaPartido,
                p.GolesLocal,
                p.GolesVisitante,
                CASE p.Estado
                    WHEN 1 THEN 'Programado'
                    WHEN 2 THEN 'EnJuego'
                    WHEN 3 THEN 'Finalizado'
                    WHEN 4 THEN 'Suspendido'
                    ELSE 'Desconocido'
                END AS Estado,
                p.FechaCreacion
            FROM Partidos p
            INNER JOIN Equipos el ON el.Id = p.EquipoLocalId
            INNER JOIN Equipos ev ON ev.Id = p.EquipoVisitanteId
            WHERE p.Id = @Id;
            """;

        await using var conexion = CrearConexion();

        var comando = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken);

        return await conexion.QuerySingleOrDefaultAsync<PartidoDto>(comando);
    }
}
