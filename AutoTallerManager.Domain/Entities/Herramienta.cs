namespace AutoTallerManager.Domain.Entities;

public class Herramienta
{
    public int Id { get; set; }
    public string CodigoActivo { get; set; } = string.Empty; // Número de inventario único interno
    public string Nombre { get; set; } = string.Empty; // Ej: 'Scanner Diagnostic Pro X'
    public string Marca { get; set; } = string.Empty;
    public string EstadoOperativo { get; set; } = "Disponible"; // 'Disponible', 'En Uso', 'Mantenimiento', 'Baja'
    public bool RequiereCalibracion { get; set; }

    // Propiedades de navegación virtuales para la granularidad 4FN
    public virtual ICollection<HerramientaMantenimiento> HistorialMantenimientos { get; set; } = new List<HerramientaMantenimiento>();
    public virtual ICollection<HerramientaPrestamo> PrestamosActivos { get; set; } = new List<HerramientaPrestamo>();
}
