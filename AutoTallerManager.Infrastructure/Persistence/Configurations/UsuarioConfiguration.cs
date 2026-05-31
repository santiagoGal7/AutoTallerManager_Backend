using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Correo)
            .IsRequired()
            .HasMaxLength(150);

        // Índice único para evitar duplicidad de credenciales en el Login
        builder.HasIndex(u => u.Correo)
            .IsUnique();

        builder.Property(u => u.ContrasenaHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(u => u.Activo)
            .HasDefaultValue(true);
    }
}
