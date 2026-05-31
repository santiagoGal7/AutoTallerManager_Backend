namespace AutoTallerManager.Domain.Entities;

public class TokenBlocklist
{
    public int Id { get; set; }
    public string TokenHash { get; set; } = string.Empty; // Identificador único del JWT revocado
    public DateTime FechaRevocacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaExpiracionOriginal { get; set; }
}
