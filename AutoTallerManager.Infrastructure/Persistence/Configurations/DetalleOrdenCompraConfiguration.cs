using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class DetalleOrdenCompraConfiguration : IEntityTypeConfiguration<DetalleOrdenCompra>
{
    public void Configure(EntityTypeBuilder<DetalleOrdenCompra> builder)
    {
        builder.ToTable("DetallesOrdenCompra");

        builder.HasKey(doc => doc.Id);

        builder.Property(doc => doc.CostoCompraPactado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(doc => doc.CantidadSolicitada)
            .IsRequired();

        builder.Property(doc => doc.CantidadRecibida)
            .IsRequired()
            .HasDefaultValue(0);

        // Relación Muchos a Uno con la Orden de Compra Madre (Borrado en Cascada)
        builder.HasOne(doc => doc.OrdenCompra)
            .WithMany(oc => oc.DetallesCompra)
            .HasForeignKey(doc => doc.IdOrdenCompra)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación Muchos a Uno con el Repuesto base (Restricción de borrado)
        builder.HasOne(doc => doc.Repuesto)
            .WithMany()
            .HasForeignKey(doc => doc.IdRepuesto)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
