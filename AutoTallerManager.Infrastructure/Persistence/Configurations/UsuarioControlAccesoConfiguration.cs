using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class UsuarioControlAccesoConfiguration : IEntityTypeConfiguration<UsuarioControlAcceso>
{
    public void Configure(EntityTypeBuilder<UsuarioControlAcceso> builder)
    {
        builder.ToTable("UsuarioControlAccesos");

        builder.HasKey(uca => uca.Id);

        builder.Property(uca => uca.FechaEvento)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(uca => uca.TipoEvento)
            .IsRequired()
            .HasMaxLength(50); // 'Login_Exitoso', 'Intento_Fallido', 'Logout'

        builder.Property(uca => uca.DireccionIP)
            .IsRequired()
            .HasMaxLength(45); // Soporta formatos IPv4 e IPv6 estructurados

        builder.Property(uca => uca.DispositivoNavegador)
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);

        // Relación Muchos a Uno con el Usuario bajo observación (Borrado en Cascada)
        builder.HasOne(uca => uca.Usuario)
            .WithMany()
            .HasForeignKey(uca => uca.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
