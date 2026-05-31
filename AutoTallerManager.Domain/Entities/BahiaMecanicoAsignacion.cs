namespace AutoTallerManager.Domain.Entities;

public class BahiaMecanicoAsignacion
{
    public int Id { get; set; }
    public int BahiaServicioId { get; set; }
    public int UsuarioMecanicoId { get; set; } // FK a Usuario (Rol Mecánico)
    public DateTime FechaAsignacion { get; set; }

    // Relaciones de navegación
    public virtual BahiaServicio? BahiaServicio { get; set; }
    public virtual Usuario? Mecanico { get; set; }
}
