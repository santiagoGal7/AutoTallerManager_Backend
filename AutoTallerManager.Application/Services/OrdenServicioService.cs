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

        // 2. Si existe, instanciar la entidad OrdenServicio usando el constructor de negocio
        var orden = new OrdenServicio(dto.VehiculoId, dto.DescripcionProblema, dto.CostoEstimado);

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
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Valida si la Orden de Servicio existe
            var orden = await _unitOfWork.Repository<OrdenServicio>().GetByIntIdAsync(dto.OrdenServicioId);
            if (orden == null)
            {
                throw new InvalidOperationException("La orden de servicio especificada no existe.");
            }

            if (orden.Estado == "Finalizada")
            {
                throw new InvalidOperationException("No se pueden añadir repuestos a una orden de servicio finalizada y cerrada.");
            }

            // 2. Valida si el Repuesto existe en la tabla de inventario
            var repuestoRepository = _unitOfWork.Repository<Repuesto>();
            var repuesto = await repuestoRepository.GetByIntIdAsync(dto.RepuestoId);
            if (repuesto == null)
            {
                throw new InvalidOperationException("El repuesto seleccionado no existe en el inventario.");
            }

            if (!repuesto.Activo)
            {
                throw new InvalidOperationException("El repuesto seleccionado no está activo en el catálogo del taller.");
            }

            // 3. Validación estricta de stock
            if (repuesto.Stock < dto.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente. Cantidad disponible en inventario: {repuesto.Stock}. Cantidad solicitada: {dto.Cantidad}.");
            }

            // 4. Decremento de stock
            repuesto.Stock -= dto.Cantidad;
            repuestoRepository.Update(repuesto);

            // 5. Instanciar y agregar detalle a la orden
            var detalleRepuesto = new DetalleOrdenRepuesto
            {
                OrdenServicioId = dto.OrdenServicioId,
                RepuestoId = dto.RepuestoId,
                Cantidad = dto.Cantidad,
                PrecioVentaHistorico = dto.PrecioVentaHistorico
            };

            await _unitOfWork.Repository<DetalleOrdenRepuesto>().AddAsync(detalleRepuesto);
            
            // Guardar cambios dentro de la transacción activa
            await _unitOfWork.CompleteAsync();
            
            // Confirmar transacción
            await _unitOfWork.CommitTransactionAsync();

            return null; // Operación exitosa sin errores heredados
        }
        catch
        {
            // Desencadenar el rollback automático de la transacción
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<ResumenTotalesOrdenDto?> CalcularTotalesOrdenAsync(int ordenId)
    {
        var existeOrden = await _unitOfWork.Repository<OrdenServicio>()
            .AnyAsync(o => o.Id == ordenId);

        if (!existeOrden)
        {
            return null;
        }

        var subtotalManoObra = await _unitOfWork.Repository<DetalleOrdenServicio>()
            .SumAsync(d => d.IdOrdenServicio == ordenId, d => d.PrecioManoObraHistorico * d.HorasEstimadas);

        var subtotalRepuestos = await _unitOfWork.Repository<DetalleOrdenRepuesto>()
            .SumAsync(r => r.OrdenServicioId == ordenId, r => r.PrecioVentaHistorico * r.Cantidad);

        var subtotalGeneral = subtotalManoObra + subtotalRepuestos;
        var impuestos = subtotalGeneral * 0.19m;
        var totalNeto = subtotalGeneral + impuestos;

        return new ResumenTotalesOrdenDto
        {
            OrdenServicioId = ordenId,
            SubtotalManoObra = subtotalManoObra,
            SubtotalRepuestos = subtotalRepuestos,
            Impuestos = impuestos,
            TotalNeto = totalNeto
        };
    }

    public async Task<string?> FacturarYCerrarOrdenAsync(GenerarFacturaDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Obtener la orden usando la potencia de IQueryable
            var orden = _unitOfWork.Repository<OrdenServicio>()
                .Find(o => o.Id == dto.OrdenServicioId)
                .FirstOrDefault();

            if (orden == null || orden.Estado == "Finalizada")
            {
                await _unitOfWork.RollbackTransactionAsync();
                return "La orden no existe o ya ha sido facturada y cerrada.";
            }

            // 2. Ejecutar las reglas de negocio usando los métodos encapsulados del dominio
            var subtotalManoObra = await _unitOfWork.Repository<DetalleOrdenServicio>()
                .SumAsync(d => d.IdOrdenServicio == dto.OrdenServicioId, d => d.PrecioManoObraHistorico * d.HorasEstimadas);

            var subtotalRepuestos = await _unitOfWork.Repository<DetalleOrdenRepuesto>()
                .SumAsync(r => r.OrdenServicioId == dto.OrdenServicioId, r => r.PrecioVentaHistorico * r.Cantidad);

            subtotalManoObra = subtotalManoObra < 0 ? 0.0m : subtotalManoObra;
            subtotalRepuestos = subtotalRepuestos < 0 ? 0.0m : subtotalRepuestos;

            orden.CalcularTotal(subtotalManoObra, subtotalRepuestos);
            orden.FinalizarOrden();

            // 3. Crear y guardar la factura en el contexto
            var subtotalGeneral = subtotalManoObra + subtotalRepuestos;
            var impuestos = subtotalGeneral * 0.19m;
            var totalNeto = subtotalGeneral + impuestos;

            var factura = new Factura
            {
                OrdenServicioId = dto.OrdenServicioId,
                NumeroFactura = "FAC-" + Guid.NewGuid().ToString()[..8].ToUpper(),
                SubtotalManoObra = subtotalManoObra,
                SubtotalRepuestos = subtotalRepuestos,
                TotalImpuestos = impuestos,
                TotalNeto = totalNeto,
                FechaEmision = DateTime.UtcNow,
                EstadoPago = "Pendiente"
            };

            await _unitOfWork.Repository<Factura>().AddAsync(factura);

            // 4. Guardar cambios en el contexto
            await _unitOfWork.CompleteAsync();

            // 5. Confirmar transacción de forma única y limpia
            await _unitOfWork.CommitTransactionAsync();

            return null;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
