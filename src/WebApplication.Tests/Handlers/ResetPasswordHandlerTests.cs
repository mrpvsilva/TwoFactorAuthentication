using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Validators;

namespace WebApplication.Tests.Handlers;

public class ResetPasswordHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly NotificationContext _notification;

    public ResetPasswordHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _notification = new NotificationContext();
    }

    private ResetPasswordHandler CreateHandler()
    {
        var validator = new ResetPasswordValidator();
        return new ResetPasswordHandler(_notification, _userManagerMock.Object, validator);
    }

    [Fact]
    public async Task Handle_ValidRequest_ResetsPasswordAndReturnsTrue()
    {
        _userManagerMock
            .Setup(m => m.ResetPasswordAsync("user@test.com", "123456", "NewPass1!"))
            .ReturnsAsync(true);

        var handler = CreateHandler();
        var result = await handler.Handle(new ResetPassword
        {
            Email = "user@test.com",
            Code = "123456",
            Password = "NewPass1!"
        }, CancellationToken.None);

        result.Should().BeTrue();
        _notification.HasNotifications.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_InvalidOrExpiredCode_AddsNotificationAndReturnsFalse()
    {
        _userManagerMock
            .Setup(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();
        var result = await handler.Handle(new ResetPassword
        {
            Email = "user@test.com",
            Code = "999999",
            Password = "NewPass1!"
        }, CancellationToken.None);

        result.Should().Be(default(bool));
        _notification.Notifications.Should().Contain(n => n.Message.Contains("Invalid or expired code"));
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_AddsNotificationAndReturnsFalse()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new ResetPassword
        {
            Email = "bademail",
            Code = "123456",
            Password = "NewPass1!"
        }, CancellationToken.None);

        result.Should().Be(default(bool));
        _notification.Notifications.Should().Contain(n => n.Message.Contains("E-mail format is invalid"));
        _userManagerMock.Verify(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CodeNotSixDigits_AddsNotificationAndReturnsFalse()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new ResetPassword
        {
            Email = "user@test.com",
            Code = "12345",
            Password = "NewPass1!"
        }, CancellationToken.None);

        result.Should().Be(default(bool));
        _notification.Notifications.Should().Contain(n => n.Message.Contains("Code must be exactly 6 digits"));
        _userManagerMock.Verify(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NonNumericCode_AddsNotificationAndReturnsFalse()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new ResetPassword
        {
            Email = "user@test.com",
            Code = "12AB56",
            Password = "NewPass1!"
        }, CancellationToken.None);

        result.Should().Be(default(bool));
        _notification.Notifications.Should().Contain(n => n.Message.Contains("Code must contain only numbers"));
    }

    [Theory]
    [InlineData("short1!", "Password must be at least 8 characters")]
    [InlineData("nouppercase1!", "Password must contain at least one uppercase letter")]
    [InlineData("NOLOWERCASE1!", "Password must contain at least one lowercase letter")]
    [InlineData("NoDigitsHere!", "Password must contain at least one number")]
    [InlineData("NoSpecial1chars", "Password must contain at least one special character")]
    public async Task Handle_WeakNewPassword_AddsNotificationAndReturnsFalse(string password, string expectedMessage)
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new ResetPassword
        {
            Email = "user@test.com",
            Code = "123456",
            Password = password
        }, CancellationToken.None);

        result.Should().Be(default(bool));
        _notification.Notifications.Should().Contain(n => n.Message.Contains(expectedMessage));
        _userManagerMock.Verify(m => m.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
