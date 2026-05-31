namespace AutoTallerManager.Domain.Entities;

public class Proveedor
{
    public int Id { get; set; }
    public string RucONit { get; set; } = string.Empty; // Identificador fiscal único
    public string RazonSocial { get; set; } = string.Empty;
    public string ContactoNombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    // Relación hacia los repuestos que suministra
    public virtual ICollection<ProveedorRepuesto> RepuestosSuministrados { get; set; } = new List<ProveedorRepuesto>();
}
