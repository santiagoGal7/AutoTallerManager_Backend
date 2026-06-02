using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoTallerManager.Application.DTOs.Auth;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace AutoTallerManager.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private static readonly IReadOnlyCollection<string> AllowedRoles = new[] { "Admin", "Mecanico", "Recepcionista", "Cliente" };

    public UsuarioService(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var repository = _unitOfWork.Repository<Usuario>();
        
        // Buscar el usuario por correo de forma directa en la base de datos
        var usuario = repository.Find(u => u.Correo.ToLower() == dto.Correo.ToLower()).FirstOrDefault();

        if (usuario == null || !usuario.Activo)
        {
            return null;
        }

        // Verificar la contraseña hash
        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, dto.Contrasena);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        // Generar el token
        var token = _tokenService.GenerateToken(usuario);

        return new LoginResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Rol = usuario.Rol,
            Token = token
        };
    }

    public async Task<UsuarioResponseDto> RegistrarUsuarioAsync(CrearUsuarioDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var repository = _unitOfWork.Repository<Usuario>();
        
        // Verificar si el correo ya existe de forma directa en la base de datos
        var existe = await repository.AnyAsync(u => u.Correo.ToLower() == dto.Correo.ToLower());
        if (existe)
        {
            throw new InvalidOperationException($"El correo '{dto.Correo}' ya se encuentra registrado.");
        }

        // Crear la entidad
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            Rol = dto.Rol,
            Activo = true
        };

        // Hashear contraseña
        usuario.ContrasenaHash = _passwordHasher.HashPassword(usuario, dto.Contrasena);

        // Guardar en la base de datos
        await repository.AddAsync(usuario);
        await _unitOfWork.CompleteAsync();

        return usuario.Adapt<UsuarioResponseDto>();
    }

    public async Task<UsuarioResponseDto> RegistrarAsync(RegistroDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        // Determinar rol asignado con fallback de menor privilegio "Cliente"
        var rolAsignado = string.IsNullOrWhiteSpace(dto.Rol) ? "Cliente" : dto.Rol.Trim();

        // Control proactivo: Validación contra lista blanca antes de acceder a la base de datos
        var normalizedRol = AllowedRoles.FirstOrDefault(r => string.Equals(r, rolAsignado, StringComparison.OrdinalIgnoreCase));
        if (normalizedRol == null)
        {
            throw new AutoTallerManager.Application.Exceptions.BusinessException($"El rol '{rolAsignado}' no está permitido en el sistema de negocio.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var repository = _unitOfWork.Repository<Usuario>();
            
            // Verificar si el correo ya existe de forma directa en la base de datos
            var existe = await repository.AnyAsync(u => u.Correo.ToLower() == dto.Correo.ToLower());
            if (existe)
            {
                throw new InvalidOperationException($"El correo '{dto.Correo}' ya se encuentra registrado.");
            }

            // Crear la entidad
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                Rol = normalizedRol,
                Activo = true
            };

            // Hashear contraseña
            usuario.ContrasenaHash = _passwordHasher.HashPassword(usuario, dto.Contrasena);

            // Guardar en la base de datos
            await repository.AddAsync(usuario);

            // Si el rol es "Cliente", crear la entidad Cliente vinculando el Usuario
            if (string.Equals(usuario.Rol, "Cliente", StringComparison.OrdinalIgnoreCase))
            {
                var clienteRepository = _unitOfWork.Repository<Cliente>();
                var cliente = new Cliente
                {
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Telefono = dto.Telefono ?? string.Empty,
                    Usuario = usuario
                };
                await clienteRepository.AddAsync(cliente);
            }

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return usuario.Adapt<UsuarioResponseDto>();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ObtenerTodosAsync()
    {
        var repository = _unitOfWork.Repository<Usuario>();
        var usuarios = await repository.GetAllAsync();
        return usuarios.Adapt<IEnumerable<UsuarioResponseDto>>();
    }
}
