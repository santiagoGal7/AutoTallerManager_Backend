using AutoTallerManager.Application.DTOs.Clientes;
using AutoTallerManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
    }

    [HttpPost("registrar-con-vehiculo")]
    public async Task<ActionResult<ClienteResponseDto>> RegistrarClienteConVehiculo([FromBody] CrearClienteDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var resultado = await _clienteService.RegistrarClienteConVehiculoAsync(dto);
            return Created(string.Empty, resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno en el servidor al procesar el registro.");
        }
    }
}
