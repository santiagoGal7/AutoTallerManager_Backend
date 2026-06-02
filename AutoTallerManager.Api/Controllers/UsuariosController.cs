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
[Authorize(Roles = "Admin")]
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
    public async Task<IActionResult> Registrar([FromBody] RegistroDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _usuarioService.RegistrarAsync(dto);
            return Created(string.Empty, result);
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

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromServices] ITokenBlocklistService blocklistService)
    {
        // Obtener el identificador único del token actual (jti)
        var jti = User.FindFirst("jti")?.Value 
                  ?? User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrEmpty(jti))
        {
            return BadRequest(new { mensaje = "No se pudo identificar el identificador del token en la petición." });
        }

        // Obtener claim de expiración para calcular el TTL dinámico en caché y optimizar memoria
        var expClaim = User.FindFirst("exp")?.Value 
                      ?? User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        var timeRemaining = TimeSpan.FromMinutes(180); // Fallback si no se puede determinar la expiración
        if (long.TryParse(expClaim, out var expTime))
        {
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expTime);
            var calculatedRemaining = expiryDate - DateTimeOffset.UtcNow;
            if (calculatedRemaining > TimeSpan.Zero)
            {
                timeRemaining = calculatedRemaining;
            }
        }

        await blocklistService.BlockTokenAsync(jti, timeRemaining);

        return Ok(new { mensaje = "Sesión cerrada exitosamente. Token revocado de forma correcta." });
    }

    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var result = await _usuarioService.ObtenerTodosAsync();
        return Ok(result);
    }
}

