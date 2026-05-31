using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class HerramientaPrestamoConfiguration : IEntityTypeConfiguration<HerramientaPrestamo>
{
    public void Configure(EntityTypeBuilder<HerramientaPrestamo> builder)
    {
        builder.ToTable("HerramientaPrestamos");

        builder.HasKey(hp => hp.Id);

        builder.Property(hp => hp.FechaPrestamo)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(hp => hp.FechaDevolucion)
            .IsRequired(false);

        // RELACIONES RELACIONALES EN 4FN
        builder.HasOne(hp => hp.Herramienta)
            .WithMany(h => h.PrestamosActivos)
            .HasForeignKey(hp => hp.HerramientaId)
            .OnDelete(DeleteBehavior.Restrict); // No se puede borrar la herramienta si tiene un registro de préstamo

        builder.HasOne(hp => hp.Mecanico)
            .WithMany() // Usuario asignado
            .HasForeignKey(hp => hp.UsuarioMecanicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(hp => hp.OrdenServicio)
            .WithMany() // Asociado a la orden donde se ocupa
            .HasForeignKey(hp => hp.OrdenServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
