namespace AutoTallerManager.Domain.Entities;

public class Repuesto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty; // SKU / Código único de barras
    public string Descripcion { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal PrecioUnitario { get; set; }
    public bool Activo { get; set; } = true;

    // Propiedades de navegación virtuales (4FN y Granularidad)
    public virtual ICollection<RepuestoUbicacion> Ubicaciones { get; set; } = new List<RepuestoUbicacion>();
    public virtual ICollection<ProveedorRepuesto> ProveedoresHomologados { get; set; } = new List<ProveedorRepuesto>();
}
