using AutoTallerManager.Application.DTOs.Clientes;

namespace AutoTallerManager.Application.Services;

public interface IClienteService
{
    Task<ClienteResponseDto> RegistrarClienteConVehiculoAsync(CrearClienteDto dto);
}
