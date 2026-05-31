using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class GarantiaServicioConfiguration : IEntityTypeConfiguration<GarantiaServicio>
{
    public void Configure(EntityTypeBuilder<GarantiaServicio> builder)
    {
        builder.ToTable("GarantiasServicio");

        builder.HasKey(gs => gs.Id);

        builder.Property(gs => gs.MotivoFalla)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(gs => gs.ResolucionDictamen)
            .IsRequired()
            .HasMaxLength(100); // Ej: 'Aprobada (Costo Taller)', 'Rechazada'

        builder.Property(gs => gs.FechaReclamo)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // ⚠️ RELACIONES MÚLTIPLES CRUZADAS A LA MISMA TABLA (BLINDADO OBLIGATORIO CON RESTRICT)
        builder.HasOne(gs => gs.OrdenServicioActual)
            .WithMany() // Orden nueva donde se ejecuta el re-trabajo corrector
            .HasForeignKey(gs => gs.OrdenServicioActualId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(gs => gs.OrdenServicioOrigen)
            .WithMany() // Orden vieja original que falló y detonó el reclamo
            .HasForeignKey(gs => gs.OrdenServicioOrigenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
