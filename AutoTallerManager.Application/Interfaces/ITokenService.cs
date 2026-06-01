using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}
