using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.ToTable("Vehiculos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Marca)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Modelo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Anio)
            .IsRequired();

        builder.Property(v => v.VIN)
            .IsRequired()
            .HasMaxLength(17);

        // Índice Único obligatorio para el número de serie/chasis del vehículo
        builder.HasIndex(v => v.VIN)
            .IsUnique();

        // 🐘 MAPEO JSONB DE ALTO RENDIMIENTO PARA POSTGRESQL
        builder.Property(v => v.EquipamientoJson)
            .HasColumnType("jsonb")
            .IsRequired()
            .HasDefaultValue("{}");

        // Relación Muchos a Uno con Cliente (Ya declarada a la inversa, pero se puede reforzar la FK)
        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Vehiculos)
            .HasForeignKey(v => v.IdCliente)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
