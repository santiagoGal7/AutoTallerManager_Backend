using System;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Services;

public class OrdenServicioService : IOrdenServicioService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrdenServicioService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<OrdenServicioResponseDto> RegistrarOrdenAsync(CrearOrdenServicioDto dto)
    {
        // 1. Verificar si el VehiculoId provisto en el DTO realmente existe en Supabase (PostgreSQL)
        var existeVehiculo = await _unitOfWork.Repository<Vehiculo>()
            .AnyAsync(v => v.Id == dto.VehiculoId);

        if (!existeVehiculo)
        {
            return new OrdenServicioResponseDto
            {
                Exitoso = false,
                Mensaje = "No se puede abrir una orden: El vehículo especificado no existe en el sistema."
            };
        }

        // 2. Si existe, instanciar la entidad OrdenServicio y asignar los valores
        var orden = new OrdenServicio
        {
            VehiculoId = dto.VehiculoId,
            DescripcionProblema = dto.DescripcionProblema,
            CostoEstimado = dto.CostoEstimado,
            Estado = "Iniciada",
            FechaIngreso = DateTime.UtcNow
        };

        // 3. Guardar la entidad a través del repositorio y confirmar la transacción
        await _unitOfWork.Repository<OrdenServicio>().AddAsync(orden);
        await _unitOfWork.CompleteAsync();

        return new OrdenServicioResponseDto
        {
            Exitoso = true,
            Mensaje = "Orden de servicio abierta con éxito.",
            Id = orden.Id
        };
    }

    public async Task<bool> AgregarServicioAOrdenAsync(AgregarServicioOrdenDto dto)
    {
        // 1. Valida si la OrdenServicioId existe. Si no, lanza una excepción de negocio
        var existeOrden = await _unitOfWork.Repository<OrdenServicio>()
            .AnyAsync(o => o.Id == dto.OrdenServicioId);

        if (!existeOrden)
        {
            throw new InvalidOperationException("La orden de servicio especificada no existe.");
        }

        // 2. Valida si el ServicioTallerId existe en el catálogo de servicios. Si no, rebota
        var existeServicio = await _unitOfWork.Repository<ServicioTaller>()
            .AnyAsync(s => s.Id == dto.ServicioTallerId);

        if (!existeServicio)
        {
            throw new InvalidOperationException("El servicio seleccionado no existe en el catálogo del taller.");
        }

        // 3. Si todo es correcto, instancia la entidad DetalleOrdenServicio, guárdala en el repositorio correspondiente y confirma los cambios
        var detalle = new DetalleOrdenServicio
        {
            IdOrdenServicio = dto.OrdenServicioId,
            IdServicioTaller = dto.ServicioTallerId,
            PrecioManoObraHistorico = dto.PrecioManoObraHistorico,
            HorasEstimadas = dto.HorasEstimadas
        };

        await _unitOfWork.Repository<DetalleOrdenServicio>().AddAsync(detalle);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<string?> AgregarRepuestoAOrdenAsync(AgregarRepuestoOrdenDto dto)
    {
        // 1. Valida si la OrdenServicioId existe en el repositorio de OrdenServicio. Si no existe, retorna error
        var existeOrden = await _unitOfWork.Repository<OrdenServicio>()
            .AnyAsync(o => o.Id == dto.OrdenServicioId);

        if (!existeOrden)
        {
            return "La orden de servicio especificada no existe.";
        }

        // 2. Valida si el RepuestoId existe en la tabla Repuestos. Si no existe, retorna error
        var existeRepuesto = await _unitOfWork.Repository<Repuesto>()
            .AnyAsync(r => r.Id == dto.RepuestoId);

        if (!existeRepuesto)
        {
            return "El repuesto seleccionado no existe en el inventario.";
        }

        // 3. Si las validaciones pasan, instancia la entidad de dominio intermedia DetalleOrdenRepuesto
        var detalleRepuesto = new DetalleOrdenRepuesto
        {
            OrdenServicioId = dto.OrdenServicioId,
            RepuestoId = dto.RepuestoId,
            Cantidad = dto.Cantidad,
            PrecioVentaHistorico = dto.PrecioVentaHistorico
        };

        // 4. Añádela a su repositorio correspondiente y confirma la transacción de forma atómica
        await _unitOfWork.Repository<DetalleOrdenRepuesto>().AddAsync(detalleRepuesto);
        await _unitOfWork.CompleteAsync();

        return null; // Indica que no hubo errores
    }
}
