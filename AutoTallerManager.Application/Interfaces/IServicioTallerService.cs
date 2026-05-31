using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;

namespace AutoTallerManager.Application.Interfaces;

public interface IServicioTallerService
{
    Task<int> RegistrarServicioAsync(CrearServicioTallerDto dto);
}
