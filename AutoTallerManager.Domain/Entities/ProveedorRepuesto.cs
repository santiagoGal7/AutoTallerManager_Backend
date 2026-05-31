namespace AutoTallerManager.Domain.Entities;

public class ProveedorRepuesto
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public int RepuestoId { get; set; }
    public decimal CostoCompraCotizado { get; set; }
    public TimeSpan TiempoEntregaEstimado { get; set; } // Tiempo que tarda el proveedor en surtir

    // Relaciones de navegación
    public virtual Proveedor? Proveedor { get; set; }
    public virtual Repuesto? Repuesto { get; set; }
}
