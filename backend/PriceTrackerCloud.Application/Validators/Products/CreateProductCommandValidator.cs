using FluentValidation;
using PriceTrackerCloud.Application.Commands.Products;

namespace PriceTrackerCloud.Application.Validators.Products;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("La descripción no puede superar 1000 caracteres.");
    }
}
