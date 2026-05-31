namespace AutoTallerManager.Domain.Entities;

public class InsumoTaller
{
    public int Id { get; set; }
    public string NombreInsumo { get; set; } = string.Empty; // Ej: 'Grasa Litio Multipropisito'
    public int StockActual { get; set; }
    public int StockMinimoAlerta { get; set; }
    public string UnidadMedida { get; set; } = string.Empty; // 'Litros', 'Unidades', 'Galones'

    public virtual ICollection<InsumoConsumoHistorial> Consumos { get; set; } = new List<InsumoConsumoHistorial>();
}
