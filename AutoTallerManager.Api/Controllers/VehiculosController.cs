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

    [HttpGet("listar-vehiculos")]
    [Authorize(Roles = "Recepcionista,Mecanico,Admin")]
    public async Task<IActionResult> ListarVehiculos()
    {
        // Consultamos todos los vehículos de la base de datos
        var vehiculos = await _unitOfWork.Repository<Vehiculo>().GetAllAsync();

        // Mapeamos a DTOs para evitar referencias circulares durante la serialización JSON
        var respuesta = vehiculos.Adapt<List<VehiculoResponseDto>>();

        return Ok(respuesta);
    }
}

