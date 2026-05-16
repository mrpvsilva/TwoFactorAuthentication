using FluentAssertions;
using Moq;
using Xunit;
using WebApplication.Handlers;
using WebApplication.Managers;
using WebApplication.Models;

namespace WebApplication.Tests.Handlers;

public class RefreshTokenHandlerTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _handler = new RefreshTokenHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidToken_ReturnsNewAuth()
    {
        var expected = new Auth { AccessToken = "new-access-token", RefreshToken = "new-refresh-token" };

        _userManagerMock
            .Setup(m => m.RefreshAccessTokenAsync("valid-refresh-token"))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new RefreshTokenRequest { Token = "valid-refresh-token" }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("new-access-token");
    }

    [Fact]
    public async Task Handle_ExpiredToken_ReturnsNull()
    {
        _userManagerMock
            .Setup(m => m.RefreshAccessTokenAsync("expired-token"))
            .ReturnsAsync((Auth?)null);

        var result = await _handler.Handle(new RefreshTokenRequest { Token = "expired-token" }, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_RevokedToken_ReturnsNull()
    {
        _userManagerMock
            .Setup(m => m.RefreshAccessTokenAsync("revoked-token"))
            .ReturnsAsync((Auth?)null);

        var result = await _handler.Handle(new RefreshTokenRequest { Token = "revoked-token" }, CancellationToken.None);

        result.Should().BeNull();
    }
}
