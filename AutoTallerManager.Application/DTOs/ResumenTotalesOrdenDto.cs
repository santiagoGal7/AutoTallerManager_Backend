namespace AutoTallerManager.Application.DTOs;

public class ResumenTotalesOrdenDto
{
    public int OrdenServicioId { get; set; }
    public decimal SubtotalManoObra { get; set; }
    public decimal SubtotalRepuestos { get; set; }
    public decimal SubtotalGeneral => SubtotalManoObra + SubtotalRepuestos;
    public decimal Impuestos { get; set; } // IVA del 19% sobre el subtotal general
    public decimal TotalNeto { get; set; }
}
