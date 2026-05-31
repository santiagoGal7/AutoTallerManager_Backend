namespace AutoTallerManager.Domain.Entities;

public class FacturaPago
{
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int MedioPagoId { get; set; } // FK hacia el catálogo de medios de pago
    public decimal MontoPagado { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.UtcNow;
    public string TransaccionReferencia { get; set; } = string.Empty; // Código de voucher o transferencia

    // Relaciones de navegación
    public virtual Factura? Factura { get; set; }
    public virtual MedioPago? MedioPago { get; set; }
}
