using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Recepcionista,Mecanico")]
public class GarantiasController : ControllerBase
{
    private readonly IGarantiaService _garantiaService;

    public GarantiasController(IGarantiaService garantiaService)
    {
        _garantiaService = garantiaService ?? throw new ArgumentNullException(nameof(garantiaService));
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarReclamoGarantia([FromBody] ReclamoGarantiaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var nuevaOrdenId = await _garantiaService.RegistrarReclamoGarantiaAsync(dto);
            return Ok(new 
            { 
                exitoso = true, 
                ordenGarantiaId = nuevaOrdenId, 
                mensaje = "Reclamo de garantía registrado con éxito. Se ha creado una nueva orden de servicio asociada." 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor al intentar registrar el reclamo de garantía.");
        }
    }
}
