using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Facturas");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.NumeroFactura)
            .IsRequired()
            .HasMaxLength(30);

        // Índice único para evitar duplicidades de folios fiscales o facturas
        builder.HasIndex(f => f.NumeroFactura)
            .IsUnique();

        builder.Property(f => f.SubtotalManoObra)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(f => f.SubtotalRepuestos)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(f => f.TotalImpuestos)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(f => f.TotalNeto)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(f => f.FechaEmision)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(f => f.EstadoPago)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Pendiente"); // 'Pendiente', 'Parcial', 'Pagada', 'Anulada'

        // Relación 1 a 1 estricta con la Orden de Servicio
        builder.HasOne(f => f.OrdenServicio)
            .WithOne() // Relación unívoca sin colección inversa obligatoria en OrdenServicio
            .HasForeignKey<Factura>(f => f.OrdenServicioId)
            .OnDelete(DeleteBehavior.Restrict); // No se puede borrar una orden si ya tiene una factura emitida
    }
}
