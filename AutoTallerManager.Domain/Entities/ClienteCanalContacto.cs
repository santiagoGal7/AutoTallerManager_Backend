namespace AutoTallerManager.Domain.Entities;

public class ClienteCanalContacto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string TipoCanal { get; set; } = string.Empty; // 'WhatsApp', 'Email', 'SMS'
    public bool PermitirNotificaciones { get; set; } = true;

    // Relación con el Cliente base
    public virtual Cliente? Cliente { get; set; }
}
