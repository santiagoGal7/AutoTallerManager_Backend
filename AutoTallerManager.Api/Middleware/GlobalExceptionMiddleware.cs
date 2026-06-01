using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoTallerManager.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Obtener o generar CorrelationId
        if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Registrar en el log de forma estructurada: CorrelationId, mensaje completo y la excepción (que incluye StackTrace)
        _logger.LogError(
            exception,
            "Error detectado en el pipeline del servidor. [CorrelationId: {CorrelationId}] [Path: {Path}] [Method: {Method}] [Message: {Message}] [StackTrace: {StackTrace}]",
            correlationId.ToString(),
            context.Request.Path,
            context.Request.Method,
            exception.Message,
            exception.StackTrace
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Devolver un mensaje genérico al cliente para evitar exponer vulnerabilidades
        var responsePayload = new
        {
            status = StatusCodes.Status500InternalServerError,
            mensaje = "Ocurrió un error interno en el servidor al procesar la solicitud. Por favor, contacte al soporte técnico con su CorrelationId.",
            correlationId = correlationId.ToString()
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(responsePayload, options);
        await context.Response.WriteAsync(json);
    }
}
