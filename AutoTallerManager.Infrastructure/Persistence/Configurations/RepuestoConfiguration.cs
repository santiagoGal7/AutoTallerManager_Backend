using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
{
    public void Configure(EntityTypeBuilder<Repuesto> builder)
    {
        builder.ToTable("Repuestos");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        // Índice único obligatorio para códigos SKU o de barras
        builder.HasIndex(r => r.Codigo)
            .IsUnique();

        builder.Property(r => r.Descripcion)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.Stock)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(r => r.PrecioUnitario)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.Activo)
            .HasDefaultValue(true);
    }
}
