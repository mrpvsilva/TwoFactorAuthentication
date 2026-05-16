using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Validators;

namespace WebApplication.Tests.Handlers;

public class AuthHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly NotificationContext _notification;

    public AuthHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _notification = new NotificationContext();
    }

    private AuthHandler CreateHandler()
    {
        var validator = new AuthValidator(_userManagerMock.Object);
        return new AuthHandler(_notification, _userManagerMock.Object, validator);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTwoFactAuth()
    {
        var user = new Entities.User { Id = Guid.NewGuid(), Email = "user@test.com" };
        var expected = new TwoFactAuth { Hash = user.Id, HasTwoFactorAuth = false };

        _userManagerMock
            .Setup(m => m.PasswordSignInAsync("user@test.com", "Password1!"))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(m => m.GetTwoFactAuthAsync("user@test.com"))
            .ReturnsAsync(expected);

        var handler = CreateHandler();
        var result = await handler.Handle(new Account { Email = "user@test.com", Password = "Password1!" }, CancellationToken.None);

        result.Should().NotBeNull();
        result.Hash.Should().Be(user.Id);
        _notification.HasNotifications.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_InvalidCredentials_AddsNotificationAndReturnsNull()
    {
        _userManagerMock
            .Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Entities.User?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(new Account { Email = "user@test.com", Password = "WrongPass1!" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.HasNotifications.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmptyEmail_AddsNotificationAndReturnsNull()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new Account { Email = "", Password = "Password1!" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("E-mail is required"));
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_AddsNotificationAndReturnsNull()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new Account { Email = "not-an-email", Password = "Password1!" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("E-mail format is invalid"));
    }

    [Fact]
    public async Task Handle_EmptyPassword_AddsNotificationAndReturnsNull()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new Account { Email = "user@test.com", Password = "" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("Password is required"));
    }
}
