using FluentAssertions;
using Xunit;
using WebApplication.Models;
using WebApplication.Validators;

namespace WebApplication.Tests.Validators;

public class ResetPasswordValidatorTests
{
    private readonly ResetPasswordValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_ReturnsValid()
    {
        var result = await _validator.ValidateAsync(new ResetPassword
        {
            Email = "user@test.com",
            Code = "123456",
            Password = "NewPass1!"
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyEmail_ReturnsInvalid()
    {
        var result = await _validator.ValidateAsync(new ResetPassword { Email = "", Code = "123456", Password = "NewPass1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail is required"));
    }

    [Fact]
    public async Task Validate_InvalidEmailFormat_ReturnsInvalid()
    {
        var result = await _validator.ValidateAsync(new ResetPassword { Email = "bademail", Code = "123456", Password = "NewPass1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail format is invalid"));
    }

    [Fact]
    public async Task Validate_EmptyCode_ReturnsInvalid()
    {
        var result = await _validator.ValidateAsync(new ResetPassword { Email = "user@test.com", Code = "", Password = "NewPass1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Code is required"));
    }

    [Fact]
    public async Task Validate_CodeTooShort_ReturnsInvalid()
    {
        var result = await _validator.ValidateAsync(new ResetPassword { Email = "user@test.com", Code = "12345", Password = "NewPass1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Code must be exactly 6 digits"));
    }

    [Fact]
    public async Task Validate_NonNumericCode_ReturnsInvalid()
    {
        var result = await _validator.ValidateAsync(new ResetPassword { Email = "user@test.com", Code = "12AB56", Password = "NewPass1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Code must contain only numbers"));
    }

    [Theory]
    [InlineData("short1!", "Password must be at least 8 characters")]
    [InlineData("nouppercase1!", "Password must contain at least one uppercase letter")]
    [InlineData("NOLOWERCASE1!", "Password must contain at least one lowercase letter")]
    [InlineData("NoDigitsHere!", "Password must contain at least one number")]
    [InlineData("NoSpecial1chars", "Password must contain at least one special character")]
    public async Task Validate_WeakPassword_ReturnsInvalid(string password, string expectedMessage)
    {
        var result = await _validator.ValidateAsync(new ResetPassword
        {
            Email = "user@test.com",
            Code = "123456",
            Password = password
        });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
    }
}
