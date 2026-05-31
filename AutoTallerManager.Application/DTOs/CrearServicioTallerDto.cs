using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs;

public class CrearServicioTallerDto
{
    [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre del servicio no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La tarifa base de mano de obra es obligatoria.")]
    [Range(0.00, 99999999.99, ErrorMessage = "La tarifa base de mano de obra debe ser un valor positivo.")]
    public decimal TarifaBaseManoObra { get; set; }

    public bool Activo { get; set; } = true;
}
