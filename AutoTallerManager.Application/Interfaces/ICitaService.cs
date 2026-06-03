using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs;

namespace AutoTallerManager.Application.Interfaces;

public interface ICitaService
{
    Task<bool> ProgramarCitaAsync(ProgramarCitaDto dto);
    Task<bool> ConfirmarAsistenciaAsync(int citaId);
    Task<int> ConvertirCitaEnOrdenAsync(int citaId);
}
