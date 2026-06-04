using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class DetalleOrdenServicioConfiguration : IEntityTypeConfiguration<DetalleOrdenServicio>
{
    public void Configure(EntityTypeBuilder<DetalleOrdenServicio> builder)
    {
        builder.ToTable("DetallesOrdenServicio");

        builder.HasKey(dos => dos.Id);

        builder.Property(dos => dos.PrecioManoObraHistorico)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); // Resguardo estricto de la 2FN contra cambios de precios futuros

        builder.Property(dos => dos.HorasEstimadas)
            .IsRequired();

        builder.Property(dos => dos.HorasReales)
            .IsRequired(false)
            .HasColumnType("decimal(10,2)");

        // Relación Muchos a Uno con la Orden Madre (Cascada: Si cae la orden, cae su desglose)
        builder.HasOne(dos => dos.OrdenServicio)
            .WithMany(o => o.DetallesServicio)
            .HasForeignKey(dos => dos.IdOrdenServicio)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación Muchos a Uno con el Catálogo de Servicios
        builder.HasOne(dos => dos.ServicioTaller)
            .WithMany() // Conecta con el Catálogo de Servicios de forma limpia sin errores de tipos
            .HasForeignKey(dos => dos.IdServicioTaller)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
