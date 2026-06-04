using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AutoTallerManager.Infrastructure.Services;

public class TokenBlocklistService : ITokenBlocklistService
{
    private readonly IDistributedCache _distributedCache;
    private const string CachePrefix = "revoked_token:";

    public TokenBlocklistService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    public async Task BlockTokenAsync(string jti, TimeSpan expiry)
    {
        if (string.IsNullOrWhiteSpace(jti))
            throw new ArgumentException("El identificador del token (jti) no puede ser nulo o vacío.", nameof(jti));

        if (expiry <= TimeSpan.Zero)
            return; // El token ya ha expirado, no es necesario cachear

        var cacheKey = $"{CachePrefix}{jti}";
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        await _distributedCache.SetStringAsync(cacheKey, "true", options);
    }

    public async Task<bool> IsTokenBlockedAsync(string jti)
    {
        if (string.IsNullOrWhiteSpace(jti))
            return false;

        var cacheKey = $"{CachePrefix}{jti}";
        var value = await _distributedCache.GetStringAsync(cacheKey);

        return value != null;
    }
}
