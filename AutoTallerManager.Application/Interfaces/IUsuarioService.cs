using System.Collections.Generic;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs.Auth;

namespace AutoTallerManager.Application.Interfaces;

public interface IUsuarioService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    Task<UsuarioResponseDto> RegistrarUsuarioAsync(CrearUsuarioDto dto);
    Task<UsuarioResponseDto> RegistrarAsync(RegistroDto dto);
    Task<IEnumerable<UsuarioResponseDto>> ObtenerTodosAsync();
    Task<UsuarioResponseDto?> ObtenerPorIdAsync(int id);
    Task<bool> DesactivarAsync(int id);
}
