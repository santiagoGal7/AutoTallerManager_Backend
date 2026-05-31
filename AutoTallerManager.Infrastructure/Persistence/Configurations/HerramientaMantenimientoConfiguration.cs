using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class HerramientaMantenimientoConfiguration : IEntityTypeConfiguration<HerramientaMantenimiento>
{
    public void Configure(EntityTypeBuilder<HerramientaMantenimiento> builder)
    {
        builder.ToTable("HerramientaMantenimientos");

        builder.HasKey(hm => hm.Id);

        builder.Property(hm => hm.FechaMantenimiento)
            .IsRequired();

        builder.Property(hm => hm.TipoMantenimiento)
            .IsRequired()
            .HasMaxLength(50); // 'Preventivo', 'Correctivo', 'Calibracion'

        builder.Property(hm => hm.DescripcionTrabajo)
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty);

        builder.Property(hm => hm.CostoMantenimiento)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Relación Muchos a Uno con la Herramienta Base (Borrado en Cascada)
        builder.HasOne(hm => hm.Herramienta)
            .WithMany(h => h.HistorialMantenimientos)
            .HasForeignKey(hm => hm.HerramientaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
