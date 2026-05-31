namespace AutoTallerManager.Domain.Entities;

public class DetalleOrdenRepuesto
{
    public int Id { get; set; }
    public int OrdenServicioId { get; set; }
    public int RepuestoId { get; set; } // Se relacionará con la entidad Repuesto
    public int Cantidad { get; set; }
    public decimal PrecioVentaHistorico { get; set; } // Esencial para preservar la 2FN

    // Relación de navegación
    public virtual OrdenServicio? OrdenServicio { get; set; }
}
