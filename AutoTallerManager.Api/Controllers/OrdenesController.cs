using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Services;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Mecanico,Recepcionista,Cliente")]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenServicioService _ordenServicioService;
    private readonly IUnitOfWork _unitOfWork;

    public OrdenesController(IOrdenServicioService ordenServicioService, IUnitOfWork unitOfWork)
    {
        _ordenServicioService = ordenServicioService ?? throw new ArgumentNullException(nameof(ordenServicioService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    [HttpPost("abrir")]
    [Authorize(Roles = "Admin,Recepcionista")]
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
    [Authorize(Roles = "Admin,Mecanico")]
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
    [Authorize(Roles = "Admin,Mecanico")]
    public async Task<IActionResult> AgregarRepuestoAOrden([FromBody] AgregarRepuestoOrdenDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var error = await _ordenServicioService.AgregarRepuestoAOrdenAsync(dto);
            if (error != null)
            {
                return BadRequest(new { mensaje = error });
            }

            return Ok(new { exitoso = true, mensaje = "Repuesto asociado a la orden con éxito." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{id}/totales")]
    [Authorize(Roles = "Admin,Mecanico,Recepcionista")]
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
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<IActionResult> FacturarOrden([FromBody] GenerarFacturaDto dto)
    {
        var error = await _ordenServicioService.FacturarYCerrarOrdenAsync(dto);
        if (error != null)
        {
            return BadRequest(new { mensaje = error });
        }
        return Ok(new { exitoso = true, mensaje = "Factura generada con éxito. La orden de servicio ha sido cerrada." });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Mecanico,Recepcionista")]
    public async Task<IActionResult> ListarOrdenes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _unitOfWork.Repository<OrdenServicio>().GetAllPagedAsync(pageNumber, pageSize);
        Response.Headers["X-Total-Count"] = totalCount.ToString();
        Response.Headers["Access-Control-Expose-Headers"] = "X-Total-Count";
        return Ok(items);
    }

    [HttpGet("mis-ordenes")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetMisOrdenes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
        {
            return Unauthorized(new { mensaje = "Usuario no autenticado." });
        }

        // 1. Obtener el usuario autenticado (búsqueda directa con puerto sobrecargado entero)
        var usuarioRepository = _unitOfWork.Repository<Usuario>();
        var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado." });
        }

        // 2. Obtener el cliente por correo (directo en BD)
        var clienteRepository = _unitOfWork.Repository<Cliente>();
        var cliente = clienteRepository.Find(c => c.Correo.ToLower() == usuario.Correo.ToLower()).FirstOrDefault();
        if (cliente == null)
        {
            return NotFound(new { mensaje = "No se encontró registro de cliente asociado a su cuenta de usuario." });
        }

        // 3. Obtener vehículos del cliente (directo en BD)
        var vehiculoRepository = _unitOfWork.Repository<Vehiculo>();
        var vehiculosDelCliente = vehiculoRepository.Find(v => v.IdCliente == cliente.Id).Select(v => v.Id).ToList();

        // 4. Obtener las órdenes de servicio correspondientes y paginar de forma transparente en BD
        var ordenesRepository = _unitOfWork.Repository<OrdenServicio>();
        var misOrdenesQuery = ordenesRepository.Find(o => vehiculosDelCliente.Contains(o.VehiculoId)).AsQueryable();

        var totalCount = misOrdenesQuery.Count();
        var actualPage = pageNumber < 1 ? 1 : pageNumber;
        var actualSize = pageSize < 1 ? 10 : pageSize;

        var items = misOrdenesQuery
            .Skip((actualPage - 1) * actualSize)
            .Take(actualSize)
            .ToList();

        Response.Headers["X-Total-Count"] = totalCount.ToString();
        Response.Headers["Access-Control-Expose-Headers"] = "X-Total-Count";

        return Ok(items);
    }
}
