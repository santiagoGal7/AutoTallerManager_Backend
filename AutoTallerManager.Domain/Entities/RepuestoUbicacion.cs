namespace AutoTallerManager.Domain.Entities;

public class RepuestoUbicacion
{
    public int Id { get; set; }
    public int RepuestoId { get; set; }
    public string Bodega { get; set; } = string.Empty; // Ej: 'Bodega Principal', 'Taller Zona Norte'
    public string Estante { get; set; } = string.Empty; // Ej: 'Seccion A-4'
    public int CapacidadMaxima { get; set; }

    // Relación con el repuesto base
    public virtual Repuesto? Repuesto { get; set; }
}
