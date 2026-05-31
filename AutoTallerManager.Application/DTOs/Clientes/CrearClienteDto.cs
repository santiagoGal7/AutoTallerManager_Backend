namespace AutoTallerManager.Application.DTOs.Clientes;

public class CrearClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public CrearVehiculoDto? VehiculoInicial { get; set; }
}
