using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class RepuestoUbicacionConfiguration : IEntityTypeConfiguration<RepuestoUbicacion>
{
    public void Configure(EntityTypeBuilder<RepuestoUbicacion> builder)
    {
        builder.ToTable("RepuestoUbicaciones");

        builder.HasKey(ru => ru.Id);

        builder.Property(ru => ru.Bodega)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ru => ru.Estante)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ru => ru.CapacidadMaxima)
            .IsRequired();

        // Relación Muchos a Uno con Repuesto (Cascada: Si se elimina el repuesto, se eliminan sus ubicaciones)
        builder.HasOne(ru => ru.Repuesto)
            .WithMany(r => r.Ubicaciones)
            .HasForeignKey(ru => ru.RepuestoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
