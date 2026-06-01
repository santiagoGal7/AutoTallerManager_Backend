using FluentValidation;
using AutoTallerManager.Application.DTOs.Clientes;

namespace AutoTallerManager.Application.DTOs.Validators;

public class CrearClienteDtoValidator : AbstractValidator<CrearClienteDto>
{
    public CrearClienteDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono del cliente es obligatorio.")
            .Matches(@"^\+?[0-9\s-]{7,20}$").WithMessage("El formato del teléfono no es válido.");

        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");
    }
}
