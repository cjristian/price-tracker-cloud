using FluentValidation;
using PriceTrackerCloud.Application.Commands.Alerts;

namespace PriceTrackerCloud.Application.Validators.Alerts;

public class CreateAlertCommandValidator : AbstractValidator<CreateAlertCommand>
{
    public CreateAlertCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("El Id del producto es obligatorio.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El Id del usuario es obligatorio.");

        RuleFor(x => x.TargetPrice)
            .GreaterThan(0).WithMessage("El precio objetivo debe ser mayor que 0.");
    }
}
