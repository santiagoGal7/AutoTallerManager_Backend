using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Recepcionista")]
public class PagosController : ControllerBase
{
    private readonly IFacturaService _facturaService;

    public PagosController(IFacturaService facturaService)
    {
        _facturaService = facturaService ?? throw new ArgumentNullException(nameof(facturaService));
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var exitoso = await _facturaService.RegistrarPagoAsync(dto);
            if (!exitoso)
            {
                return NotFound(new { mensaje = "La factura especificada no existe." });
            }

            return Ok(new { exitoso = true, mensaje = "Pago registrado exitosamente en la factura." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor al intentar registrar el pago.");
        }
    }
}
