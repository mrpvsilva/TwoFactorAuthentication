using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PasswordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("forgot")]
        [EnableRateLimiting(RateLimitingExtensions.AuthForgotPassword)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("verify-code")]
        [EnableRateLimiting(RateLimitingExtensions.AuthResetPassword)]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyResetCode request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("reset")]
        [EnableRateLimiting(RateLimitingExtensions.AuthResetPassword)]
        public async Task<IActionResult> Reset([FromBody] ResetPassword request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
