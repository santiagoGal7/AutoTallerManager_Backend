using FluentValidation;

namespace AutoTallerManager.Application.DTOs.Validators;

public class RegistrarPagoDtoValidator : AbstractValidator<RegistrarPagoDto>
{
    public RegistrarPagoDtoValidator()
    {
        RuleFor(x => x.FacturaId)
            .NotEmpty().WithMessage("El ID de la factura es obligatorio.")
            .GreaterThan(0).WithMessage("El ID de la factura debe ser mayor a cero.");

        RuleFor(x => x.MedioPagoId)
            .NotEmpty().WithMessage("El ID del medio de pago es obligatorio.")
            .GreaterThan(0).WithMessage("El ID del medio de pago debe ser mayor a cero.");

        RuleFor(x => x.MontoPagado)
            .GreaterThan(0.0m).WithMessage("El monto pagado debe ser mayor a cero.");

        RuleFor(x => x.TransaccionReferencia)
            .NotEmpty().WithMessage("La referencia de transacción es obligatoria.")
            .MaximumLength(100).WithMessage("La referencia de transacción no puede superar los 100 caracteres.");
    }
}
