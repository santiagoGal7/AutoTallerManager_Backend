namespace AutoTallerManager.Domain.Entities;

public class MecanicoEspecialidad
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Especialidad { get; set; } = string.Empty; // Ej: 'Transmisiones', 'Inyeccion'
    public string NivelExperiencia { get; set; } = string.Empty; // Ej: 'Junior', 'Senior'

    // Relación con el Usuario base
    public virtual Usuario? Usuario { get; set; }
}
