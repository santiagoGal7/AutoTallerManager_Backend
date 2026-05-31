namespace AutoTallerManager.Domain.Entities;

public class BahiaServicio
{
    public int Id { get; set; }
    public string NombreBahia { get; set; } = string.Empty; // Ej: 'Bahia Pesados 1'
    public string UbicacionFisica { get; set; } = string.Empty; // Ej: 'Zona Sur'
    public bool EstadoDisponible { get; set; } = true;

    // Relación multivaluada independiente para 4FN
    public virtual ICollection<BahiaMecanicoAsignacion> MecanicosAsignados { get; set; } = new List<BahiaMecanicoAsignacion>();
}
