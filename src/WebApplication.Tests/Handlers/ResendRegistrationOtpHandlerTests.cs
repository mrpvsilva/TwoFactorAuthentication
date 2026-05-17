using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Entities;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Tests.Handlers;

public class ResendRegistrationOtpHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly Mock<IEmailOtpManager> _emailOtpManagerMock;
    private readonly NotificationContext _notification;

    public ResendRegistrationOtpHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _emailOtpManagerMock = new Mock<IEmailOtpManager>();
        _notification = new NotificationContext();
    }

    private ResendRegistrationOtpHandler CreateHandler() =>
        new ResendRegistrationOtpHandler(_notification, _userManagerMock.Object, _emailOtpManagerMock.Object);

    [Fact]
    public async Task Handle_UnverifiedUser_SendsOtpAndReturnsHash()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "pending@test.com", EmailVerified = false };

        _userManagerMock
            .Setup(m => m.GetUnverifiedByEmailAsync("pending@test.com"))
            .ReturnsAsync(user);

        _emailOtpManagerMock
            .Setup(m => m.SendAsync(user.Id))
            .ReturnsAsync(true);

        var handler = CreateHandler();
        var result = await handler.Handle(new ResendRegistrationOtp { Email = "pending@test.com" }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Hash.Should().Be(user.Id);
        _notification.HasNotifications.Should().BeFalse();
        _emailOtpManagerMock.Verify(m => m.SendAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_NoUnverifiedUser_AddsNotificationAndReturnsNull()
    {
        _userManagerMock
            .Setup(m => m.GetUnverifiedByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(new ResendRegistrationOtp { Email = "verified@test.com" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("pendente de verificação"));
        _emailOtpManagerMock.Verify(m => m.SendAsync(It.IsAny<Guid>()), Times.Never);
    }
}
