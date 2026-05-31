using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class MecanicoCertificacionConfiguration : IEntityTypeConfiguration<MecanicoCertificacion>
{
    public void Configure(EntityTypeBuilder<MecanicoCertificacion> builder)
    {
        builder.ToTable("MecanicoCertificaciones");

        builder.HasKey(mc => mc.Id);

        builder.Property(mc => mc.NombreCertificacion)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(mc => mc.EnteEmisor)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mc => mc.FechaExpiracion)
            .IsRequired();

        // Relación Muchos a Uno con Usuario
        builder.HasOne(mc => mc.Usuario)
            .WithMany(u => u.Certificaciones)
            .HasForeignKey(mc => mc.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
