using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Correo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.FechaRegistro)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relación Uno a Muchos: Un Cliente tiene muchos Vehículos
        builder.HasMany(c => c.Vehiculos)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.IdCliente)
            .OnDelete(DeleteBehavior.Restrict); // Impide borrar físicamente un cliente si tiene autos registrados
    }
}
