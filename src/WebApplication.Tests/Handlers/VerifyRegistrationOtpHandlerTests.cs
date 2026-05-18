using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Tests.Handlers;

public class VerifyRegistrationOtpHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly Mock<IEmailOtpManager> _emailOtpManagerMock;
    private readonly NotificationContext _notification;

    public VerifyRegistrationOtpHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _emailOtpManagerMock = new Mock<IEmailOtpManager>();
        _notification = new NotificationContext();
    }

    private VerifyRegistrationOtpHandler CreateHandler() =>
        new VerifyRegistrationOtpHandler(_notification, _userManagerMock.Object, _emailOtpManagerMock.Object);

    [Fact]
    public async Task Handle_ValidCode_MarksEmailVerifiedAndReturnsTrue()
    {
        var userId = Guid.NewGuid();

        _emailOtpManagerMock
            .Setup(m => m.VerifyCodeOnlyAsync(userId, "123456"))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(m => m.MarkEmailVerifiedAsync(userId))
            .ReturnsAsync(true);

        var handler = CreateHandler();
        var result = await handler.Handle(new VerifyRegistrationOtp { Hash = userId, Code = "123456" }, CancellationToken.None);

        result.Should().BeTrue();
        _notification.HasNotifications.Should().BeFalse();
        _userManagerMock.Verify(m => m.MarkEmailVerifiedAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCode_AddsNotificationAndReturnsFalse()
    {
        var userId = Guid.NewGuid();

        _emailOtpManagerMock
            .Setup(m => m.VerifyCodeOnlyAsync(userId, "000000"))
            .ReturnsAsync(false);

        var handler = CreateHandler();
        var result = await handler.Handle(new VerifyRegistrationOtp { Hash = userId, Code = "000000" }, CancellationToken.None);

        result.Should().BeFalse();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("inválido ou expirado"));
        _userManagerMock.Verify(m => m.MarkEmailVerifiedAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExpiredCode_AddsNotificationAndReturnsFalse()
    {
        var userId = Guid.NewGuid();

        _emailOtpManagerMock
            .Setup(m => m.VerifyCodeOnlyAsync(userId, It.IsAny<string>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();
        var result = await handler.Handle(new VerifyRegistrationOtp { Hash = userId, Code = "654321" }, CancellationToken.None);

        result.Should().BeFalse();
        _notification.HasNotifications.Should().BeTrue();
        _userManagerMock.Verify(m => m.MarkEmailVerifiedAsync(It.IsAny<Guid>()), Times.Never);
    }
}
