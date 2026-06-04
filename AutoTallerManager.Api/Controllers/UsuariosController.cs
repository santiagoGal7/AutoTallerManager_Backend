using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs.Auth;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _usuarioService.LoginAsync(dto);
        if (result == null)
        {
            return Unauthorized(new { mensaje = "El correo o la contraseña son incorrectos, o la cuenta está inactiva." });
        }

        return Ok(result);
    }

    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<ActionResult<UsuarioResponseDto>> Registrar([FromBody] RegistroDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Roles privilegiados solo los puede crear un Admin autenticado
        var rolesPrivilegiados = new[] { "Admin", "Mecanico", "Recepcionista" };
        var rolSolicitado = dto.Rol?.Trim() ?? string.Empty;

        if (rolesPrivilegiados.Contains(rolSolicitado, StringComparer.OrdinalIgnoreCase))
        {
            if (!(User.Identity?.IsAuthenticated ?? false) || !User.IsInRole("Admin"))
            {
                return Unauthorized(new { mensaje = "Solo un administrador autenticado puede crear usuarios con roles privilegiados (Admin, Mecanico, Recepcionista)." });
            }
        }

        try
        {
            var result = await _usuarioService.RegistrarAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno al intentar registrar al usuario.");
        }
    }



    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var result = await _usuarioService.ObtenerTodosAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioResponseDto>> ObtenerPorId(int id)
    {
        // Blindaje IDOR: Solo un usuario con rol Admin puede ver a cualquiera.
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId) || usuarioId != id)
            {
                return Forbid();
            }
        }

        var usuario = await _usuarioService.ObtenerPorIdAsync(id);
        if (usuario == null)
        {
            return NotFound(new { mensaje = "El usuario especificado no existe." });
        }

        return Ok(usuario);
    }

    [HttpPatch("{id}/desactivar")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var callerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(callerIdClaim, out var callerId) && callerId == id)
            return BadRequest(new { mensaje = "Un administrador no puede desactivar su propia cuenta." });

        var exitoso = await _usuarioService.DesactivarAsync(id);
        if (!exitoso)
            return NotFound(new { mensaje = "El usuario especificado no existe." });

        return Ok(new { exitoso = true, mensaje = $"Usuario id={id} desactivado correctamente." });
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<IActionResult> ObtenerPerfil()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
        {
            return Unauthorized(new { mensaje = "El token de usuario no contiene un identificador válido." });
        }

        var usuario = await _usuarioService.ObtenerPorIdAsync(usuarioId);
        if (usuario == null)
        {
            return NotFound(new { mensaje = "El usuario asociado a este token no existe en el sistema." });
        }

        return Ok(usuario);
    }
}

