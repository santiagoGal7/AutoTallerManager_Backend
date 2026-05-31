namespace AutoTallerManager.Domain.Entities;

public class ServicioTaller
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // Ej: 'Cambio de Aceite', 'Alineacion y Balanceo'
    public string Descripcion { get; set; } = string.Empty;
    public decimal TarifaBaseManoObra { get; set; }
    public bool Activo { get; set; } = true;

    // Propiedad de navegación hacia las órdenes
    public virtual ICollection<OrdenServicio> OrdenesServicios { get; set; } = new List<OrdenServicio>();
}
