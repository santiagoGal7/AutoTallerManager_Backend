using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;

    public CitasController(ICitaService citaService)
    {
        _citaService = citaService ?? throw new ArgumentNullException(nameof(citaService));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista,Cliente")]
    public async Task<IActionResult> ProgramarCita([FromBody] ProgramarCitaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var exitoso = await _citaService.ProgramarCitaAsync(dto);
            if (exitoso)
            {
                return Ok(new { exitoso = true, mensaje = "Cita programada exitosamente." });
            }
            return BadRequest(new { mensaje = "No se pudo programar la cita." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor al intentar programar la cita.");
        }
    }

    [HttpPost("{id}/confirmar")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<IActionResult> ConfirmarAsistencia(int id)
    {
        try
        {
            var exitoso = await _citaService.ConfirmarAsistenciaAsync(id);
            if (!exitoso)
            {
                return NotFound(new { mensaje = "La cita especificada no existe." });
            }

            return Ok(new { exitoso = true, mensaje = "Asistencia de la cita confirmada exitosamente." });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor al intentar confirmar la asistencia.");
        }
    }

    [HttpPost("{id}/convertir-orden")]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<IActionResult> ConvertirCitaEnOrden(int id)
    {
        try
        {
            var ordenId = await _citaService.ConvertirCitaEnOrdenAsync(id);
            return Ok(new { exitoso = true, ordenId = ordenId, mensaje = "Cita convertida en orden de servicio exitosamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor al intentar convertir la cita en orden de servicio.");
        }
    }
}
