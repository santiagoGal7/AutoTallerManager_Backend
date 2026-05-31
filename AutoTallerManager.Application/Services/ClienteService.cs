using AutoTallerManager.Application.DTOs.Clientes;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using Mapster;

namespace AutoTallerManager.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ClienteResponseDto> RegistrarClienteConVehiculoAsync(CrearClienteDto dto)
    {
        // a) Usar el repositorio de Cliente para verificar mediante una expresión si el correo electrónico ya está registrado.
        var existeCorreo = _unitOfWork.Repository<Cliente>()
            .Find(c => c.Correo.ToLower() == dto.Correo.ToLower())
            .Any();

        if (existeCorreo)
        {
            throw new InvalidOperationException("El correo ya se encuentra registrado.");
        }

        // b) Si el correo es único, mapear el DTO a la entidad de dominio 'Cliente'.
        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Telefono = dto.Telefono,
            Correo = dto.Correo,
            FechaRegistro = DateTime.UtcNow
        };

        // c) Si el DTO incluye un vehículo inicial, mapearlo a la entidad 'Vehiculo' y asociarlo al cliente respetando la propiedad de navegación.
        Vehiculo? vehiculo = null;
        if (dto.VehiculoInicial != null)
        {
            vehiculo = new Vehiculo
            {
                Marca = dto.VehiculoInicial.Marca,
                Modelo = dto.VehiculoInicial.Modelo,
                Anio = dto.VehiculoInicial.Anio,
                VIN = dto.VehiculoInicial.VIN,
                Cliente = cliente
            };

            var historialKilometraje = new HistorialKilometraje
            {
                Kilometraje = dto.VehiculoInicial.Kilometraje,
                FechaLectura = DateTime.UtcNow,
                OrigenLectura = "Registro Inicial",
                Vehiculo = vehiculo
            };

            vehiculo.HistorialesKilometraje.Add(historialKilometraje);
            cliente.Vehiculos.Add(vehiculo);
        }

        // d) Invocar 'await _unitOfWork.Repository<Cliente>().AddAsync(cliente)'.
        await _unitOfWork.Repository<Cliente>().AddAsync(cliente);

        // e) Si el vehículo existe, persistirlo de igual forma en su respectivo repositorio.
        if (vehiculo != null)
        {
            await _unitOfWork.Repository<Vehiculo>().AddAsync(vehiculo);
        }

        // f) Confirmar la transacción atómica llamando a 'await _unitOfWork.CompleteAsync()'.
        await _unitOfWork.CompleteAsync();

        // g) Retornar el 'ClienteResponseDto' mapeado mediante Mapster.
        return cliente.Adapt<ClienteResponseDto>();
    }
}
