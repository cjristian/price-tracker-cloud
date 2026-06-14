using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Domain.Enums;
using PriceTrackerCloud.Infrastructure.Auth;

namespace PriceTrackerCloud.Tests.Infrastructure;

public class JwtTokenGeneratorTests
{
    private readonly JwtTokenGenerator _sut;

    public JwtTokenGeneratorTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"]          = "super-secret-key-for-testing-minimum-256-bits!!",
                ["JwtSettings:Issuer"]             = "TestIssuer",
                ["JwtSettings:Audience"]           = "TestAudience",
                ["JwtSettings:ExpirationMinutes"]  = "60"
            })
            .Build();

        _sut = new JwtTokenGenerator(config);
    }

    [Fact]
    public void GenerateToken_ShouldReturnNonEmptyString()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Ana", Email = "ana@test.com", Role = UserRole.User };
        var token = _sut.GenerateToken(user);
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldContainUserIdClaim()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "Ana", Email = "ana@test.com", Role = UserRole.User };

        var token = _sut.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value
            .Should().Be(userId.ToString());
    }

    [Fact]
    public void GenerateToken_ShouldContainEmailClaim()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Ana", Email = "ana@test.com", Role = UserRole.User };

        var token = _sut.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value
            .Should().Be("ana@test.com");
    }
}
