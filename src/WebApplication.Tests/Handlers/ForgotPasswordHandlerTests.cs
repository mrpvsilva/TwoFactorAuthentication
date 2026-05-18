using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using WebApplication.Data;
using WebApplication.Entities;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Services;
using WebApplication.Validators;

namespace WebApplication.Tests.Handlers;

public class ForgotPasswordHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly NotificationContext _notification;
    private readonly TfaContext _context;

    public ForgotPasswordHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _emailServiceMock = new Mock<IEmailService>();
        _notification = new NotificationContext();

        var options = new DbContextOptionsBuilder<TfaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new TfaContext(options);
    }

    private ForgotPasswordHandler CreateHandler()
    {
        var validator = new ForgotPasswordValidator(_context);
        return new ForgotPasswordHandler(_notification, _userManagerMock.Object, validator, _emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingEmail_SendsEmailAndReturnsTrue()
    {
        _context.Users.Add(new User { Id = Guid.NewGuid(), Email = "user@test.com", Password = "hashed" });
        await _context.SaveChangesAsync();

        _userManagerMock
            .Setup(m => m.GeneratePasswordResetCodeAsync("user@test.com"))
            .ReturnsAsync("123456");

        var handler = CreateHandler();
        var result = await handler.Handle(new ForgotPassword { Email = "user@test.com" }, CancellationToken.None);

        result.Should().BeTrue();
        _notification.HasNotifications.Should().BeFalse();
        _emailServiceMock.Verify(
            e => e.SendAsync("user@test.com", "Password Reset Code", It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentEmail_AddsNotificationAndReturnsFalse()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new ForgotPassword { Email = "ghost@test.com" }, CancellationToken.None);

        result.Should().Be(default(bool));
        _notification.Notifications.Should().Contain(n => n.Message.Contains("No account found with this e-mail"));
        _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyEmail_AddsNotificationAndReturnsFalse()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new ForgotPassword { Email = "" }, CancellationToken.None);

        result.Should().Be(default(bool));
        _notification.Notifications.Should().Contain(n => n.Message.Contains("E-mail is required"));
    }

    [Fact]
    public async Task Handle_WhenCodeIsNull_DoesNotSendEmail()
    {
        _context.Users.Add(new User { Id = Guid.NewGuid(), Email = "user@test.com", Password = "hashed" });
        await _context.SaveChangesAsync();

        _userManagerMock
            .Setup(m => m.GeneratePasswordResetCodeAsync("user@test.com"))
            .ReturnsAsync((string?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(new ForgotPassword { Email = "user@test.com" }, CancellationToken.None);

        result.Should().BeTrue();
        _emailServiceMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
