namespace AutoTallerManager.Domain.Entities;

public class Aseguradora
{
    public int Id { get; set; }
    public string RfcONit { get; set; } = string.Empty; // Identificación fiscal de la aseguradora
    public string RazonSocial { get; set; } = string.Empty;
    public string ContactoEmergencia { get; set; } = string.Empty;
    public string CorreoCorporativo { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;

    // Propiedades de navegación virtuales
    public virtual ICollection<AseguradoraCobertura> CoberturasHomologadas { get; set; } = new List<AseguradoraCobertura>();
    public virtual ICollection<OrdenAseguradoraDetail> OrdenesAmparadas { get; set; } = new List<OrdenAseguradoraDetail>();
}
