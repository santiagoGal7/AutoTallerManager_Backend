using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class InsumoTallerConfiguration : IEntityTypeConfiguration<InsumoTaller>
{
    public void Configure(EntityTypeBuilder<InsumoTaller> builder)
    {
        builder.ToTable("InsumosTaller");

        builder.HasKey(it => it.Id);

        builder.Property(it => it.NombreInsumo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(it => it.StockActual)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(it => it.StockMinimoAlerta)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(it => it.UnidadMedida)
            .IsRequired()
            .HasMaxLength(20); // 'Litros', 'Unidades', 'Galones'
    }
}
