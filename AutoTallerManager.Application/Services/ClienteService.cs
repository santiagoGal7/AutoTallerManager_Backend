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
        var existeCorreo = await _unitOfWork.Repository<Cliente>()
            .AnyAsync(c => c.Correo.ToLower() == dto.Correo.ToLower());

        if (existeCorreo)
        {
            throw new InvalidOperationException("El correo ya se encuentra registrado.");
        }

        // b) Si el correo es único, mapear el DTO a la entidad de dominio 'Cliente'.
        var nuevoCliente = new Cliente
        {
            Nombre = dto.Nombre,
            Telefono = dto.Telefono,
            Correo = dto.Correo,
            FechaRegistro = DateTime.UtcNow,
            Vehiculos = new List<Vehiculo>() // Inicializar la lista limpia
        };

        // c) Mapear manualmente los vehículos del DTO a la entidad de dominio (soporta tanto DTO único como colección)
        if (dto.VehiculoInicial != null)
        {
            var vehiculo = new Vehiculo
            {
                Marca = dto.VehiculoInicial.Marca,
                Modelo = dto.VehiculoInicial.Modelo,
                Anio = dto.VehiculoInicial.Anio,
                VIN = dto.VehiculoInicial.VIN,
                Cliente = nuevoCliente, // Relación al padre explícita
                HistorialesKilometraje = new List<HistorialKilometraje>()
            };

            var historial = new HistorialKilometraje
            {
                Kilometraje = dto.VehiculoInicial.Kilometraje,
                FechaLectura = DateTime.UtcNow,
                OrigenLectura = "Registro Inicial",
                Vehiculo = vehiculo
            };
            vehiculo.HistorialesKilometraje.Add(historial);

            nuevoCliente.Vehiculos.Add(vehiculo);
        }

        if (dto.Vehiculos != null && dto.Vehiculos.Any())
        {
            foreach (var vDto in dto.Vehiculos)
            {
                var vehiculo = new Vehiculo
                {
                    Marca = vDto.Marca,
                    Modelo = vDto.Modelo,
                    Anio = vDto.Anio,
                    VIN = vDto.VIN,
                    Cliente = nuevoCliente, // Relación al padre explícita
                    HistorialesKilometraje = new List<HistorialKilometraje>()
                };

                var historial = new HistorialKilometraje
                {
                    Kilometraje = vDto.Kilometraje,
                    FechaLectura = DateTime.UtcNow,
                    OrigenLectura = "Registro Inicial",
                    Vehiculo = vehiculo
                };
                vehiculo.HistorialesKilometraje.Add(historial);

                nuevoCliente.Vehiculos.Add(vehiculo);
            }
        }

        // d) Guardar la entidad raíz (guardará automáticamente toda la cascada en la misma transacción)
        await _unitOfWork.Repository<Cliente>().AddAsync(nuevoCliente);
        
        // AUTOMÁTICAMENTE CREAR VÍNCULO CON LA TABLA DE USUARIOS PARA ROL "Cliente"
        var usuarioRepository = _unitOfWork.Repository<Usuario>();
        var existeUsuario = await usuarioRepository.AnyAsync(u => u.Correo.ToLower() == dto.Correo.ToLower());
        if (!existeUsuario)
        {
            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Rol = "Cliente",
                Activo = true
            };
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Usuario>();
            nuevoUsuario.ContrasenaHash = passwordHasher.HashPassword(nuevoUsuario, "Cliente123*");
            
            await usuarioRepository.AddAsync(nuevoUsuario);
        }

        // e) Confirmar la transacción atómica
        await _unitOfWork.CompleteAsync();

        // f) Retornar la respuesta mapeando mediante Mapster
        return nuevoCliente.Adapt<ClienteResponseDto>();
    }
}
