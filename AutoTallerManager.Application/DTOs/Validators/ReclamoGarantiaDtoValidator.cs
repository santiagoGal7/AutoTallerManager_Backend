using FluentValidation;

namespace AutoTallerManager.Application.DTOs.Validators;

public class ReclamoGarantiaDtoValidator : AbstractValidator<ReclamoGarantiaDto>
{
    public ReclamoGarantiaDtoValidator()
    {
        RuleFor(x => x.OrdenOriginalId)
            .NotEmpty().WithMessage("El ID de la orden original es obligatorio.")
            .GreaterThan(0).WithMessage("El ID de la orden original debe ser mayor a cero.");

        RuleFor(x => x.MotivoFalla)
            .NotEmpty().WithMessage("El motivo de la falla es obligatorio.")
            .MaximumLength(500).WithMessage("El motivo de la falla no puede superar los 500 caracteres.");

        RuleFor(x => x.ResolucionDictamen)
            .NotEmpty().WithMessage("La resolución o dictamen es obligatorio.")
            .MaximumLength(200).WithMessage("La resolución o dictamen no puede superar los 200 caracteres.");
    }
}
