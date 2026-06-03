namespace AutoTallerManager.Application.DTOs;

public class RegistrarPagoDto
{
    public int FacturaId { get; set; }
    public int MedioPagoId { get; set; }
    public decimal MontoPagado { get; set; }
    public string TransaccionReferencia { get; set; } = string.Empty;
}
