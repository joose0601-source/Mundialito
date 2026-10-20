using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mundialito.Application.Interfaces;
using Mundialito.Domain;
using Mundialito.Infrastructure.Persistence;

namespace Mundialito.Infrastructure.Repositories;

public sealed class GolPartidoRepository : IGolPartidoRepository
{
    private readonly ApplicationDbContext _context;
    private readonly string _connectionString;

    public GolPartidoRepository(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión DefaultConnection.");
    }

    public async Task AgregarAsync(
        GolPartido golPartido,
        CancellationToken cancellationToken = default)
    {
        await _context.GolesPartido.AddAsync(
            golPartido,
            cancellationToken);
    }

    public async Task<int> ObtenerGolesRegistradosPorEquipoAsync(
        Guid partidoId,
        Guid equipoId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COALESCE(SUM(gp.CantidadGoles), 0)
            FROM dbo.GolesPartido gp
            INNER JOIN dbo.Jugadores j
                ON j.Id = gp.JugadorId
            WHERE gp.PartidoId = @PartidoId
              AND j.EquipoId = @EquipoId;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new
            {
                PartidoId = partidoId,
                EquipoId = equipoId
            },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<int>(command);
    }
}