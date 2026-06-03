using FluentValidation;

namespace AutoTallerManager.Application.DTOs.Validators;

public class RegistrarDiagnosticoDtoValidator : AbstractValidator<RegistrarDiagnosticoDto>
{
    public RegistrarDiagnosticoDtoValidator()
    {
        RuleFor(x => x.Diagnostico)
            .NotEmpty().WithMessage("El diagnóstico es obligatorio.")
            .MaximumLength(1000).WithMessage("El diagnóstico no puede superar los 1000 caracteres.");
    }
}
