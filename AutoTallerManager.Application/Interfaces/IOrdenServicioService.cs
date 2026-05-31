using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;

namespace AutoTallerManager.Application.Interfaces;

public interface IOrdenServicioService
{
    Task<OrdenServicioResponseDto> RegistrarOrdenAsync(CrearOrdenServicioDto dto);
    Task<bool> AgregarServicioAOrdenAsync(AgregarServicioOrdenDto dto);
    Task<string?> AgregarRepuestoAOrdenAsync(AgregarRepuestoOrdenDto dto);
}
