using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Entities;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Validators;

namespace WebApplication.Tests.Validators;

public class AuthValidatorTests
{
    private readonly Mock<IUserManager> _userManagerMock;

    public AuthValidatorTests()
    {
        _userManagerMock = new Mock<IUserManager>();
    }

    [Fact]
    public async Task Validate_ValidCredentials_ReturnsValid()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "user@test.com", EmailVerified = true };

        _userManagerMock
            .Setup(m => m.PasswordSignInAsync("user@test.com", "Password1!"))
            .ReturnsAsync(user);

        var validator = new AuthValidator(_userManagerMock.Object);
        var result = await validator.ValidateAsync(new Account { Email = "user@test.com", Password = "Password1!" });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_InvalidCredentials_ReturnsInvalid()
    {
        _userManagerMock
            .Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var validator = new AuthValidator(_userManagerMock.Object);
        var result = await validator.ValidateAsync(new Account { Email = "user@test.com", Password = "WrongPass1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Invalid e-mail or password"));
    }

    [Fact]
    public async Task Validate_EmptyEmail_ReturnsInvalid()
    {
        var validator = new AuthValidator(_userManagerMock.Object);
        var result = await validator.ValidateAsync(new Account { Email = "", Password = "Password1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail is required"));
    }

    [Fact]
    public async Task Validate_InvalidEmailFormat_ReturnsInvalid()
    {
        var validator = new AuthValidator(_userManagerMock.Object);
        var result = await validator.ValidateAsync(new Account { Email = "not-an-email", Password = "Password1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail format is invalid"));
    }

    [Fact]
    public async Task Validate_EmptyPassword_ReturnsInvalid()
    {
        var validator = new AuthValidator(_userManagerMock.Object);
        var result = await validator.ValidateAsync(new Account { Email = "user@test.com", Password = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Password is required"));
    }

    [Fact]
    public async Task Validate_EmptyEmailAndPassword_DoesNotCallPasswordSignIn()
    {
        var validator = new AuthValidator(_userManagerMock.Object);
        await validator.ValidateAsync(new Account { Email = "", Password = "" });

        _userManagerMock.Verify(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
