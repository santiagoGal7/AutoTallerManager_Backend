using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using AutoTallerManager.Application.Exceptions;

namespace AutoTallerManager.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _env = env ?? throw new ArgumentNullException(nameof(env));
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

        // Registrar en el log de forma estructurada e inteligente basada en el entorno
        if (_env.IsDevelopment())
        {
            // En desarrollo, registramos la excepción completa para facilitar el debugging
            _logger.LogError(
                exception,
                "Error detectado en el pipeline del servidor. [CorrelationId: {CorrelationId}] [Path: {Path}] [Method: {Method}] [Message: {Message}]",
                correlationId.ToString(),
                context.Request.Path,
                context.Request.Method,
                exception.Message
            );
        }
        else
        {
            // En producción, registramos el error completo con su stack trace a nivel de servidor por observabilidad, sin filtrarlo al cliente
            _logger.LogError(
                exception,
                "Error no controlado en producción. [CorrelationId: {CorrelationId}] [Path: {Path}] [Method: {Method}]",
                correlationId.ToString(),
                context.Request.Path,
                context.Request.Method
            );
        }

        context.Response.ContentType = "application/json";

        object responsePayload;

        if (exception is BusinessException businessException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            responsePayload = new
            {
                status = StatusCodes.Status400BadRequest,
                mensaje = businessException.Message,
                correlationId = correlationId.ToString()
            };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (_env.IsDevelopment())
            {
                // En desarrollo expone detalles técnicos para agilizar la depuración
                responsePayload = new
                {
                    status = StatusCodes.Status500InternalServerError,
                    mensaje = "Ocurrió un error interno en el servidor al procesar la solicitud.",
                    detalle = exception.Message,
                    stackTrace = exception.StackTrace,
                    correlationId = correlationId.ToString()
                };
            }
            else
            {
                // En producción enmascara la excepción por seguridad y solo retorna un mensaje genérico con el CorrelationId
                responsePayload = new
                {
                    error = "Ocurrió un problema interno",
                    correlationId = correlationId.ToString()
                };
            }
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(responsePayload, options);
        await context.Response.WriteAsync(json);
    }
}
