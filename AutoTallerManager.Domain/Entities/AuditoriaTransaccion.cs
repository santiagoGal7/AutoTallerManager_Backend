namespace AutoTallerManager.Domain.Entities;

public class AuditoriaTransaccion
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; } // Quién hizo la acción (FK a Usuario)
    public string EntidadAfectada { get; set; } = string.Empty; // Ej: 'Vehiculo', 'Repuesto'
    public string TipoAccion { get; set; } = string.Empty; // 'INSERT', 'UPDATE', 'DELETE'
    public string DetalleDatos { get; set; } = "{}"; // Almacenará el dump JSONB del estado
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    // Relación con el usuario responsable
    public virtual Usuario? Usuario { get; set; }
}
