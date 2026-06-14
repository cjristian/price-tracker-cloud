using FluentAssertions;
using PriceTrackerCloud.Application.Commands.Auth;
using PriceTrackerCloud.Application.Validators.Auth;

namespace PriceTrackerCloud.Tests.Validators;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var cmd = new RegisterUserCommand("Ana García", "ana@test.com", "Password1");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "ana@test.com", "Password1")]   // nombre vacío
    [InlineData("Ana", "", "Password1")]             // email vacío
    [InlineData("Ana", "no-es-email", "Password1")] // email inválido
    [InlineData("Ana", "ana@test.com", "short1")]   // contraseña < 8 chars
    [InlineData("Ana", "ana@test.com", "sinmayusc1")] // sin mayúscula
    [InlineData("Ana", "ana@test.com", "SinNumero")]  // sin número
    public void Validate_InvalidCommand_ShouldFail(string name, string email, string password)
    {
        var cmd = new RegisterUserCommand(name, email, password);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveCorrectErrorMessage()
    {
        var cmd = new RegisterUserCommand("", "ana@test.com", "Password1");
        var result = _validator.Validate(cmd);
        result.Errors.Should().Contain(e => e.ErrorMessage == "El nombre es obligatorio.");
    }
}
