using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Tests.Handlers;

public class VerifyCodeHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly NotificationContext _notification;
    private readonly VerifyCodeHandler _handler;

    public VerifyCodeHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _notification = new NotificationContext();
        _handler = new VerifyCodeHandler(_notification, _userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCode_ReturnsAuthWithTokens()
    {
        var userId = Guid.NewGuid();
        var expected = new Auth { AccessToken = "access-token", RefreshToken = "refresh-token" };

        _userManagerMock
            .Setup(m => m.VerifyCodeAsync(userId, "654321"))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new VerifyCode { Hash = userId, Code = "654321" }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("access-token");
        _notification.HasNotifications.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_InvalidCode_AddsNotificationAndReturnsNull()
    {
        _userManagerMock
            .Setup(m => m.VerifyCodeAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((Auth?)null);

        var result = await _handler.Handle(new VerifyCode { Hash = Guid.NewGuid(), Code = "000000" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("Invalid or expired verification code"));
    }

    [Fact]
    public async Task Handle_ExpiredCode_AddsNotificationAndReturnsNull()
    {
        _userManagerMock
            .Setup(m => m.VerifyCodeAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((Auth?)null);

        var result = await _handler.Handle(new VerifyCode { Hash = Guid.NewGuid(), Code = "111111" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.HasNotifications.Should().BeTrue();
    }
}
