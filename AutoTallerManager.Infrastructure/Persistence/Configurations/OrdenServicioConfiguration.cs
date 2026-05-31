using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
{
    public void Configure(EntityTypeBuilder<OrdenServicio> builder)
    {
        builder.ToTable("OrdenesServicio");

        builder.HasKey(os => os.Id); // EF Core infiere Id desde la entidad

        builder.Property(os => os.TipoServicio)
            .IsRequired()
            .HasMaxLength(50); // Mantenimiento, Reparación, Diagnóstico

        builder.Property(os => os.Estado)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Pendiente");

        builder.Property(os => os.FechaIngreso)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(os => os.FechaEstimadaEntrega)
            .IsRequired();

        // RELACIONES RELACIONALES CRÍTICAS (CON RESTRICCIÓN DE BORRADO FÍSICO)
        builder.HasOne(os => os.Vehiculo)
            .WithMany(v => v.OrdenesServicio)
            .HasForeignKey(os => os.IdVehiculo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(os => os.Mecanico)
            .WithMany() // Usuario de rol mecánico
            .HasForeignKey(os => os.IdMecanico)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación 1 a 1 opcional con CitaTaller (Enlace lógico Cita-Orden)
        builder.HasOne(os => os.CitaTaller)
            .WithOne()
            .HasForeignKey<OrdenServicio>(os => os.IdCita)
            .OnDelete(DeleteBehavior.SetNull); // Si se elimina la cita, la orden se conserva
    }
}
