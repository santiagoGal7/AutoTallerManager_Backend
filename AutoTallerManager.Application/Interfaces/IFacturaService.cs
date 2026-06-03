using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;

namespace AutoTallerManager.Application.Interfaces;

public interface IFacturaService
{
    Task<bool> RegistrarPagoAsync(RegistrarPagoDto dto);
}
