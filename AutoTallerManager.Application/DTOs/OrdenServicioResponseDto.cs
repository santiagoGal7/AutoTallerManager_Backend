namespace AutoTallerManager.Application.DTOs;

public class OrdenServicioResponseDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int? Id { get; set; }
}
