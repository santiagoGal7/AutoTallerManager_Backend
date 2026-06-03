using FluentValidation;
using System;

namespace AutoTallerManager.Application.DTOs.Validators;

public class ProgramarCitaDtoValidator : AbstractValidator<ProgramarCitaDto>
{
    public ProgramarCitaDtoValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("El ID del cliente es obligatorio.")
            .GreaterThan(0).WithMessage("El ID del cliente debe ser mayor a cero.");

        RuleFor(x => x.VehiculoId)
            .NotEmpty().WithMessage("El ID del vehículo es obligatorio.")
            .GreaterThan(0).WithMessage("El ID del vehículo debe ser mayor a cero.");

        RuleFor(x => x.ServicioTallerId)
            .NotEmpty().WithMessage("El ID del servicio es obligatorio.")
            .GreaterThan(0).WithMessage("El ID del servicio debe ser mayor a cero.");

        RuleFor(x => x.FechaHoraReserva)
            .NotEmpty().WithMessage("La fecha y hora de reserva son obligatorias.")
            .GreaterThan(DateTime.UtcNow).WithMessage("La fecha de reserva debe ser en el futuro.");

        RuleFor(x => x.NotasSintomas)
            .NotEmpty().WithMessage("Las notas de los síntomas son obligatorias.")
            .MaximumLength(500).WithMessage("Las notas no pueden superar los 500 caracteres.");
    }
}
