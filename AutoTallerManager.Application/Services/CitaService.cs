using System;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Application.Exceptions;

namespace AutoTallerManager.Application.Services;

public class CitaService : ICitaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CitaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> ProgramarCitaAsync(ProgramarCitaDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Validar que el cliente exista
            var cliente = await _unitOfWork.Repository<Cliente>().GetByIntIdAsync(dto.ClienteId);
            if (cliente == null)
            {
                throw new InvalidOperationException("El cliente especificado no existe.");
            }

            // Validar que el vehículo exista y pertenezca al cliente
            var vehiculo = await _unitOfWork.Repository<Vehiculo>().GetByIntIdAsync(dto.VehiculoId);
            if (vehiculo == null || vehiculo.IdCliente != dto.ClienteId)
            {
                throw new InvalidOperationException("El vehículo no existe o no pertenece al cliente especificado.");
            }

            // Validar que el servicio de catálogo exista
            var servicio = await _unitOfWork.Repository<ServicioTaller>().GetByIntIdAsync(dto.ServicioTallerId);
            if (servicio == null || !servicio.Activo)
            {
                throw new InvalidOperationException("El servicio taller seleccionado no existe o no está activo.");
            }

            var cita = new CitaTaller
            {
                ClienteId = dto.ClienteId,
                VehiculoId = dto.VehiculoId,
                ServicioTallerId = dto.ServicioTallerId,
                FechaHoraReserva = dto.FechaHoraReserva,
                EstadoCita = "Programada",
                NotasSintomas = dto.NotasSintomas
            };

            await _unitOfWork.Repository<CitaTaller>().AddAsync(cita);
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

    public async Task<bool> ConfirmarAsistenciaAsync(int citaId)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var repository = _unitOfWork.Repository<CitaTaller>();
            var cita = await repository.GetByIntIdAsync(citaId);
            if (cita == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }

            cita.EstadoCita = "Asistió";
            repository.Update(cita);

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

    public async Task<int> ConvertirCitaEnOrdenAsync(int citaId)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var repository = _unitOfWork.Repository<CitaTaller>();
            var cita = await repository.GetByIntIdAsync(citaId);
            if (cita == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException("La cita especificada no existe.");
            }

            if (cita.EstadoCita == "Cancelada" || cita.EstadoCita == "No Asistió")
            {
                throw new InvalidOperationException($"No se puede convertir una cita que se encuentra en estado '{cita.EstadoCita}'.");
            }

            // 1. Instanciar la nueva orden de servicio
            var orden = new OrdenServicio(cita.VehiculoId, "SERVICIO AGENDADO: " + (string.IsNullOrEmpty(cita.NotasSintomas) ? "Sin notas" : cita.NotasSintomas), 0.0m);
            await _unitOfWork.Repository<OrdenServicio>().AddAsync(orden);
            await _unitOfWork.SaveChangesAsync(); // Generar ID de la orden

            // 2. Obtener el servicio del catálogo y crear el detalle
            var servicio = await _unitOfWork.Repository<ServicioTaller>().GetByIntIdAsync(cita.ServicioTallerId);
            if (servicio != null && servicio.Activo)
            {
                var detalle = new DetalleOrdenServicio
                {
                    IdOrdenServicio = orden.Id,
                    IdServicioTaller = servicio.Id,
                    PrecioManoObraHistorico = servicio.TarifaBaseManoObra,
                    HorasEstimadas = 1
                };
                orden.AgregarDetalle(detalle);
                orden.CalcularTotales(0.19m); // Calcular el total inicial
            }

            // 3. Confirmar la asistencia de la cita
            cita.EstadoCita = "Asistió";
            repository.Update(cita);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return orden.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
