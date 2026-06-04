using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Api.Middleware;

public class AuditoriaMiddleware
{
    private readonly RequestDelegate _next;

    public AuditoriaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var isWrite = method == "POST" || method == "PUT" || method == "PATCH" || method == "DELETE";

        string requestBody = string.Empty;
        if (isWrite)
        {
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
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
                Console.WriteLine($"--> Advertencia en AuditoriaMiddleware: {ex.Message}");
            }
        }
    }
}
