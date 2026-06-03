using System;

namespace AutoTallerManager.Application.DTOs;

public class ProgramarCitaDto
{
    public int ClienteId { get; set; }
    public int VehiculoId { get; set; }
    public int ServicioTallerId { get; set; }
    public DateTime FechaHoraReserva { get; set; }
    public string NotasSintomas { get; set; } = string.Empty;
}
