namespace AutoTallerManager.Domain.Entities;

public class HerramientaMantenimiento
{
    public int Id { get; set; }
    public int HerramientaId { get; set; }
    public DateTime FechaMantenimiento { get; set; }
    public string TipoMantenimiento { get; set; } = string.Empty; // 'Preventivo', 'Correctivo', 'Calibracion'
    public string DescripcionTrabajo { get; set; } = string.Empty;
    public decimal CostoMantenimiento { get; set; }

    // Relación con la herramienta base
    public virtual Herramienta? Herramienta { get; set; }
}
