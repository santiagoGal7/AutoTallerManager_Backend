namespace AutoTallerManager.Domain.Entities;

public class HerramientaCategoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // Relación de uno a muchos: Una categoría agrupa muchas herramientas
    public virtual ICollection<Herramienta> Herramientas { get; set; } = new List<Herramienta>();
}
