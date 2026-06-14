using FluentAssertions;
using Moq;
using PriceTrackerCloud.Application.Commands.Auth;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Tests.Handlers;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtMock = new();

    public RegisterUserCommandHandlerTests()
    {
        _uowMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_NewUser_ShouldReturnToken()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ExistsWithEmailAsync("ana@test.com")).ReturnsAsync(false);
        _hasherMock.Setup(h => h.Hash("Password1")).Returns("hashed_password");
        _jwtMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("jwt_token");
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new RegisterUserCommandHandler(_uowMock.Object, _hasherMock.Object, _jwtMock.Object);
        var cmd = new RegisterUserCommand("Ana", "ana@test.com", "Password1");

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Token.Should().Be("jwt_token");
        result.Email.Should().Be("ana@test.com");
        _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "ana@test.com")), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ExistsWithEmailAsync("ana@test.com")).ReturnsAsync(true);

        var handler = new RegisterUserCommandHandler(_uowMock.Object, _hasherMock.Object, _jwtMock.Object);
        var cmd = new RegisterUserCommand("Ana", "ana@test.com", "Password1");

        // Act
        var act = () => handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ana@test.com*");
    }
}
