using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting(RateLimitingExtensions.AuthLogin)]
        public async Task<IActionResult> Auth([FromBody] Account account)
        {
            return Ok(await _mediator.Send(account));
        }

        [HttpPost("AddTwoFactAuth")]
        public async Task<IActionResult> AddTwoFactAuth([FromBody] AddTwoFactAuth addTwoFactAuth)
        {
            var auth = await _mediator.Send(addTwoFactAuth);
            if (auth == null) return Ok(null);

            SetRefreshTokenCookie(auth.RefreshToken);
            return Ok(new { auth.AccessToken });
        }

        [HttpPost("VerifyCode")]
        [EnableRateLimiting(RateLimitingExtensions.Auth2Fa)]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCode verify)
        {
            var auth = await _mediator.Send(verify);
            if (auth == null) return Ok(null);

            SetRefreshTokenCookie(auth.RefreshToken);
            return Ok(new { auth.AccessToken });
        }

        [HttpPost("refresh")]
        [EnableRateLimiting(RateLimitingExtensions.AuthRefresh)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var auth = await _mediator.Send(new RefreshTokenRequest { Token = refreshToken });
            if (auth == null)
            {
                ClearRefreshTokenCookie();
                return Unauthorized();
            }

            SetRefreshTokenCookie(auth.RefreshToken);
            return Ok(new { auth.AccessToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
                await _mediator.Send(new RevokeRefreshToken { Token = refreshToken });

            ClearRefreshTokenCookie();
            return NoContent();
        }

        [HttpDelete("totp")]
        [Authorize]
        [EnableRateLimiting(RateLimitingExtensions.AuthResetTotp)]
        public async Task<IActionResult> ResetTotp()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            return Ok(await _mediator.Send(new ResetTwoFactAuth { UserId = userId }));
        }

        private void SetRefreshTokenCookie(string token)
        {
            Response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(7)
            });
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            });
        }
    }
}
