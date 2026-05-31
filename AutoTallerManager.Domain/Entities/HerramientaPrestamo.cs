namespace AutoTallerManager.Domain.Entities;

public class HerramientaPrestamo
{
    public int Id { get; set; }
    public int HerramientaId { get; set; }
    public int UsuarioMecanicoId { get; set; } // FK a Usuario (Rol Mecánico)
    public int OrdenServicioId { get; set; } // En qué orden se está usando
    public DateTime FechaPrestamo { get; set; } = DateTime.UtcNow;
    public DateTime? FechaDevolucion { get; set; }

    // Relaciones de navegación
    public virtual Herramienta? Herramienta { get; set; }
    public virtual Usuario? Mecanico { get; set; }
    public virtual OrdenServicio? OrdenServicio { get; set; }
}
