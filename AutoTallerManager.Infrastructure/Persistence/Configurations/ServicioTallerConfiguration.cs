using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class ServicioTallerConfiguration : IEntityTypeConfiguration<ServicioTaller>
{
    public void Configure(EntityTypeBuilder<ServicioTaller> builder)
    {
        builder.ToTable("ServiciosTaller");

        builder.HasKey(st => st.Id);

        builder.Property(st => st.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(st => st.Descripcion)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(st => st.TarifaBaseManoObra)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(st => st.Activo)
            .HasDefaultValue(true);
    }
}
