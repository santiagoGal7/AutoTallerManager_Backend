using System;
using System.Threading.Tasks;

namespace AutoTallerManager.Application.Interfaces;

public interface ITokenBlocklistService
{
    /// <summary>
    /// Registra un identificador de token (jti) en la lista de revocación con un tiempo de expiración determinado.
    /// </summary>
    Task BlockTokenAsync(string jti, TimeSpan expiry);

    /// <summary>
    /// Consulta si un identificador de token (jti) se encuentra revocado.
    /// </summary>
    Task<bool> IsTokenBlockedAsync(string jti);
}
