using System.Collections.Generic;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Interfaces;

public interface IOrdenServicioService
{
    Task<OrdenServicioResponseDto> RegistrarOrdenAsync(CrearOrdenServicioDto dto);
    Task<bool> AgregarServicioAOrdenAsync(AgregarServicioOrdenDto dto);
    Task<string?> AgregarRepuestoAOrdenAsync(AgregarRepuestoOrdenDto dto);
    Task<ResumenTotalesOrdenDto?> CalcularTotalesOrdenAsync(int ordenId);
    Task<string?> FacturarYCerrarOrdenAsync(GenerarFacturaDto dto);
    Task<bool> AsignarMecanicoAsync(int ordenId, int mecanicoId);
    Task<bool> RegistrarDiagnosticoAsync(int ordenId, string diagnostico);
    Task<bool> RegistrarHorasRealesAsync(int detalleId, decimal horasReales);
    Task<(IEnumerable<OrdenServicio> Items, int TotalCount)> GetMisOrdenesPaginadoAsync(int usuarioId, int pageNumber, int pageSize);
}
