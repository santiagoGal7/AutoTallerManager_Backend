namespace AutoTallerManager.Domain.Entities;

public class Vehiculo
{
    public int Id { get; set; }
    public int IdCliente { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string VIN { get; set; } = string.Empty; // Número de chasis único

    /// <summary>
    /// Características especiales del auto (ej: modificaciones, blindaje, accesorios).
    /// Mapeado directamente a una columna JSONB en PostgreSQL para máxima velocidad.
    /// </summary>
    public string EquipamientoJson { get; set; } = "{}"; 

    // Propiedades de navegación virtuales
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual ICollection<HistorialKilometraje> HistorialesKilometraje { get; set; } = new List<HistorialKilometraje>();
    public virtual ICollection<OrdenServicio> OrdenesServicio { get; set; } = new List<OrdenServicio>();
}
