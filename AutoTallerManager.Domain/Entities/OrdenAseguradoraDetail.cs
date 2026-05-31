namespace AutoTallerManager.Domain.Entities;

public class OrdenAseguradoraDetail
{
    public int Id { get; set; }
    public int OrdenServicioId { get; set; }
    public int AseguradoraId { get; set; }
    public string NumeroSiniestro { get; set; } = string.Empty; // Código de autorización del seguro
    public decimal MontoMaximoAprobado { get; set; }
    public DateTime FechaAutorizacion { get; set; }

    // Relaciones de navegación
    public virtual OrdenServicio? OrdenServicio { get; set; }
    public virtual Aseguradora? Aseguradora { get; set; }
}
