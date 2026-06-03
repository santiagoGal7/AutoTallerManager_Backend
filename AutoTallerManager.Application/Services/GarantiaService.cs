using System;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Application.Exceptions;

namespace AutoTallerManager.Application.Services;

public class GarantiaService : IGarantiaService
{
    private readonly IUnitOfWork _unitOfWork;

    public GarantiaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<int> RegistrarReclamoGarantiaAsync(ReclamoGarantiaDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Obtener y verificar la orden original
            var ordenOriginal = _unitOfWork.Repository<OrdenServicio>()
                .Find(o => o.Id == dto.OrdenOriginalId)
                .FirstOrDefault();

            if (ordenOriginal == null)
            {
                throw new InvalidOperationException("La orden de servicio original no existe.");
            }

            // 2. Verificar que la orden original esté Finalizada (Cerrada)
            if (ordenOriginal.Estado != "Finalizada")
            {
                throw new InvalidOperationException($"No se puede aplicar un reclamo de garantía sobre una orden que no está finalizada. Estado actual: '{ordenOriginal.Estado}'.");
            }

            // 3. Crear una nueva orden de servicio asociada al mismo vehículo y sin costo estimado
            var descripcionGarantia = $"RECLAMO GARANTÍA (Ref: {ordenOriginal.Id}): {dto.MotivoFalla}";
            var nuevaOrden = new OrdenServicio(ordenOriginal.VehiculoId, descripcionGarantia, 0.0m);
            
            await _unitOfWork.Repository<OrdenServicio>().AddAsync(nuevaOrden);
            await _unitOfWork.SaveChangesAsync(); // Generar ID de la nueva orden

            // 4. Crear el registro físico de la garantía
            var garantia = new GarantiaServicio
            {
                OrdenServicioActualId = nuevaOrden.Id,
                OrdenServicioOrigenId = ordenOriginal.Id,
                MotivoFalla = dto.MotivoFalla.Trim(),
                ResolucionDictamen = dto.ResolucionDictamen.Trim(),
                FechaReclamo = DateTime.UtcNow
            };

            await _unitOfWork.Repository<GarantiaServicio>().AddAsync(garantia);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return nuevaOrden.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
