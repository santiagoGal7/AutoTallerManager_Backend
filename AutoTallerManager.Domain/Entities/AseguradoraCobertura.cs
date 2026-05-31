namespace AutoTallerManager.Domain.Entities;

public class AseguradoraCobertura
{
    public int Id { get; set; }
    public int AseguradoraId { get; set; }
    public string CodigoCobertura { get; set; } = string.Empty; // Ej: 'COB-TR-01'
    public string Descripcion { get; set; } = string.Empty;
    public decimal PorcentajeDeducible { get; set; } // Lo que debe pagar el cliente obligatoriamente

    // Relación con la aseguradora madre
    public virtual Aseguradora? Aseguradora { get; set; }
}
