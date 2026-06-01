using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RepuestosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RepuestosController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    [HttpPost]
    public async Task<IActionResult> AltaRepuesto([FromBody] CrearRepuestoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var repuesto = dto.Adapt<Repuesto>();
        repuesto.Activo = true;

        var repository = _unitOfWork.Repository<Repuesto>();
        await repository.AddAsync(repuesto);
        await _unitOfWork.CompleteAsync();

        return Created(string.Empty, new { id = repuesto.Id, mensaje = "Repuesto registrado en el catálogo del taller con éxito." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> BajaRepuesto(int id)
    {
        var repository = _unitOfWork.Repository<Repuesto>();
        var repuestos = await repository.GetAllAsync();
        var repuesto = repuestos.FirstOrDefault(r => r.Id == id);

        if (repuesto == null)
        {
            return NotFound(new { mensaje = "El repuesto especificado no existe." });
        }

        // Baja física (o lógica si se prefiere)
        // Usaremos eliminación lógica por seguridad
        repuesto.Activo = false;
        await _unitOfWork.CompleteAsync();

        return Ok(new { mensaje = "Repuesto dado de baja lógicamente con éxito." });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Mecanico,Recepcionista")]
    public async Task<IActionResult> ListarRepuestos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _unitOfWork.Repository<Repuesto>().GetAllPagedAsync(pageNumber, pageSize);
        Response.Headers["X-Total-Count"] = totalCount.ToString();
        Response.Headers["Access-Control-Expose-Headers"] = "X-Total-Count";
        return Ok(items);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Mecanico,Recepcionista")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var repository = _unitOfWork.Repository<Repuesto>();
        var repuestos = await repository.GetAllAsync();
        var repuesto = repuestos.FirstOrDefault(r => r.Id == id);

        if (repuesto == null)
        {
            return NotFound(new { mensaje = "El repuesto especificado no existe." });
        }

        return Ok(repuesto);
    }
}
