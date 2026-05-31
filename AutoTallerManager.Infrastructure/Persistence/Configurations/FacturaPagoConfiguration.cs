using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTallerManager.Infrastructure.Persistence.Configurations;

public class FacturaPagoConfiguration : IEntityTypeConfiguration<FacturaPago>
{
    public void Configure(EntityTypeBuilder<FacturaPago> builder)
    {
        builder.ToTable("FacturaPagos");

        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.MontoPagado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(fp => fp.FechaPago)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(fp => fp.TransaccionReferencia)
            .HasMaxLength(100)
            .HasDefaultValue(string.Empty); // Código de voucher o número de transferencia

        // Relaciones Muchos a Uno con Restricciones Estratégicas
        builder.HasOne(fp => fp.Factura)
            .WithMany(f => f.HistorialPagos)
            .HasForeignKey(fp => fp.FacturaId)
            .OnDelete(DeleteBehavior.Cascade); // Si se elimina la factura de borrador, se limpian sus intentos de pago

        builder.HasOne(fp => fp.MedioPago)
            .WithMany(mp => mp.PagosAsociados)
            .HasForeignKey(fp => fp.MedioPagoId)
            .OnDelete(DeleteBehavior.Restrict); // No puedes eliminar el medio 'Efectivo' si tiene transacciones asociadas
    }
}
