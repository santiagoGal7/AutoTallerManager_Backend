using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Application.DTOs.Clientes;
using Mapster;

namespace AutoTallerManager.Application.Configuration;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Vehiculo, VehiculoResponseDto>.NewConfig()
            .Map(dest => dest.Kilometraje, src => src.HistorialesKilometraje != null && src.HistorialesKilometraje.Any()
                ? src.HistorialesKilometraje.OrderByDescending(h => h.FechaLectura).Select(h => h.Kilometraje).FirstOrDefault()
                : 0);

        // Aseguramos que la colección se proyecte correctamente
        TypeAdapterConfig<Cliente, ClienteResponseDto>.NewConfig()
            .Map(dest => dest.Vehiculos, src => src.Vehiculos);
    }
}
