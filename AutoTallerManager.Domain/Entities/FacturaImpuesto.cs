namespace AutoTallerManager.Domain.Entities;

public class FacturaImpuesto
{
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public string NombreImpuesto { get; set; } = string.Empty; // Ej: 'IVA', 'ReteFuente'
    public decimal Porcentaje { get; set; } // Ej: 19.00 o 4.00
    public decimal MontoCalculado { get; set; }

    // Relación con la Factura base
    public virtual Factura? Factura { get; set; }
}
