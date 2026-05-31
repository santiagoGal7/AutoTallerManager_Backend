using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class BahiaMecanicoAsignacionConfiguration : IEntityTypeConfiguration<BahiaMecanicoAsignacion>
{
    public void Configure(EntityTypeBuilder<BahiaMecanicoAsignacion> builder)
    {
        builder.ToTable("BahiaMecanicoAsignaciones");

        builder.HasKey(bma => bma.Id);

        builder.Property(bma => bma.FechaAsignacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relación Muchos a Uno con la Bahía Madre (Borrado en Cascada)
        builder.HasOne(bma => bma.BahiaServicio)
            .WithMany(bs => bs.MecanicosAsignados)
            .HasForeignKey(bma => bma.BahiaServicioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación Muchos a Uno con el Usuario Mecánico (Restricción de Borrado Físico)
        builder.HasOne(bma => bma.Mecanico)
            .WithMany() // No requiere colección inversa explícita en Usuario
            .HasForeignKey(bma => bma.UsuarioMecanicoId)
            .OnDelete(DeleteBehavior.Restrict); // Impide borrar físicamente al usuario si está asignado a un puesto
    }
}
