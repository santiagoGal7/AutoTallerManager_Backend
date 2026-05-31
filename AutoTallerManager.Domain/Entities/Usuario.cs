namespace AutoTallerManager.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string ContrasenaHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // 'Admin', 'Mecanico', 'Recepcionista'
    public bool Activo { get; set; } = true;

    // Propiedades de navegación para la separación en 4FN
    public virtual ICollection<MecanicoEspecialidad> Especialidades { get; set; } = new List<MecanicoEspecialidad>();
    public virtual ICollection<MecanicoCertificacion> Certificaciones { get; set; } = new List<MecanicoCertificacion>();
}
