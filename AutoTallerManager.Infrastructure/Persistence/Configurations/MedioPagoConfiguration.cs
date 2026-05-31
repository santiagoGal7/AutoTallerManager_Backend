using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class MedioPagoConfiguration : IEntityTypeConfiguration<MedioPago>
{
    public void Configure(EntityTypeBuilder<MedioPago> builder)
    {
        builder.ToTable("MediosPago");

        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(mp => mp.PermiteCuotas)
            .IsRequired();

        builder.Property(mp => mp.Activo)
            .HasDefaultValue(true);
    }
}
