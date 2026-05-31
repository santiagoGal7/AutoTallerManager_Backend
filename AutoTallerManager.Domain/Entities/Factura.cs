namespace AutoTallerManager.Domain.Entities;

public class Factura
{
    public int Id { get; set; }
    public int OrdenServicioId { get; set; }
    public string NumeroFactura { get; set; } = string.Empty; // Ej: 'FACT-2026-0001'
    public decimal SubtotalManoObra { get; set; }
    public decimal SubtotalRepuestos { get; set; }
    public decimal TotalImpuestos { get; set; }
    public decimal TotalNeto { get; set; } // Calculado como Subtotal + Impuestos
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public string EstadoPago { get; set; } = "Pendiente"; // 'Pendiente', 'Parcial', 'Pagada', 'Anulada'

    // Relación de navegación 1 a 1 con la Orden de Servicio
    public virtual OrdenServicio? OrdenServicio { get; set; }

    // Propiedades de navegación para granularidad 4FN
    public virtual ICollection<FacturaImpuesto> ImpuestosAplicados { get; set; } = new List<FacturaImpuesto>();
    public virtual ICollection<FacturaPago> HistorialPagos { get; set; } = new List<FacturaPago>();
}
