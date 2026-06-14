using FluentAssertions;
using PriceTrackerCloud.Infrastructure.Auth;

namespace PriceTrackerCloud.Tests.Infrastructure;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _sut = new();

    [Fact]
    public void Hash_ShouldReturnNonEmptyString()
    {
        var hash = _sut.Hash("mypassword");
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Hash_SamePassword_ShouldProduceDifferentHashes()
    {
        var hash1 = _sut.Hash("mypassword");
        var hash2 = _sut.Hash("mypassword");
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Verify_CorrectPassword_ShouldReturnTrue()
    {
        var hash = _sut.Hash("mypassword");
        _sut.Verify(hash, "mypassword").Should().BeTrue();
    }

    [Fact]
    public void Verify_WrongPassword_ShouldReturnFalse()
    {
        var hash = _sut.Hash("mypassword");
        _sut.Verify(hash, "wrongpassword").Should().BeFalse();
    }
}
