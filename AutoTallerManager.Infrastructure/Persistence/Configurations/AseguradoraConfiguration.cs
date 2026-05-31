using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class AseguradoraConfiguration : IEntityTypeConfiguration<Aseguradora>
{
    public void Configure(EntityTypeBuilder<Aseguradora> builder)
    {
        builder.ToTable("Aseguradoras");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.RfcONit)
            .IsRequired()
            .HasMaxLength(50);

        // Índice único estricto para la identificación fiscal de la entidad corporativa
        builder.HasIndex(a => a.RfcONit)
            .IsUnique();

        builder.Property(a => a.RazonSocial)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.ContactoEmergencia)
            .HasMaxLength(100)
            .HasDefaultValue(string.Empty);

        builder.Property(a => a.CorreoCorporativo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Activa)
            .HasDefaultValue(true);
    }
}
