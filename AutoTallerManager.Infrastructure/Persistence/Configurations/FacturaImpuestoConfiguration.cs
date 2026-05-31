using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class FacturaImpuestoConfiguration : IEntityTypeConfiguration<FacturaImpuesto>
{
    public void Configure(EntityTypeBuilder<FacturaImpuesto> builder)
    {
        builder.ToTable("FacturaImpuestos");

        builder.HasKey(fi => fi.Id);

        builder.Property(fi => fi.NombreImpuesto)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(fi => fi.Porcentaje)
            .IsRequired()
            .HasColumnType("decimal(5,2)"); // Permite guardar tasas como 19.00% o 4.50%

        builder.Property(fi => fi.MontoCalculado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Relación Muchos a Uno con la Factura Madre (Borrado en Cascada)
        builder.HasOne(fi => fi.Factura)
            .WithMany(f => f.ImpuestosAplicados)
            .HasForeignKey(fi => fi.FacturaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
