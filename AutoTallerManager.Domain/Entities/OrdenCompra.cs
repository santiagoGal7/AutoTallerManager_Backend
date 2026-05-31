namespace AutoTallerManager.Domain.Entities;

public class OrdenCompra
{
    public int Id { get; set; }
    public int ProveedorId { get; set; } // FK a Proveedor
    public string CodigoOrden { get; set; } = string.Empty; // Ej: 'OC-2026-0089'
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public string EstadoOrden { get; set; } = "Solicitado"; // 'Solicitado', 'En Transito', 'Recibido', 'Cancelado'

    // Relación de navegación con el proveedor
    public virtual Proveedor? Proveedor { get; set; }

    // Propiedades de navegación para granularidad transaccional 4FN
    public virtual ICollection<DetalleOrdenCompra> DetallesCompra { get; set; } = new List<DetalleOrdenCompra>();
}
