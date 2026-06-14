using FluentAssertions;
using PriceTrackerCloud.Application.Commands.Products;
using PriceTrackerCloud.Application.Validators.Products;

namespace PriceTrackerCloud.Tests.Validators;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var cmd = new CreateProductCommand("PlayStation 5", "Consola Sony", "Videojuegos");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_ShouldFail()
    {
        var cmd = new CreateProductCommand("", "desc", "Categoria");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "El nombre del producto es obligatorio.");
    }

    [Fact]
    public void Validate_EmptyCategory_ShouldFail()
    {
        var cmd = new CreateProductCommand("Producto", "desc", "");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "La categoría es obligatoria.");
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldFail()
    {
        var cmd = new CreateProductCommand("Producto", new string('x', 1001), "Categoria");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
    }
}
