using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly ITokenBlocklistService _blocklistService;

    public AuthController(ITokenBlocklistService blocklistService)
    {
        _blocklistService = blocklistService ?? throw new ArgumentNullException(nameof(blocklistService));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Obtener el identificador único del token actual (jti)
        var jti = User.FindFirst("jti")?.Value 
                  ?? User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrEmpty(jti))
        {
            return BadRequest(new { mensaje = "No se pudo identificar el identificador del token en la petición." });
        }

        // Obtener claim de expiración para calcular el TTL dinámico
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

        await _blocklistService.BlockTokenAsync(jti, timeRemaining);

        return Ok(new { mensaje = "Sesión cerrada exitosamente. Token revocado de forma correcta." });
    }
}
