using System;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Application.Exceptions;

namespace AutoTallerManager.Application.Services;

public class FacturaService : IFacturaService
{
    private readonly IUnitOfWork _unitOfWork;

    public FacturaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> RegistrarPagoAsync(RegistrarPagoDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. Obtener la factura
            var facturaRepository = _unitOfWork.Repository<Factura>();
            var factura = await facturaRepository.GetByIntIdAsync(dto.FacturaId);
            if (factura == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }

            if (factura.EstadoPago == "Pagada")
            {
                throw new InvalidOperationException("La factura ya se encuentra completamente pagada.");
            }

            // Validar que el medio de pago exista
            var medioPago = await _unitOfWork.Repository<MedioPago>().GetByIntIdAsync(dto.MedioPagoId);
            if (medioPago == null || !medioPago.Activo)
            {
                throw new InvalidOperationException("El medio de pago seleccionado no existe o no está activo.");
            }

            // 2. Obtener el historial de pagos previos para calcular el saldo restante
            var pagosRepository = _unitOfWork.Repository<FacturaPago>();
            var pagosPrevios = pagosRepository.Find(p => p.FacturaId == factura.Id).ToList();
            var totalPagadoPrevio = pagosPrevios.Sum(p => p.MontoPagado);

            var restante = factura.TotalNeto - totalPagadoPrevio;
            if (dto.MontoPagado > restante)
            {
                throw new InvalidOperationException($"El monto ingresado ({dto.MontoPagado}) supera el saldo pendiente de la factura ({restante}).");
            }

            // 3. Crear el nuevo registro de pago
            var pago = new FacturaPago
            {
                FacturaId = factura.Id,
                MedioPagoId = dto.MedioPagoId,
                MontoPagado = dto.MontoPagado,
                FechaPago = DateTime.UtcNow,
                TransaccionReferencia = dto.TransaccionReferencia.Trim()
            };

            await pagosRepository.AddAsync(pago);

            // 4. Actualizar el estado de la factura
            var totalAcumulado = totalPagadoPrevio + dto.MontoPagado;
            if (totalAcumulado >= factura.TotalNeto)
            {
                factura.EstadoPago = "Pagada";
            }
            else
            {
                factura.EstadoPago = "Parcial";
            }

            facturaRepository.Update(factura);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
