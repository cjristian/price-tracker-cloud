using FluentAssertions;
using PriceTrackerCloud.Application.Commands.Alerts;
using PriceTrackerCloud.Application.Validators.Alerts;

namespace PriceTrackerCloud.Tests.Validators;

public class CreateAlertCommandValidatorTests
{
    private readonly CreateAlertCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var cmd = new CreateAlertCommand(Guid.NewGuid(), Guid.NewGuid(), 499.99m);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ZeroTargetPrice_ShouldFail()
    {
        var cmd = new CreateAlertCommand(Guid.NewGuid(), Guid.NewGuid(), 0m);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "El precio objetivo debe ser mayor que 0.");
    }

    [Fact]
    public void Validate_NegativeTargetPrice_ShouldFail()
    {
        var cmd = new CreateAlertCommand(Guid.NewGuid(), Guid.NewGuid(), -10m);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyProductId_ShouldFail()
    {
        var cmd = new CreateAlertCommand(Guid.NewGuid(), Guid.Empty, 100m);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
    }
}
