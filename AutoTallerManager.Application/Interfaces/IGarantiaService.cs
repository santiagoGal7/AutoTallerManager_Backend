using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;

namespace AutoTallerManager.Application.Interfaces;

public interface IGarantiaService
{
    Task<int> RegistrarReclamoGarantiaAsync(ReclamoGarantiaDto dto);
}
