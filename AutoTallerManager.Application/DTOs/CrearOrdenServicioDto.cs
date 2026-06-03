using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs;

public class CrearOrdenServicioDto
{
    [Required(ErrorMessage = "El ID del vehículo es obligatorio.")]
    public int VehiculoId { get; set; } // Ajustado a VehiculoId para resolver la vinculación JSON con Postman

    [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "La descripción del problema o síntoma es obligatoria.")]
    public string DescripcionProblema { get; set; } = string.Empty;

    [Range(0, 99999999, ErrorMessage = "El costo estimado debe ser un valor positivo.")]
    public decimal CostoEstimado { get; set; }

    public int? CantidadRepuestos { get; set; }
    public DateTime? FechaCita { get; set; }
}
