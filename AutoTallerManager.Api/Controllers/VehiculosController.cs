using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs.Clientes;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}

