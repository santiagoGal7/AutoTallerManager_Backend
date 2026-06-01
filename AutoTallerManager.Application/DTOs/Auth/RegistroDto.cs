namespace AutoTallerManager.Application.DTOs.Auth;

public class RegistroDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string Rol { get; set; } = "Admin";
}
