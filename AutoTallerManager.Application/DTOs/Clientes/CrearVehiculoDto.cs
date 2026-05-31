using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs.Clientes;

public class CrearVehiculoDto
{
    [Required(ErrorMessage = "La marca es obligatoria.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es obligatorio.")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año es obligatorio.")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "El número VIN es obligatorio.")]
    [StringLength(17, MinimumLength = 17, ErrorMessage = "El número VIN debe tener exactamente 17 caracteres.")]
    public string VIN { get; set; } = string.Empty;

    [Required(ErrorMessage = "El kilometraje es obligatorio.")]
    public int Kilometraje { get; set; }
}
