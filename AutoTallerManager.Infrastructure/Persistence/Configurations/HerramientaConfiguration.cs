using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class HerramientaConfiguration : IEntityTypeConfiguration<Herramienta>
{
    public void Configure(EntityTypeBuilder<Herramienta> builder)
    {
        builder.ToTable("Herramientas");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.CodigoActivo)
            .IsRequired()
            .HasMaxLength(50);

        // Índice único estricto para número de activo interno del taller
        builder.HasIndex(h => h.CodigoActivo)
            .IsUnique();

        builder.Property(h => h.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.Marca)
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(h => h.EstadoOperativo)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Disponible"); // 'Disponible', 'En Uso', 'Mantenimiento', 'Baja'

        builder.Property(h => h.RequiereCalibracion)
            .HasDefaultValue(false);
    }
}
