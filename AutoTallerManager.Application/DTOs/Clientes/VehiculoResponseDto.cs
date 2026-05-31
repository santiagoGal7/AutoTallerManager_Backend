namespace AutoTallerManager.Application.DTOs.Clientes;

public class VehiculoResponseDto
{
    public int Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string VIN { get; set; } = string.Empty;
    public int Kilometraje { get; set; }
}
