namespace AutoTallerManager.Domain.Entities;

public class OrdenServicio
{
    public int Id { get; set; }
    public int IdVehiculo { get; set; }
    public int IdMecanico { get; set; }
    
    // Conexión lógica con el CRM: Nulable si el auto entra sin cita previa
    public int? IdCita { get; set; } 
    
    public string TipoServicio { get; set; } = string.Empty; // Mantenimiento, Reparación, Diagnóstico
    public string Estado { get; set; } = string.Empty; // Pendiente, En Proceso, Completada, Cancelada
    public DateTime FechaIngreso { get; set; }
    public DateTime FechaEstimadaEntrega { get; set; }

    // Propiedades de navegación virtuales
    public virtual Vehiculo Vehiculo { get; set; } = null!;
    public virtual Usuario Mecanico { get; set; } = null!;
    public virtual CitaTaller? CitaTaller { get; set; }
    
    public virtual ICollection<DetalleOrdenServicio> DetallesServicio { get; set; } = new List<DetalleOrdenServicio>();
    public virtual ICollection<DetalleOrdenRepuesto> DetallesRepuesto { get; set; } = new List<DetalleOrdenRepuesto>();
    public virtual ICollection<OrdenEstadoHistorial> EstadosHistorial { get; set; } = new List<OrdenEstadoHistorial>();
}
