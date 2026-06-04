using System;
using System.Text.Json;
using System.Threading.Tasks;
using AutoTallerManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AutoTallerManager.Api.Middleware;

public class JwtBlocklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtBlocklistMiddleware> _logger;

    public JwtBlocklistMiddleware(RequestDelegate next, ILogger<JwtBlocklistMiddleware> logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlocklistService blocklistService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // Buscar el claim jti de forma segura (tanto por nombre corto como por la constante estandarizada)
            var jti = context.User.FindFirst("jti")?.Value 
                      ?? context.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti) && await blocklistService.IsTokenBlockedAsync(jti))
            {
                // Obtener o generar CorrelationId para mantener consistencia absoluta con el GlobalExceptionMiddleware
                if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
                {
                    correlationId = Guid.NewGuid().ToString();
                }

                _logger.LogWarning("Intento de acceso con token revocado bloqueado. [JTI: {Jti}] [CorrelationId: {CorrelationId}] [Path: {Path}] [Method: {Method}]", 
                    jti, correlationId.ToString(), context.Request.Path, context.Request.Method);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                var responsePayload = new
                {
                    status = StatusCodes.Status401Unauthorized,
                    mensaje = "Este token ha sido revocado. Por favor, inicie sesión nuevamente.",
                    correlationId = correlationId.ToString()
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(responsePayload, options);
                await context.Response.WriteAsync(json);
                return;
            }
        }

        await _next(context);
    }
}
