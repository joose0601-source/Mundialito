
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mundialito.Domain;

namespace Mundialito.Infrastructure.Persistence.Configurations;

public sealed class EquipoConfiguration : IEntityTypeConfiguration<Equipo>
{
    public void Configure(EntityTypeBuilder<Equipo> builder)
    {
        builder.ToTable("Equipos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Entrenador)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.FechaCreacion)
            .IsRequired();

        builder.HasIndex(e => e.Nombre)
            .IsUnique();
    }
}
