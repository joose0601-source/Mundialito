
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain;

namespace Mundialito.Infrastructure.Persistence.Configurations;

public sealed class PartidoConfiguration
    : IEntityTypeConfiguration<Partido>
{
    public void Configure(EntityTypeBuilder<Partido> builder)
    {
        builder.ToTable("Partidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FechaPartido)
            .IsRequired();

        builder.Property(p => p.GolesLocal)
            .IsRequired(false);

        builder.Property(p => p.GolesVisitante)
            .IsRequired(false);

        builder.Property(p => p.Estado)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.FechaCreacion)
            .IsRequired();

        builder.HasOne(p => p.EquipoLocal)
            .WithMany()
            .HasForeignKey(p => p.EquipoLocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.EquipoVisitante)
            .WithMany()
            .HasForeignKey(p => p.EquipoVisitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.FechaPartido);
    }
}
