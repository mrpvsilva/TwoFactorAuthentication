using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Tests.Handlers;

public class AddTwoFactAuthHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly NotificationContext _notification;
    private readonly AddTwoFactAuthHandler _handler;

    public AddTwoFactAuthHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _notification = new NotificationContext();
        _handler = new AddTwoFactAuthHandler(_notification, _userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCode_ReturnsAuthWithTokens()
    {
        var userId = Guid.NewGuid();
        var uri = "otpauth://totp/...";
        var expected = new Auth { AccessToken = "access-token", RefreshToken = "refresh-token" };

        _userManagerMock
            .Setup(m => m.AddTwoFactorTokenAsync(userId, uri, "123456"))
            .ReturnsAsync(expected);

        var request = new AddTwoFactAuth { Hash = userId, AuthenticatorUri = uri, Code = "123456" };
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("access-token");
        _notification.HasNotifications.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_InvalidCode_AddsNotificationAndReturnsNull()
    {
        _userManagerMock
            .Setup(m => m.AddTwoFactorTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Auth?)null);

        var request = new AddTwoFactAuth { Hash = Guid.NewGuid(), AuthenticatorUri = "uri", Code = "000000" };
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("Invalid authenticator code"));
    }
}
