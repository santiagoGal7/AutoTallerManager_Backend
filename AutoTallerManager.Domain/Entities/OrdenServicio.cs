using System;

namespace AutoTallerManager.Domain.Entities;

public class OrdenServicio
{
    public int Id { get; set; }
    public int VehiculoId { get; set; } // Ajustado al nombre de la base de datos
    public string Estado { get; set; } = "Iniciada"; // Valores válidos: Iniciada, En Progreso, Finalizada
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public string DescripcionProblema { get; set; } = string.Empty;
    public decimal CostoEstimado { get; set; } = 0.0m;
    public string? DiagnosticoMecanico { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public decimal? CostoTotal { get; set; }
    public int? ServicioTallerId { get; set; }

    // Propiedad de navegación hacia el vehículo
    public Vehiculo Vehiculo { get; set; } = null!;
}
