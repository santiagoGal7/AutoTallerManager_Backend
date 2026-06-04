using AutoTallerManager.Application.DTOs.Clientes;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Recepcionista,Cliente")]
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
            return CreatedAtAction(nameof(ObtenerClientePorId), new { id = resultado.Id }, resultado);
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

    [HttpGet]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<IActionResult> ListarClientes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = _unitOfWork.Repository<Cliente>()
            .Find(c => true)
            .Include(c => c.Vehiculos)
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var actualPage = pageNumber < 1 ? 1 : pageNumber;
        var actualSize = pageSize < 1 ? 10 : pageSize;

        var items = await query
            .Skip((actualPage - 1) * actualSize)
            .Take(actualSize)
            .ToListAsync();

        Response.Headers["X-Total-Count"] = totalCount.ToString();
        Response.Headers["Access-Control-Expose-Headers"] = "X-Total-Count";
        var respuesta = items.Adapt<List<ClienteResponseDto>>();
        return Ok(respuesta);
    }

    [HttpGet("listar-vehiculos")]
    [Authorize(Roles = "Recepcionista,Mecanico,Admin")]
    public async Task<IActionResult> ListarVehiculos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = _unitOfWork.Repository<Vehiculo>()
            .Find(v => true)
            .Include(v => v.HistorialesKilometraje)
            .AsNoTracking();

        var totalCount = await query.CountAsync();

        var actualPage = pageNumber < 1 ? 1 : pageNumber;
        var actualSize = pageSize < 1 ? 10 : pageSize;

        var items = await query
            .Skip((actualPage - 1) * actualSize)
            .Take(actualSize)
            .ToListAsync();

        Response.Headers["X-Total-Count"] = totalCount.ToString();
        Response.Headers["Access-Control-Expose-Headers"] = "X-Total-Count";

        var respuesta = items.Adapt<List<VehiculoResponseDto>>();
        return Ok(respuesta);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteResponseDto>> ObtenerClientePorId(int id)
    {
        var repository = _unitOfWork.Repository<Cliente>();
        var cliente = await repository
            .Find(c => c.Id == id)
            .Include(c => c.Vehiculos)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (cliente == null)
        {
            return NotFound(new { mensaje = "El cliente especificado no existe." });
        }

        // CONTROL DE IDOR (Insecure Direct Object Reference)
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (userRole == "Cliente" && !string.Equals(cliente.Correo, userEmail, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid(); // Retornar 403 Forbidden si un Cliente intenta acceder a la info de otro cliente
        }

        var respuesta = cliente.Adapt<ClienteResponseDto>();
        return Ok(respuesta);
    }
}


