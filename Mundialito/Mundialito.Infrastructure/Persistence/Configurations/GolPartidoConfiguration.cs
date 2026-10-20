
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain;

namespace Mundialito.Infrastructure.Persistence.Configurations;

public sealed class GolPartidoConfiguration
    : IEntityTypeConfiguration<GolPartido>
{
    public void Configure(EntityTypeBuilder<GolPartido> builder)
    {
        builder.ToTable("GolesPartido");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.CantidadGoles)
            .IsRequired();

        builder.Property(g => g.FechaCreacion)
            .IsRequired();

        builder.HasOne(g => g.Partido)
            .WithMany()
            .HasForeignKey(g => g.PartidoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Jugador)
            .WithMany()
            .HasForeignKey(g => g.JugadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(g => g.PartidoId);

        builder.HasIndex(g => g.JugadorId);
    }
}
