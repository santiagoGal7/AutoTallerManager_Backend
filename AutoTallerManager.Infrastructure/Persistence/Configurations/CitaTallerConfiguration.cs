using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class CitaTallerConfiguration : IEntityTypeConfiguration<CitaTaller>
{
    public void Configure(EntityTypeBuilder<CitaTaller> builder)
    {
        builder.ToTable("CitasTaller");

        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.FechaHoraReserva)
            .IsRequired();

        builder.Property(ct => ct.EstadoCita)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Programada");

        builder.Property(ct => ct.NotasSintomas)
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty);

        // Relaciones Muchos a Uno con Restricción de Borrado
        builder.HasOne(ct => ct.Cliente)
            .WithMany()
            .HasForeignKey(ct => ct.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ct => ct.Vehiculo)
            .WithMany()
            .HasForeignKey(ct => ct.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ct => ct.ServicioTaller)
            .WithMany()
            .HasForeignKey(ct => ct.ServicioTallerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
