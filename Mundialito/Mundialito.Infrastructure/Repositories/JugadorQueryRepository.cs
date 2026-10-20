
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Features.Jugadores.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Infrastructure.Repositories;

public sealed class JugadorQueryRepository : IJugadorQueryRepository
{
    private readonly string _connectionString;

    public JugadorQueryRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
    }

    public async Task<bool> ExisteEquipoAsync(
        Guid equipoId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Equipos
                WHERE Id = @EquipoId
            ) THEN 1 ELSE 0 END;
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new { EquipoId = equipoId },
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<bool> ExisteNumeroCamisetaAsync(
        Guid equipoId,
        int numeroCamiseta,
        Guid? excluirJugadorId = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Jugadores
                WHERE EquipoId = @EquipoId
                  AND NumeroCamiseta = @NumeroCamiseta
                  AND (@ExcluirJugadorId IS NULL OR Id <> @ExcluirJugadorId)
            ) THEN 1 ELSE 0 END;
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new
            {
                EquipoId = equipoId,
                NumeroCamiseta = numeroCamiseta,
                ExcluirJugadorId = excluirJugadorId
            },
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<JugadorDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                j.Id,
                j.Nombre,
                j.NumeroCamiseta,
                j.Posicion,
                j.EquipoId,
                e.Nombre AS NombreEquipo,
                j.FechaCreacion
            FROM Jugadores AS j
            INNER JOIN Equipos AS e ON e.Id = j.EquipoId
            WHERE j.Id = @Id;
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<JugadorDto>(command);
    }

    public async Task<PaginacionResult<JugadorDto>> ObtenerTodosAsync(
        int pagina,
        int tamanoPagina,
        Guid? equipoId = null,
        string? nombre = null,
        string? posicion = null,
        string sortBy = "nombre",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        var columnasPermitidas = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["nombre"] = "j.Nombre",
            ["numeroCamiseta"] = "j.NumeroCamiseta",
            ["posicion"] = "j.Posicion",
            ["fechaCreacion"] = "j.FechaCreacion"
        };

        if (string.IsNullOrWhiteSpace(sortBy) ||
            !columnasPermitidas.TryGetValue(sortBy, out var columnaOrdenamiento))
        {
            throw new ArgumentException(
                "El campo de ordenamiento no es válido.",
                nameof(sortBy));
        }

        if (!string.Equals(sortDirection, "asc",
                StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(sortDirection, "desc",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "La dirección debe ser 'asc' o 'desc'.",
                nameof(sortDirection));
        }

        pagina = Math.Max(1, pagina);
        tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

        var direccion = sortDirection.ToUpperInvariant();

        var filtro = """
            WHERE (@EquipoId IS NULL OR j.EquipoId = @EquipoId)
              AND (@Nombre IS NULL OR j.Nombre LIKE '%' + @Nombre + '%')
              AND (@Posicion IS NULL OR j.Posicion = @Posicion)
            """;

        var sqlDatos = $"""
            SELECT
                j.Id,
                j.Nombre,
                j.NumeroCamiseta,
                j.Posicion,
                j.EquipoId,
                e.Nombre AS NombreEquipo,
                j.FechaCreacion
            FROM Jugadores AS j
            INNER JOIN Equipos AS e ON e.Id = j.EquipoId
            {filtro}
            ORDER BY {columnaOrdenamiento} {direccion}, j.Id
            OFFSET @Desplazamiento ROWS
            FETCH NEXT @TamanoPagina ROWS ONLY;

            SELECT COUNT(*)
            FROM Jugadores AS j
            {filtro};
            """;

        var parametros = new
        {
            EquipoId = equipoId,
            Nombre = string.IsNullOrWhiteSpace(nombre) ? null : nombre.Trim(),
            Posicion = string.IsNullOrWhiteSpace(posicion) ? null : posicion.Trim(),
            Desplazamiento = (pagina - 1) * tamanoPagina,
            TamanoPagina = tamanoPagina
        };

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                sqlDatos,
                parametros,
                cancellationToken: cancellationToken));

        var jugadores = (await multi.ReadAsync<JugadorDto>()).AsList();
        var total = await multi.ReadSingleAsync<int>();

        var totalPages = (int)Math.Ceiling(
            total / (double)tamanoPagina);

        return new PaginacionResult<JugadorDto>(
            jugadores,
            pagina,
            tamanoPagina,
            total,
            totalPages);
    }
}
