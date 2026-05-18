using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using WebApplication.Data;
using WebApplication.Entities;
using WebApplication.Models;
using WebApplication.Validators;

namespace WebApplication.Tests.Validators;

public class ForgotPasswordValidatorTests
{
    private TfaContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TfaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TfaContext(options);
    }

    [Fact]
    public async Task Validate_ExistingEmail_ReturnsValid()
    {
        using var ctx = CreateContext();
        ctx.Users.Add(new User { Id = Guid.NewGuid(), Email = "user@test.com", Password = "hashed" });
        await ctx.SaveChangesAsync();

        var validator = new ForgotPasswordValidator(ctx);
        var result = await validator.ValidateAsync(new ForgotPassword { Email = "user@test.com" });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NonExistentEmail_ReturnsInvalid()
    {
        using var ctx = CreateContext();
        var validator = new ForgotPasswordValidator(ctx);

        var result = await validator.ValidateAsync(new ForgotPassword { Email = "ghost@test.com" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("No account found with this e-mail"));
    }

    [Fact]
    public async Task Validate_EmptyEmail_ReturnsInvalid()
    {
        using var ctx = CreateContext();
        var validator = new ForgotPasswordValidator(ctx);

        var result = await validator.ValidateAsync(new ForgotPassword { Email = "" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail is required"));
    }

    [Fact]
    public async Task Validate_InvalidEmailFormat_ReturnsInvalid()
    {
        using var ctx = CreateContext();
        var validator = new ForgotPasswordValidator(ctx);

        var result = await validator.ValidateAsync(new ForgotPassword { Email = "not-an-email" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("E-mail format is invalid"));
    }
}
