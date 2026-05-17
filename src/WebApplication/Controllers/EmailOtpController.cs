using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailOtpController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailOtpController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendEmailOtp request)
        {
            var result = await _mediator.Send(request);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyEmailOtp request)
        {
            var auth = await _mediator.Send(request);
            if (auth == null) return BadRequest();

            SetRefreshTokenCookie(auth.RefreshToken);
            return Ok(new { auth.AccessToken });
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
    }
}
