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
using WebApplication.Validators;

namespace WebApplication.Tests.Handlers;

public class RegisterUserHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly NotificationContext _notification;
    private readonly TfaContext _context;

    public RegisterUserHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _notification = new NotificationContext();

        var options = new DbContextOptionsBuilder<TfaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new TfaContext(options);
    }

    private RegisterUserHandler CreateHandler()
    {
        var validator = new RegisterUserValidator(_context);
        return new RegisterUserHandler(_notification, _userManagerMock.Object, validator);
    }

    [Fact]
    public async Task Handle_ValidNewUser_CreatesAndReturnsUser()
    {
        _userManagerMock
            .Setup(m => m.AddUserAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var handler = CreateHandler();
        var result = await handler.Handle(new RegisterUser { Email = "new@test.com", Password = "Password1!" }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Email.Should().Be("new@test.com");
        _notification.HasNotifications.Should().BeFalse();
        _userManagerMock.Verify(m => m.AddUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_AddsNotificationAndReturnsNull()
    {
        _context.Users.Add(new User { Id = Guid.NewGuid(), Email = "exists@test.com", Password = "hashed" });
        await _context.SaveChangesAsync();

        var handler = CreateHandler();
        var result = await handler.Handle(new RegisterUser { Email = "exists@test.com", Password = "Password1!" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("already registered"));
        _userManagerMock.Verify(m => m.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyEmail_AddsNotificationAndReturnsNull()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new RegisterUser { Email = "", Password = "Password1!" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("E-mail is required"));
    }

    [Fact]
    public async Task Handle_InvalidEmailFormat_AddsNotificationAndReturnsNull()
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new RegisterUser { Email = "bademail", Password = "Password1!" }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains("E-mail format is invalid"));
    }

    [Theory]
    [InlineData("short1!", "Password must be at least 8 characters")]
    [InlineData("nouppercase1!", "Password must contain at least one uppercase letter")]
    [InlineData("NOLOWERCASE1!", "Password must contain at least one lowercase letter")]
    [InlineData("NoDigitsHere!", "Password must contain at least one number")]
    [InlineData("NoSpecial1chars", "Password must contain at least one special character")]
    public async Task Handle_WeakPassword_AddsNotificationAndReturnsNull(string password, string expectedMessage)
    {
        var handler = CreateHandler();
        var result = await handler.Handle(new RegisterUser { Email = "user@test.com", Password = password }, CancellationToken.None);

        result.Should().BeNull();
        _notification.Notifications.Should().Contain(n => n.Message.Contains(expectedMessage));
    }
}
