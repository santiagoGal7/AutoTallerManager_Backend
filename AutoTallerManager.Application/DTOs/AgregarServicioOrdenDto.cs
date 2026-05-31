using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs;

public class AgregarServicioOrdenDto
{
    [Required(ErrorMessage = "El ID de la orden de servicio es obligatorio.")]
    public int OrdenServicioId { get; set; }

    [Required(ErrorMessage = "El ID del servicio del taller es obligatorio.")]
    public int ServicioTallerId { get; set; }

    [Required(ErrorMessage = "El precio de mano de obra histórico es obligatorio.")]
    [Range(0.01, 99999999.99, ErrorMessage = "El precio de mano de obra histórico debe ser un valor positivo.")]
    public decimal PrecioManoObraHistorico { get; set; }

    [Required(ErrorMessage = "Las horas estimadas son obligatorias.")]
    [Range(1, 1000, ErrorMessage = "Las horas estimadas deben ser al menos 1.")]
    public int HorasEstimadas { get; set; }
}
