using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class DetalleOrdenRepuestoConfiguration : IEntityTypeConfiguration<DetalleOrdenRepuesto>
{
    public void Configure(EntityTypeBuilder<DetalleOrdenRepuesto> builder)
    {
        builder.ToTable("DetallesOrdenRepuesto");

        builder.HasKey(dor => dor.Id);

        builder.Property(dor => dor.Cantidad)
            .IsRequired();

        builder.Property(dor => dor.PrecioVentaHistorico)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); // Preservación estricta de la 2FN

        // Relación Muchos a Uno con la Orden de Servicio Madre (Borrado en Cascada)
        builder.HasOne(dor => dor.OrdenServicio)
            .WithMany(os => os.DetallesRepuesto)
            .HasForeignKey(dor => dor.OrdenServicioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación lógica Muchos a Uno con el catálogo de Repuestos (Restricción de borrado)
        builder.HasOne<Repuesto>() // Se mapea directo a la entidad destino
            .WithMany()
            .HasForeignKey(dor => dor.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
