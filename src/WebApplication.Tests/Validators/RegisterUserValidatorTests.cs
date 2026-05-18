using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using WebApplication.Data;
using WebApplication.Entities;
using WebApplication.Validators;

namespace WebApplication.Tests.Validators;

public class RegisterUserValidatorTests
{
    private TfaContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TfaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TfaContext(options);
    }

    [Fact]
    public async Task Validate_ValidUser_ReturnsValid()
    {
        using var ctx = CreateContext();
        var validator = new RegisterUserValidator(ctx);

        var result = await validator.ValidateAsync(new User { Email = "new@test.com", Password = "Password1!" });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_DuplicateEmail_ReturnsInvalid()
    {
        using var ctx = CreateContext();
        ctx.Users.Add(new User { Id = Guid.NewGuid(), Email = "exists@test.com", Password = "hashed", EmailVerified = true });
        await ctx.SaveChangesAsync();

        var validator = new RegisterUserValidator(ctx);
        var result = await validator.ValidateAsync(new User { Email = "exists@test.com", Password = "Password1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("already registered"));
    }

    [Fact]
    public async Task Validate_EmptyEmail_ReturnsInvalid()
    {
        using var ctx = CreateContext();
        var validator = new RegisterUserValidator(ctx);

        var result = await validator.ValidateAsync(new User { Email = "", Password = "Password1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail is required"));
    }

    [Fact]
    public async Task Validate_InvalidEmailFormat_ReturnsInvalid()
    {
        using var ctx = CreateContext();
        var validator = new RegisterUserValidator(ctx);

        var result = await validator.ValidateAsync(new User { Email = "not-an-email", Password = "Password1!" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail format is invalid"));
    }

    [Theory]
    [InlineData("short1!", "Password must be at least 8 characters")]
    [InlineData("nouppercase1!", "Password must contain at least one uppercase letter")]
    [InlineData("NOLOWERCASE1!", "Password must contain at least one lowercase letter")]
    [InlineData("NoDigitsHere!", "Password must contain at least one number")]
    [InlineData("NoSpecial1chars", "Password must contain at least one special character")]
    public async Task Validate_WeakPassword_ReturnsInvalidWithMessage(string password, string expectedMessage)
    {
        using var ctx = CreateContext();
        var validator = new RegisterUserValidator(ctx);

        var result = await validator.ValidateAsync(new User { Email = "user@test.com", Password = password });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
    }

    [Fact]
    public async Task Validate_EmptyPassword_ReturnsInvalid()
    {
        using var ctx = CreateContext();
        var validator = new RegisterUserValidator(ctx);

        var result = await validator.ValidateAsync(new User { Email = "user@test.com", Password = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Password is required"));
    }
}
