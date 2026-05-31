namespace AutoTallerManager.Domain.Entities;

public class MedioPago
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // Ej: 'Efectivo', 'Tarjeta Credito', 'Transferencia Bancaria'
    public bool PermiteCuotas { get; set; }
    public bool Activo { get; set; } = true;

    public virtual ICollection<FacturaPago> PagosAsociados { get; set; } = new List<FacturaPago>();
}
