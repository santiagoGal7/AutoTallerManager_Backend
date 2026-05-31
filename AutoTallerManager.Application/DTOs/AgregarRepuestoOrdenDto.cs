using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs;

public class AgregarRepuestoOrdenDto
{
    [Required(ErrorMessage = "El ID de la orden de servicio es obligatorio.")]
    public int OrdenServicioId { get; set; }

    [Required(ErrorMessage = "El ID del repuesto es obligatorio.")]
    public int RepuestoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta histórico debe ser un valor positivo.")]
    public decimal PrecioVentaHistorico { get; set; }
}
