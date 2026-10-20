
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Mundialito.Application.Features.Equipos.Queries;
using Mundialito.Application.Interfaces;

namespace Mundialito.Infrastructure.Repositories;

public sealed class EquipoQueryRepository : IEquipoQueryRepository
{
    private readonly string _connectionString;

    public EquipoQueryRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DefaultConnection.");
    }

    public async Task<bool> ExisteAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT CAST(
                CASE WHEN EXISTS (
                    SELECT 1
                    FROM Equipos
                    WHERE Nombre = @Nombre
                )
                THEN 1 ELSE 0 END
            AS bit);
            """;

        using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new { Nombre = nombre },
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<PaginacionResult<EquipoDto>> ObtenerTodosAsync(
        int page,
        int pageSize,
        string? nombre = null,
        string? entrenador = null,
        string sortBy = "nombre",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var columnasPermitidas = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["nombre"] = "e.Nombre",
            ["entrenador"] = "e.Entrenador",
            ["fechaCreacion"] = "e.FechaCreacion"
        };

        if (!columnasPermitidas.TryGetValue(
                sortBy ?? "nombre",
                out var columnaOrdenamiento))
        {
            throw new ArgumentException(
                "El campo de ordenamiento no es válido.",
                nameof(sortBy));
        }

        var direccion = sortDirection?.ToLowerInvariant();

        if (direccion is not ("asc" or "desc"))
        {
            throw new ArgumentException(
                "La dirección debe ser 'asc' o 'desc'.",
                nameof(sortDirection));
        }

        var sql = $"""
            SELECT
                e.Id,
                e.Nombre,
                e.Entrenador,
                e.FechaCreacion
            FROM Equipos e
            WHERE (@Nombre IS NULL OR e.Nombre LIKE '%' + @Nombre + '%')
              AND (@Entrenador IS NULL OR e.Entrenador LIKE '%' + @Entrenador + '%')
            ORDER BY {columnaOrdenamiento} {direccion}, e.Id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM Equipos e
            WHERE (@Nombre IS NULL OR e.Nombre LIKE '%' + @Nombre + '%')
              AND (@Entrenador IS NULL OR e.Entrenador LIKE '%' + @Entrenador + '%');
            """;

        var offset = (page - 1) * pageSize;

        var parametros = new
        {
            Nombre = string.IsNullOrWhiteSpace(nombre) ? null : nombre.Trim(),
            Entrenador = string.IsNullOrWhiteSpace(entrenador)
                ? null
                : entrenador.Trim(),
            Offset = offset,
            PageSize = pageSize
        };

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                sql,
                parametros,
                cancellationToken: cancellationToken));

        var items = (await multi.ReadAsync<EquipoDto>()).AsList();
        var totalItems = await multi.ReadSingleAsync<int>();

        var totalPages = (int)Math.Ceiling(
            totalItems / (double)pageSize);

        return new PaginacionResult<EquipoDto>(
            items,
            page,
            pageSize,
            totalItems,
            totalPages);
    }

    public async Task<EquipoDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                Nombre,
                Entrenador,
                FechaCreacion
            FROM Equipos
            WHERE Id = @Id;
            """;

        using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<EquipoDto>(
            command);
    }
}
