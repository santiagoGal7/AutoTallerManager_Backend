using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs.Auth;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Registrar([FromBody] CrearUsuarioDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _usuarioService.RegistrarUsuarioAsync(dto);
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

    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var result = await _usuarioService.ObtenerTodosAsync();
        return Ok(result);
    }
}
