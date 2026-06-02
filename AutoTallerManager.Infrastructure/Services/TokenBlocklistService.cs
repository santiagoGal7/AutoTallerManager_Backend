using System;
using System.Threading.Tasks;
using AutoTallerManager.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AutoTallerManager.Infrastructure.Services;

public class TokenBlocklistService : ITokenBlocklistService
{
    private readonly IMemoryCache _memoryCache;
    private const string CachePrefix = "revoked_token:";

    public TokenBlocklistService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public Task BlockTokenAsync(string jti, TimeSpan expiry)
    {
        if (string.IsNullOrWhiteSpace(jti))
            throw new ArgumentException("El identificador del token (jti) no puede ser nulo o vacío.", nameof(jti));

        if (expiry <= TimeSpan.Zero)
            return Task.CompletedTask; // El token ya ha expirado, no es necesario cachear

        var cacheKey = $"{CachePrefix}{jti}";
        _memoryCache.Set(cacheKey, true, expiry);

        return Task.CompletedTask;
    }

    public Task<bool> IsTokenBlockedAsync(string jti)
    {
        if (string.IsNullOrWhiteSpace(jti))
            return Task.FromResult(false);

        var cacheKey = $"{CachePrefix}{jti}";
        var isBlocked = _memoryCache.TryGetValue(cacheKey, out _);

        return Task.FromResult(isBlocked);
    }
}
