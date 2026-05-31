namespace AutoTallerManager.Domain.Entities;

public class OrdenEstadoHistorial
{
    public int Id { get; set; }
    public int OrdenServicioId { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; } // Quién autorizó el cambio de estado (FK a Usuario)

    public virtual OrdenServicio? OrdenServicio { get; set; }
    public virtual Usuario? Usuario { get; set; }
}
