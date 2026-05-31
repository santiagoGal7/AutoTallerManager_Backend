namespace AutoTallerManager.Application.DTOs.Clientes;

public class ClienteResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public List<VehiculoResponseDto> Vehiculos { get; set; } = new List<VehiculoResponseDto>();
}
