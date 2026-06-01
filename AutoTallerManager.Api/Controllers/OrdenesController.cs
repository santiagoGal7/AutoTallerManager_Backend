using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Mecanico,Recepcionista")]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenServicioService _ordenServicioService;

    public OrdenesController(IOrdenServicioService ordenServicioService)
    {
        _ordenServicioService = ordenServicioService ?? throw new ArgumentNullException(nameof(ordenServicioService));
    }

    [HttpPost("abrir")]
    [Authorize(Policy = "RequireRecepcionistaRole")]
    public async Task<IActionResult> AbrirOrden([FromBody] CrearOrdenServicioDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _ordenServicioService.RegistrarOrdenAsync(dto);
        if (resultado.Exitoso)
        {
            return Created(string.Empty, resultado);
        }

        return BadRequest(new { mensaje = resultado.Mensaje });
    }

    [HttpPost("agregar-servicio")]
    [Authorize(Policy = "RequireMecanicoRole")]
    public async Task<IActionResult> AgregarServicio([FromBody] AgregarServicioOrdenDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var exitoso = await _ordenServicioService.AgregarServicioAOrdenAsync(dto);
            if (exitoso)
            {
                return Ok(new { mensaje = "El servicio fue añadido con éxito a la orden de reparación." });
            }
            return BadRequest(new { mensaje = "No se pudo añadir el servicio a la orden de reparación." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor al intentar agregar el servicio a la orden.");
        }
    }

    [HttpPost("agregar-repuesto")]
    [Authorize(Policy = "RequireMecanicoRole")]
    public async Task<IActionResult> AgregarRepuestoAOrden([FromBody] AgregarRepuestoOrdenDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var error = await _ordenServicioService.AgregarRepuestoAOrdenAsync(dto);
        if (error != null)
        {
            return BadRequest(new { mensaje = error });
        }

        return Ok(new { exitoso = true, mensaje = "Repuesto asociado a la orden con éxito." });
    }

    [HttpGet("{id}/totales")]
    [Authorize(Roles = "Mecanico,Recepcionista")]
    public async Task<IActionResult> GetTotalesOrden(int id)
    {
        var resumen = await _ordenServicioService.CalcularTotalesOrdenAsync(id);
        if (resumen == null)
        {
            return NotFound(new { mensaje = "La orden de servicio especificada no existe." });
        }
        return Ok(resumen);
    }

    [HttpPost("facturar")]
    [Authorize(Policy = "RequireRecepcionistaRole")]
    public async Task<IActionResult> FacturarOrden([FromBody] GenerarFacturaDto dto)
    {
        var error = await _ordenServicioService.FacturarYCerrarOrdenAsync(dto);
        if (error != null)
        {
            return BadRequest(new { mensaje = error });
        }
        return Ok(new { exitoso = true, mensaje = "Factura generada con éxito. La orden de servicio ha sido cerrada." });
    }
}

