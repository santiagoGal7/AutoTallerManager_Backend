using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class HistorialKilometrajeConfiguration : IEntityTypeConfiguration<HistorialKilometraje>
{
    public void Configure(EntityTypeBuilder<HistorialKilometraje> builder)
    {
        builder.ToTable("HistorialesKilometraje");

        builder.HasKey(hk => hk.Id);

        builder.Property(hk => hk.Kilometraje)
            .IsRequired();

        builder.Property(hk => hk.FechaLectura)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(hk => hk.OrigenLectura)
            .IsRequired()
            .HasMaxLength(50);

        // Relación Muchos a Uno con Vehículo
        builder.HasOne(hk => hk.Vehiculo)
            .WithMany(v => v.HistorialesKilometraje)
            .HasForeignKey(hk => hk.VehiculoId)
            .OnDelete(DeleteBehavior.Cascade); // Si el auto se elimina por completo, su historial de kilometraje se limpia en cascada
    }
}
