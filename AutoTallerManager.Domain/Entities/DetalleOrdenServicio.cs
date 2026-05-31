namespace AutoTallerManager.Domain.Entities;

public class DetalleOrdenServicio
{
    public int Id { get; set; }
    public int IdOrdenServicio { get; set; }
    public int IdServicioTaller { get; set; }
    public decimal PrecioManoObraHistorico { get; set; }
    public int HorasEstimadas { get; set; }

    // Propiedades de navegación virtuales para Entity Framework Core
    public virtual OrdenServicio OrdenServicio { get; set; } = null!;
    public virtual ServicioTaller ServicioTaller { get; set; } = null!;
}
