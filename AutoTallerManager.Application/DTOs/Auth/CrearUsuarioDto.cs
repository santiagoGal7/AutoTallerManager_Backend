namespace AutoTallerManager.Application.DTOs.Auth;

public class CrearUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
