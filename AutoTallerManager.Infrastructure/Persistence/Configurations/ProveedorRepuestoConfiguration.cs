using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class ProveedorRepuestoConfiguration : IEntityTypeConfiguration<ProveedorRepuesto>
{
    public void Configure(EntityTypeBuilder<ProveedorRepuesto> builder)
    {
        builder.ToTable("ProveedorRepuestos");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.CostoCompraCotizado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Mapea el TimeSpan como un tipo compatible con intervalos/duración en PostgreSQL
        builder.Property(pr => pr.TiempoEntregaEstimado)
            .IsRequired();

        // Relaciones Muchos a Uno cruzadas con Restrict/Cascade según corresponda
        builder.HasOne(pr => pr.Repuesto)
            .WithMany(r => r.ProveedoresHomologados)
            .HasForeignKey(pr => pr.RepuestoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pr => pr.Proveedor)
            .WithMany(p => p.RepuestosSuministrados)
            .HasForeignKey(pr => pr.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict); // No se puede borrar un proveedor si suministra repuestos activos
    }
}
