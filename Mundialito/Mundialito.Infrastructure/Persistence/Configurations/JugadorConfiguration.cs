
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain;

namespace Mundialito.Infrastructure.Persistence.Configurations;

public sealed class JugadorConfiguration
    : IEntityTypeConfiguration<Jugador>
{
    public void Configure(EntityTypeBuilder<Jugador> builder)
    {
        builder.ToTable("Jugadores");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(j => j.NumeroCamiseta)
            .IsRequired();

        builder.Property(j => j.Posicion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.FechaCreacion)
            .IsRequired();

        builder.HasOne(j => j.Equipo)
            .WithMany(e => e.Jugadores)
            .HasForeignKey(j => j.EquipoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(j => new { j.EquipoId, j.NumeroCamiseta })
            .IsUnique();
    }
}
