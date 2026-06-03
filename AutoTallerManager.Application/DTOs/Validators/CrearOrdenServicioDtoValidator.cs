using FluentValidation;
using System;

namespace AutoTallerManager.Application.DTOs.Validators;

public class CrearOrdenServicioDtoValidator : AbstractValidator<CrearOrdenServicioDto>
{
    public CrearOrdenServicioDtoValidator()
    {
        RuleFor(x => x.VehiculoId)
            .NotEmpty().WithMessage("El ID del vehículo es obligatorio.")
            .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor a cero.");

        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.")
            .GreaterThan(0).WithMessage("El ID del cliente debe ser mayor a cero.");

        RuleFor(x => x.DescripcionProblema)
            .NotEmpty().WithMessage("La descripción del problema o síntoma es obligatoria.");

        RuleFor(x => x.CostoEstimado)
            .GreaterThanOrEqualTo(0).WithMessage("El costo estimado debe ser un valor positivo.");

        RuleFor(x => x.CantidadRepuestos)
            .GreaterThan(0).WithMessage("La cantidad de repuestos debe ser mayor a cero.")
            .When(x => x.CantidadRepuestos.HasValue);

        RuleFor(x => x.FechaCita)
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-1))
            .WithMessage("La fecha de la cita no puede ser una fecha pasada.")
            .When(x => x.FechaCita.HasValue);
    }
}
