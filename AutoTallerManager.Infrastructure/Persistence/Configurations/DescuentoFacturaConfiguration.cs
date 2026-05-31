using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class DescuentoFacturaConfiguration : IEntityTypeConfiguration<DescuentoFactura>
{
    public void Configure(EntityTypeBuilder<DescuentoFactura> builder)
    {
        builder.ToTable("DescuentosFactura");

        builder.HasKey(df => df.Id);

        builder.Property(df => df.Descripcion)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(df => df.Porcentaje)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(df => df.MontoDescontado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Relación Muchos a Uno con la Factura Madre (Borrado en Cascada)
        builder.HasOne(df => df.Factura)
            .WithMany() // Conecta con la propiedad inversa si existiese, mapeo directo a la FK
            .HasForeignKey(df => df.IdFactura)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
