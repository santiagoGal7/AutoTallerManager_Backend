using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class ProveedorContactoConfiguration : IEntityTypeConfiguration<ProveedorContacto>
{
    public void Configure(EntityTypeBuilder<ProveedorContacto> builder)
    {
        builder.ToTable("ProveedorContactos");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pc => pc.Cargo)
            .HasMaxLength(50)
            .HasDefaultValue(string.Empty);

        builder.Property(pc => pc.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(pc => pc.Correo)
            .HasMaxLength(100)
            .HasDefaultValue(string.Empty);

        // Relación Muchos a Uno con el maestro Proveedor
        builder.HasOne(pc => pc.Proveedor)
            .WithMany() // No es obligatoria una colección inversa explícita si no existe en la clase base
            .HasForeignKey(pc => pc.IdProveedor)
            .OnDelete(DeleteBehavior.Cascade); // Si se elimina la empresa proveedora, mueren sus contactos asociados
    }
}
