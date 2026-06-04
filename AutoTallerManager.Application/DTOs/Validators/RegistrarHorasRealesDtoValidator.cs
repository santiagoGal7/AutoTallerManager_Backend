using FluentValidation;

namespace AutoTallerManager.Application.DTOs.Validators;

public class RegistrarHorasRealesDtoValidator : AbstractValidator<RegistrarHorasRealesDto>
{
    public RegistrarHorasRealesDtoValidator()
    {
        RuleFor(x => x.HorasReales)
            .NotNull().WithMessage("Las horas reales son obligatorias.")
            .GreaterThanOrEqualTo(0).WithMessage("Las horas reales no pueden ser un valor negativo.");
    }
}
