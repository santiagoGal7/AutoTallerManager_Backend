using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Application.Exceptions;

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
        // 1. Validación defensiva contra IDOR: Confirmar que el vehículo realmente le pertenece al cliente
        var vehiculoValido = await _unitOfWork.Repository<Vehiculo>()
            .AnyAsync(v => v.Id == dto.VehiculoId && v.IdCliente == dto.ClienteId);

        if (!vehiculoValido)
        {
            throw new BusinessException("El vehículo no existe o no pertenece al cliente especificado.");
        }

        // 2. Instanciar la entidad OrdenServicio usando el constructor de negocio
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
        // 1. Obtener la orden de servicio
        var orden = _unitOfWork.Repository<OrdenServicio>()
            .Find(o => o.Id == dto.OrdenServicioId)
            .FirstOrDefault();

        if (orden == null)
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

        // Cargar detalles existentes en el DbContext para el relationship fix-up automático
        _unitOfWork.Repository<DetalleOrdenServicio>()
            .Find(d => d.IdOrdenServicio == orden.Id)
            .ToList();

        _unitOfWork.Repository<DetalleOrdenRepuesto>()
            .Find(r => r.OrdenServicioId == orden.Id)
            .ToList();

        // 3. Crear el detalle y delegar agregación y cálculo financiero al Dominio
        var detalle = new DetalleOrdenServicio
        {
            IdOrdenServicio = dto.OrdenServicioId,
            IdServicioTaller = dto.ServicioTallerId,
            PrecioManoObraHistorico = dto.PrecioManoObraHistorico,
            HorasEstimadas = dto.HorasEstimadas
        };

        orden.AgregarDetalle(detalle);
        orden.CalcularTotales(0.19m); // IVA estándar del 19%

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
            // 1. Obtener la orden de servicio
            var orden = _unitOfWork.Repository<OrdenServicio>()
                .Find(o => o.Id == dto.OrdenServicioId)
                .FirstOrDefault();

            if (orden == null)
            {
                throw new InvalidOperationException("La orden de servicio especificada no existe.");
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

            // Cargar detalles existentes en el DbContext para el relationship fix-up automático
            _unitOfWork.Repository<DetalleOrdenServicio>()
                .Find(d => d.IdOrdenServicio == orden.Id)
                .ToList();

            _unitOfWork.Repository<DetalleOrdenRepuesto>()
                .Find(r => r.OrdenServicioId == orden.Id)
                .ToList();

            // 5. Instanciar el detalle y delegar agregación y cálculo al Dominio
            var detalleRepuesto = new DetalleOrdenRepuesto
            {
                OrdenServicioId = dto.OrdenServicioId,
                RepuestoId = dto.RepuestoId,
                Cantidad = dto.Cantidad,
                PrecioVentaHistorico = dto.PrecioVentaHistorico
            };

            orden.AgregarDetalle(detalleRepuesto);
            orden.CalcularTotales(0.19m); // IVA estándar del 19%

            // Guardar cambios dentro de la transacción activa
            await _unitOfWork.CompleteAsync();
            
            // Confirmar transacción
            await _unitOfWork.CommitTransactionAsync();

            return null;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<ResumenTotalesOrdenDto?> CalcularTotalesOrdenAsync(int ordenId)
    {
        var orden = _unitOfWork.Repository<OrdenServicio>()
            .Find(o => o.Id == ordenId)
            .FirstOrDefault();

        if (orden == null)
        {
            return null;
        }

        // Cargar detalles existentes en el DbContext para el relationship fix-up automático
        _unitOfWork.Repository<DetalleOrdenServicio>()
            .Find(d => d.IdOrdenServicio == orden.Id)
            .ToList();

        _unitOfWork.Repository<DetalleOrdenRepuesto>()
            .Find(r => r.OrdenServicioId == orden.Id)
            .ToList();

        orden.CalcularTotales(0.19m);

        var subtotalManoObra = orden.DetallesServicio.Sum(d => d.PrecioManoObraHistorico * d.HorasEstimadas);
        var subtotalRepuestos = orden.DetallesRepuesto.Sum(r => r.PrecioVentaHistorico * r.Cantidad);

        return new ResumenTotalesOrdenDto
        {
            OrdenServicioId = ordenId,
            SubtotalManoObra = subtotalManoObra,
            SubtotalRepuestos = subtotalRepuestos,
            Impuestos = orden.Impuestos,
            TotalNeto = orden.Total
        };
    }

    public async Task<string?> FacturarYCerrarOrdenAsync(GenerarFacturaDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Obtener la orden de servicio
            var orden = _unitOfWork.Repository<OrdenServicio>()
                .Find(o => o.Id == dto.OrdenServicioId)
                .FirstOrDefault();

            if (orden == null || orden.Estado == "Finalizada")
            {
                await _unitOfWork.RollbackTransactionAsync();
                return "La orden no existe o ya ha sido facturada y cerrada.";
            }

            // Cargar detalles existentes en el DbContext para el relationship fix-up automático
            _unitOfWork.Repository<DetalleOrdenServicio>()
                .Find(d => d.IdOrdenServicio == orden.Id)
                .ToList();

            _unitOfWork.Repository<DetalleOrdenRepuesto>()
                .Find(r => r.OrdenServicioId == orden.Id)
                .ToList();

            // 2. Ejecutar las reglas de negocio usando los métodos encapsulados del dominio
            orden.CalcularTotales(0.19m);
            orden.FinalizarOrden();

            // 3. Crear y guardar la factura en el contexto
            var subtotalManoObra = orden.DetallesServicio.Sum(d => d.PrecioManoObraHistorico * d.HorasEstimadas);
            var subtotalRepuestos = orden.DetallesRepuesto.Sum(r => r.PrecioVentaHistorico * r.Cantidad);

            var factura = new Factura
            {
                OrdenServicioId = dto.OrdenServicioId,
                NumeroFactura = "FAC-" + Guid.NewGuid().ToString()[..8].ToUpper(),
                SubtotalManoObra = subtotalManoObra,
                SubtotalRepuestos = subtotalRepuestos,
                TotalImpuestos = orden.Impuestos,
                TotalNeto = orden.Total,
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
