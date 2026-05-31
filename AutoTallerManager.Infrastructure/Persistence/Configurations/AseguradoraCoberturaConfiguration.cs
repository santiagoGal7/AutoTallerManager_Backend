using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class AseguradoraCoberturaConfiguration : IEntityTypeConfiguration<AseguradoraCobertura>
{
    public void Configure(EntityTypeBuilder<AseguradoraCobertura> builder)
    {
        builder.ToTable("AseguradoraCoberturas");

        builder.HasKey(ac => ac.Id);

        builder.Property(ac => ac.CodigoCobertura)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(ac => ac.Descripcion)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ac => ac.PorcentajeDeducible)
            .IsRequired()
            .HasColumnType("decimal(5,2)"); // Ejemplo: 10.00% o 5.50% de deducible obligatorio

        // Relación Muchos a Uno con la Aseguradora Madre (Borrado en Cascada)
        builder.HasOne(ac => ac.Aseguradora)
            .WithMany(a => a.CoberturasHomologadas)
            .HasForeignKey(ac => ac.AseguradoraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
