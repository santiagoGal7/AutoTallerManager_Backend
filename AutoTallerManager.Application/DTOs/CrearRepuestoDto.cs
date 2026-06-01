namespace AutoTallerManager.Application.DTOs;

public class CrearRepuestoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal PrecioUnitario { get; set; }
}
