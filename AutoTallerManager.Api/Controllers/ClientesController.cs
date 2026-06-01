using AutoTallerManager.Application.DTOs.Clientes;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly IUnitOfWork _unitOfWork;

    public ClientesController(IClienteService clienteService, IUnitOfWork unitOfWork)
    {
        _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    [HttpPost("registrar-con-vehiculo")]
    [Authorize(Roles = "Recepcionista,Admin")]
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

    [HttpGet("listar-vehiculos")]
    [Authorize(Roles = "Recepcionista,Mecanico,Admin")]
    public async Task<IActionResult> ListarVehiculos()
    {
        // Usar el repositorio para traer los carros reales de Supabase
        var vehiculos = await _unitOfWork.Repository<Vehiculo>().GetAllAsync();
        
        // Mapear para evitar problemas de referencias circulares JSON
        var respuesta = vehiculos.Adapt<List<VehiculoResponseDto>>();
        
        return Ok(respuesta);
    }
}

