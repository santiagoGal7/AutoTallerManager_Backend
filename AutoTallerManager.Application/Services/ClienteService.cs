using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs.Clientes;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Application.Exceptions;
using Mapster;

namespace AutoTallerManager.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ClienteService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<ClienteResponseDto> RegistrarClienteConVehiculoAsync(CrearClienteDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // a) Usar el repositorio de Cliente para verificar mediante una expresión si el correo electrónico ya está registrado.
            var existeCorreo = await _unitOfWork.Repository<Cliente>()
                .AnyAsync(c => c.Correo.ToLower() == dto.Correo.ToLower());

            if (existeCorreo)
            {
                throw new BusinessException($"El correo electrónico '{dto.Correo}' ya se encuentra registrado en el sistema.");
            }

            // Validación defensiva y asíncrona de VINs duplicados en la base de datos
            var vehiculoRepository = _unitOfWork.Repository<Vehiculo>();

            if (dto.VehiculoInicial != null)
            {
                var existeVin = await vehiculoRepository.AnyAsync(v => v.VIN.ToUpper() == dto.VehiculoInicial.VIN.ToUpper());
                if (existeVin)
                {
                    throw new BusinessException($"El número de chasis (VIN) '{dto.VehiculoInicial.VIN}' ya se encuentra registrado en el sistema.");
                }
            }

            if (dto.Vehiculos != null && dto.Vehiculos.Any())
            {
                foreach (var vDto in dto.Vehiculos)
                {
                    var existeVin = await vehiculoRepository.AnyAsync(v => v.VIN.ToUpper() == vDto.VIN.ToUpper());
                    if (existeVin)
                    {
                        throw new BusinessException($"El número de chasis (VIN) '{vDto.VIN}' ya se encuentra registrado en el sistema.");
                    }
                }
            }

            // b) Crear credenciales de acceso para el nuevo cliente (Usuario) si no existe en la base de datos
            var usuarioRepository = _unitOfWork.Repository<Usuario>();
            var usuarioExistente = usuarioRepository.Find(u => u.Correo.ToLower() == dto.Correo.ToLower()).FirstOrDefault();
            
            int usuarioId;
            if (usuarioExistente == null)
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Correo = dto.Correo,
                    Rol = "Cliente",
                    Activo = true,
                    ContrasenaHash = _passwordHasher.HashPassword("Cliente123*")
                };
                await usuarioRepository.AddAsync(nuevoUsuario);
                
                // Ejecuta un SaveChangesAsync() intermedio para que PostgreSQL asigne y devuelva el Id
                await _unitOfWork.SaveChangesAsync();
                
                usuarioId = nuevoUsuario.Id;
            }
            else
            {
                usuarioId = usuarioExistente.Id;
            }

            // c) Instanciar la entidad Cliente y asignar explícitamente la FK: cliente.UsuarioId = usuarioId
            var nuevoCliente = new Cliente
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Correo = dto.Correo,
                FechaRegistro = DateTime.UtcNow,
                UsuarioId = usuarioId
            };
            
            await _unitOfWork.Repository<Cliente>().AddAsync(nuevoCliente);
            // SaveChangesAsync intermedio para obtener el Id del cliente
            await _unitOfWork.SaveChangesAsync();

            // d) Unificación de Creación de Vehículos asignando la FK (IdCliente) de forma estricta
            var vehiculosParaInsertar = new List<Vehiculo>();

            if (dto.VehiculoInicial != null)
            {
                var vehiculo = new Vehiculo
                {
                    Marca = dto.VehiculoInicial.Marca,
                    Modelo = dto.VehiculoInicial.Modelo,
                    Anio = dto.VehiculoInicial.Anio,
                    VIN = dto.VehiculoInicial.VIN,
                    IdCliente = nuevoCliente.Id, // Asignación de FK
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
                vehiculosParaInsertar.Add(vehiculo);
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
                        IdCliente = nuevoCliente.Id, // Asignación de FK
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
                    vehiculosParaInsertar.Add(vehiculo);
                }
            }

            // Agregar cada vehículo al repositorio
            foreach (var vehiculo in vehiculosParaInsertar)
            {
                await vehiculoRepository.AddAsync(vehiculo);
            }

            // SaveChanges final y Commit
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Vinculación en memoria para mantener integridad de la respuesta DTO
            nuevoCliente.Vehiculos = vehiculosParaInsertar;

            return nuevoCliente.Adapt<ClienteResponseDto>();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}

