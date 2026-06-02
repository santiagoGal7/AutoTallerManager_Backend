using AutoTallerManager.Application.DTOs.Clientes;

namespace AutoTallerManager.Application.Interfaces;

public interface IClienteService
{
    Task<ClienteResponseDto> RegistrarClienteConVehiculoAsync(CrearClienteDto dto);
}
