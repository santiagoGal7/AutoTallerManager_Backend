using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Api.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, List<string>>();

        // 1. Capturar errores existentes en el ModelState (ej. errores de enlace JSON, tipos incompatibles o anotaciones de datos)
        if (!context.ModelState.IsValid)
        {
            foreach (var state in context.ModelState)
            {
                var fieldErrors = state.Value.Errors
                    .Select(e => e.ErrorMessage)
                    .Where(msg => !string.IsNullOrEmpty(msg))
                    .ToList();

                if (fieldErrors.Any())
                {
                    errors[state.Key] = fieldErrors;
                }
            }
        }

        // 2. Ejecutar FluentValidation de forma dinámica para los DTOs recibidos en los argumentos de la acción
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            var argumentType = argument.GetType();
            // Resolver el validador registrado en el DI para este tipo de objeto
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        if (!errors.ContainsKey(failure.PropertyName))
                        {
                            errors[failure.PropertyName] = new List<string>();
                        }

                        // Evitar mensajes duplicados
                        if (!errors[failure.PropertyName].Contains(failure.ErrorMessage))
                        {
                            errors[failure.PropertyName].Add(failure.ErrorMessage);
                        }
                    }
                }
            }
        }

        // 3. Si existen errores de validación, abortar la ejecución y retornar el JSON estructurado con un 400 Bad Request
        if (errors.Any())
        {
            var responsePayload = new
            {
                Status = 400,
                Mensaje = "Se presentaron uno o más errores de validación.",
                Errores = errors
            };

            context.Result = new BadRequestObjectResult(responsePayload);
            return;
        }

        await next();
    }
}
