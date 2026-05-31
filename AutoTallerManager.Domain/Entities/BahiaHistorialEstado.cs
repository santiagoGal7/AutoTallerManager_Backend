namespace AutoTallerManager.Domain.Entities;

public class BahiaHistorialEstado
{
    public int Id { get; set; }
    public int IdBahiaServicio { get; set; }
    public string Estado { get; set; } = string.Empty; // 'Disponible', 'Ocupada', 'Mantenimiento'
    public DateTime FechaCambio { get; set; }
    public string Observaciones { get; set; } = string.Empty;

    // Propiedad de navegación
    public virtual BahiaServicio BahiaServicio { get; set; } = null!;
}
