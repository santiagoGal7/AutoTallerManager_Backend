using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Services;

public class ServicioTallerService : IServicioTallerService
{
    private readonly IUnitOfWork _unitOfWork;

    public ServicioTallerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<int> RegistrarServicioAsync(CrearServicioTallerDto dto)
    {
        // 1. Mapear DTO a Entidad de Dominio
        var servicio = new ServicioTaller
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            TarifaBaseManoObra = dto.TarifaBaseManoObra,
            Activo = dto.Activo
        };

        // 2. Insertar a través del repositorio genérico
        await _unitOfWork.Repository<ServicioTaller>().AddAsync(servicio);

        // 3. Confirmar persistencia de forma atómica
        await _unitOfWork.CompleteAsync();

        // 4. Retornar el ID generado en la base de datos (Supabase)
        return servicio.Id;
    }
}
