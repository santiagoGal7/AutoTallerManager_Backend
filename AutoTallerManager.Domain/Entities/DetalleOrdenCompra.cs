namespace AutoTallerManager.Domain.Entities;

public class DetalleOrdenCompra
{
    public int Id { get; set; }
    public int IdOrdenCompra { get; set; }
    public int IdRepuesto { get; set; }
    public decimal CostoCompraPactado { get; set; }
    
    // Columnas unificadas que absorben la lógica de recepción de almacén
    public int CantidadSolicitada { get; set; }
    public int CantidadRecibida { get; set; }

    // Propiedades de navegación virtuales
    public virtual OrdenCompra OrdenCompra { get; set; } = null!;
    public virtual Repuesto Repuesto { get; set; } = null!;
}
