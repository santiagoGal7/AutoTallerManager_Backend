namespace AutoTallerManager.Domain.Entities;

public class GarantiaServicio
{
    public int Id { get; set; }
    public int OrdenServicioActualId { get; set; } // La orden donde se está aplicando la corrección
    public int OrdenServicioOrigenId { get; set; } // La orden vieja que falló
    public string MotivoFalla { get; set; } = string.Empty;
    public string ResolucionDictamen { get; set; } = string.Empty; // 'Aprobada (Costo Taller)', 'Rechazada'
    public DateTime FechaReclamo { get; set; } = DateTime.UtcNow;

    // Relaciones cruzadas de navegación
    public virtual OrdenServicio? OrdenServicioActual { get; set; }
    public virtual OrdenServicio? OrdenServicioOrigen { get; set; }
}
