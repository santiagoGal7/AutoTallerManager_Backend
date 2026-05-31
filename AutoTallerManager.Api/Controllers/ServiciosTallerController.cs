using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosTallerController : ControllerBase
{
    private readonly IServicioTallerService _servicioTallerService;

    public ServiciosTallerController(IServicioTallerService servicioTallerService)
    {
        _servicioTallerService = servicioTallerService ?? throw new ArgumentNullException(nameof(servicioTallerService));
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarServicio([FromBody] CrearServicioTallerDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var idGenerado = await _servicioTallerService.RegistrarServicioAsync(dto);
            return Created(string.Empty, new { id = idGenerado, mensaje = "Servicio registrado exitosamente en el catálogo del taller." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error interno en el servidor al registrar el servicio.", detalle = ex.Message });
        }
    }
}
