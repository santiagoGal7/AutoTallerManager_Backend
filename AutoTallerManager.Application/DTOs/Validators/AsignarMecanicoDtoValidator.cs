using FluentValidation;

namespace AutoTallerManager.Application.DTOs.Validators;

public class AsignarMecanicoDtoValidator : AbstractValidator<AsignarMecanicoDto>
{
    public AsignarMecanicoDtoValidator()
    {
        RuleFor(x => x.MecanicoId)
            .NotEmpty().WithMessage("El ID del mecánico es obligatorio.")
            .GreaterThan(0).WithMessage("El ID del mecánico debe ser mayor a cero.");
    }
}
