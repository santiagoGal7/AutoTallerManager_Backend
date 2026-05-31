using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class BahiaHistorialEstadoConfiguration : IEntityTypeConfiguration<BahiaHistorialEstado>
{
    public void Configure(EntityTypeBuilder<BahiaHistorialEstado> builder)
    {
        builder.ToTable("BahiaHistorialEstados");

        builder.HasKey(bhe => bhe.Id);

        builder.Property(bhe => bhe.Estado)
            .IsRequired()
            .HasMaxLength(50); // 'Disponible', 'Ocupada', 'Mantenimiento'

        builder.Property(bhe => bhe.FechaCambio)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(bhe => bhe.Observaciones)
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        // Relación Muchos a Uno con la Bahía Base
        builder.HasOne(bhe => bhe.BahiaServicio)
            .WithMany() // Mapeo directo utilizando la navegación virtual de la entidad
            .HasForeignKey(bhe => bhe.IdBahiaServicio)
            .OnDelete(DeleteBehavior.Cascade); // Si se elimina el espacio, cae su histórico logístico
    }
}
