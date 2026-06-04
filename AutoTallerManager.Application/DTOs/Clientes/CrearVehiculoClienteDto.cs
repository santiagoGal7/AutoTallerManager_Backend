using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.Application.DTOs.Clientes;

public class CrearVehiculoClienteDto : CrearVehiculoDto
{
    [Required(ErrorMessage = "El cliente es obligatorio.")]
    public int ClienteId { get; set; }
}
