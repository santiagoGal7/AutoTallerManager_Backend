namespace AutoTallerManager.Domain.Entities;

public class DescuentoFactura
{
    public int Id { get; set; }
    public int IdFactura { get; set; }
    public string Descripcion { get; set; } = string.Empty; // Ej: 'Descuento Convenio Seguro'
    public decimal Porcentaje { get; set; }
    public decimal MontoDescontado { get; set; }

    // Propiedad de navegación hacia la factura principal
    public virtual Factura Factura { get; set; } = null!;
}
