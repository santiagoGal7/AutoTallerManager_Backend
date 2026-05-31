namespace AutoTallerManager.Domain.Entities;

public class UsuarioControlAcceso
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;
    public string TipoEvento { get; set; } = string.Empty; // 'Login_Exitoso', 'Intento_Fallido', 'Logout'
    public string DireccionIP { get; set; } = string.Empty;
    public string DispositivoNavegador { get; set; } = string.Empty;

    public virtual Usuario? Usuario { get; set; }
}
