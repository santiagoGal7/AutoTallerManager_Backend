namespace AutoTallerManager.Domain.Entities;

public class MecanicoCertificacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string NombreCertificacion { get; set; } = string.Empty; // Ej: 'Certificacion ASE T2'
    public string EnteEmisor { get; set; } = string.Empty; // Ej: 'National Institute for ASE'
    public DateTime FechaExpiracion { get; set; }

    // Relación con el Usuario base
    public virtual Usuario? Usuario { get; set; }
}
