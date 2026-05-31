namespace AutoTallerManager.Domain.Entities;

public class HistorialKilometraje
{
    public int Id { get; set; }
    public int VehiculoId { get; set; }
    public int Kilometraje { get; set; }
    public DateTime FechaLectura { get; set; }
    public string OrigenLectura { get; set; } = string.Empty; // Ej: 'Ingreso Taller', 'Revision Preventiva'

    // Relación con el vehículo
    public virtual Vehiculo? Vehiculo { get; set; }
}
