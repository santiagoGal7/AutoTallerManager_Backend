using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Api.Middleware;

public class AuditoriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditoriaMiddleware> _logger;
    private readonly List<string> _rutasExcluidas = new()
    {
        "/api/usuarios/login",
        "/api/auth/login",
        "/api/auth/registro"
    };

    public AuditoriaMiddleware(RequestDelegate next, ILogger<AuditoriaMiddleware> logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId) || string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers["X-Correlation-ID"] = correlationId;
        }

        var method = context.Request.Method;
        var isWrite = method == "POST" || method == "PUT" || method == "PATCH" || method == "DELETE";
        var path = (context.Request.Path.Value ?? string.Empty).TrimEnd('/');
        var esRutaExcluida = _rutasExcluidas.Any(r => string.Equals(r.TrimEnd('/'), path, StringComparison.OrdinalIgnoreCase));

        string requestBody = string.Empty;
        if (isWrite && !esRutaExcluida)
        {
            try
            {
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    var originalBody = await reader.ReadToEndAsync();
                    requestBody = SanitizarBody(originalBody, correlationId.ToString());
                    context.Request.Body.Position = 0;
                }
            }
            catch (Exception ex)
            {
                // Loguear error pero no interrumpir la ejecución del pipeline
                _logger.LogWarning(ex, "Advertencia en AuditoriaMiddleware (Lectura/Sanitización). [CorrelationId: {CorrelationId}] [Path: {Path}]", correlationId.ToString(), context.Request.Path);
            }
        }

        await _next(context);

        // Solo auditar si la petición de escritura fue exitosa (código 2xx)
        if (isWrite && context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            try
            {
                // Extraer nombre de la entidad desde los segmentos de ruta
                var pathSegments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var entity = pathSegments?.Length > 1 ? pathSegments[1] : "Sistema";

                // Extraer el ID del usuario del token JWT
                int? usuarioId = null;
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedId))
                {
                    usuarioId = parsedId;
                }

                var tipoAccion = method switch
                {
                    "POST" => "INSERT",
                    "PUT" or "PATCH" => "UPDATE",
                    "DELETE" => "DELETE",
                    _ => method
                };

                // Resolver IUnitOfWork desde los servicios de la petición (Scoped)
                var unitOfWork = context.RequestServices.GetRequiredService<IUnitOfWork>();
                var auditRecord = new AuditoriaTransaccion
                {
                    UsuarioId = usuarioId,
                    EntidadAfectada = entity,
                    TipoAccion = tipoAccion,
                    DetalleDatos = string.IsNullOrWhiteSpace(requestBody) ? "{}" : requestBody,
                    FechaHora = DateTime.UtcNow
                };

                await unitOfWork.Repository<AuditoriaTransaccion>().AddAsync(auditRecord);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Loguear error pero no interrumpir la respuesta al cliente
                _logger.LogWarning(ex, "Advertencia en AuditoriaMiddleware (Persistencia). [CorrelationId: {CorrelationId}] [Path: {Path}]", correlationId.ToString(), context.Request.Path);
            }
        }
    }

    private string SanitizarBody(string body, string correlationId)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return body;
        }

        try
        {
            using (var doc = JsonDocument.Parse(body))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
                    {
                        SanitizarElement(doc.RootElement, writer);
                    }
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            // Loguear el error de análisis pero no exponer datos sensibles y retornar JSON vacío.
            _logger.LogWarning(ex, "Advertencia al sanitizar JSON en el cuerpo de la solicitud. [CorrelationId: {CorrelationId}]", correlationId);
            return "{}";
        }
    }

    private void SanitizarElement(JsonElement element, Utf8JsonWriter writer, string? propertyName = null)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                if (propertyName != null && EsCampoSensible(propertyName))
                {
                    writer.WriteString(propertyName, "*****");
                }
                else
                {
                    if (propertyName != null)
                    {
                        writer.WriteStartObject(propertyName);
                    }
                    else
                    {
                        writer.WriteStartObject();
                    }

                    foreach (var property in element.EnumerateObject())
                    {
                        SanitizarElement(property.Value, writer, property.Name);
                    }

                    writer.WriteEndObject();
                }
                break;

            case JsonValueKind.Array:
                if (propertyName != null && EsCampoSensible(propertyName))
                {
                    writer.WriteString(propertyName, "*****");
                }
                else
                {
                    if (propertyName != null)
                    {
                        writer.WriteStartArray(propertyName);
                    }
                    else
                    {
                        writer.WriteStartArray();
                    }

                    foreach (var item in element.EnumerateArray())
                    {
                        SanitizarElement(item, writer);
                    }

                    writer.WriteEndArray();
                }
                break;

            default:
                if (propertyName != null)
                {
                    if (EsCampoSensible(propertyName))
                    {
                        writer.WriteString(propertyName, "*****");
                    }
                    else
                    {
                        element.WriteTo(writer);
                    }
                }
                else
                {
                    element.WriteTo(writer);
                }
                break;
        }
    }

    private bool EsCampoSensible(string nombreCampo)
    {
        if (string.IsNullOrEmpty(nombreCampo)) return false;

        var nombreLower = nombreCampo.ToLowerInvariant();
        return nombreLower == "password" ||
               nombreLower == "contrasena" ||
               nombreLower == "token" ||
               nombreLower == "cvv" ||
               nombreLower == "tarjeta";
    }
}
