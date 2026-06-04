using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs.Clientes;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoTallerManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiculosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public VehiculosController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    [HttpGet]
    [Authorize(Roles = "Recepcionista,Mecanico,Admin")]
    public async Task<IActionResult> ListarVehiculos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        // Consultamos todos los vehículos de la base de datos de forma paginada
        var (items, totalCount) = await _unitOfWork.Repository<Vehiculo>().GetAllPagedAsync(pageNumber, pageSize);
        Response.Headers["X-Total-Count"] = totalCount.ToString();
        Response.Headers["Access-Control-Expose-Headers"] = "X-Total-Count";

        // Mapeamos a DTOs para evitar referencias circulares durante la serialización JSON
        var respuesta = items.Adapt<List<VehiculoResponseDto>>();

        return Ok(respuesta);
    }
}

