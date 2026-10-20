
using Microsoft.EntityFrameworkCore;
using Mundialito.Domain;

namespace Mundialito.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Equipo> Equipos => Set<Equipo>();

    public DbSet<Jugador> Jugadores => Set<Jugador>();

    public DbSet<Partido> Partidos => Set<Partido>();

    public DbSet<GolPartido> GolesPartido => Set<GolPartido>();

    public DbSet<IdempotencyRecord> IdempotencyRecords
        => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
