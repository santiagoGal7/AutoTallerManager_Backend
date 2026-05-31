namespace AutoTallerManager.Domain.Entities;

public class CitaTaller
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int VehiculoId { get; set; }
    public int ServicioTallerId { get; set; } // Qué servicio planea realizarse
    public DateTime FechaHoraReserva { get; set; }
    public string EstadoCita { get; set; } = "Programada"; // 'Programada', 'Asistió', 'Cancelada', 'No Asistió'
    public string NotasSintomas { get; set; } = string.Empty; // Lo que reporta el cliente al agendar

    // Relaciones de navegación
    public virtual Cliente? Cliente { get; set; }
    public virtual Vehiculo? Vehiculo { get; set; }
    public virtual ServicioTaller? ServicioTaller { get; set; }
}
