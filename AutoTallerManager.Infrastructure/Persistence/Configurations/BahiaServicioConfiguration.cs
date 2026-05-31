using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class BahiaServicioConfiguration : IEntityTypeConfiguration<BahiaServicio>
{
    public void Configure(EntityTypeBuilder<BahiaServicio> builder)
    {
        builder.ToTable("BahiasServicio");

        builder.HasKey(bs => bs.Id);

        builder.Property(bs => bs.NombreBahia)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bs => bs.UbicacionFisica)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(bs => bs.EstadoDisponible)
            .HasDefaultValue(true);
    }
}
