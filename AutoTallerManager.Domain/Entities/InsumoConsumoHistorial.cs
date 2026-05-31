namespace AutoTallerManager.Domain.Entities;

public class InsumoConsumoHistorial
{
    public int Id { get; set; }
    public int InsumoTallerId { get; set; }
    public virtual InsumoTaller? InsumoTaller { get; set; }
}
