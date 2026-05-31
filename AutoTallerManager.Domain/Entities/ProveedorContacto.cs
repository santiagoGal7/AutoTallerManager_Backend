namespace AutoTallerManager.Domain.Entities;

public class ProveedorContacto
{
    public int Id { get; set; }
    public int IdProveedor { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    // Propiedad de navegación hacia el maestro de proveedores
    public virtual Proveedor Proveedor { get; set; } = null!;
}
