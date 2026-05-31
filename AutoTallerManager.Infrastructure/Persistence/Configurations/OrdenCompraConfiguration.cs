using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class OrdenCompraConfiguration : IEntityTypeConfiguration<OrdenCompra>
{
    public void Configure(EntityTypeBuilder<OrdenCompra> builder)
    {
        builder.ToTable("OrdenesCompra");

        builder.HasKey(oc => oc.Id);

        builder.Property(oc => oc.CodigoOrden)
            .IsRequired()
            .HasMaxLength(30);

        // Índice único para evitar duplicidades de códigos de tracking de compra
        builder.HasIndex(oc => oc.CodigoOrden)
            .IsUnique();

        builder.Property(oc => oc.FechaEmision)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(oc => oc.EstadoOrden)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Solicitado"); // 'Solicitado', 'En Transito', 'Recibido', 'Cancelado'

        // Relación Muchos a Uno con el Proveedor que surte la orden
        builder.HasOne(oc => oc.Proveedor)
            .WithMany()
            .HasForeignKey(oc => oc.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict); // No se puede borrar el proveedor si tiene órdenes de compra vigentes
    }
}
