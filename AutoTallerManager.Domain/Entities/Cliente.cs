namespace AutoTallerManager.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Vínculo con la tabla de Usuarios (Atomicidad de registro)
    public int? UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; }

    // Propiedad de navegación: Un cliente puede tener muchos autos
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
