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
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting(RateLimitingExtensions.AccountRegister)]
        public async Task<IActionResult> Register([FromBody] RegisterUser register)
        {
            var result = await _mediator.Send(register);
            if (result == null) return BadRequest();
            return Ok(new { result.Hash });
        }

        [HttpPost("verify-email")]
        [EnableRateLimiting(RateLimitingExtensions.AccountVerifyEmail)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyRegistrationOtp request)
        {
            var success = await _mediator.Send(request);
            return Ok(new { success });
        }

        [HttpPost("resend-verification")]
        [EnableRateLimiting(RateLimitingExtensions.AccountVerifyEmail)]
        public async Task<IActionResult> ResendVerification([FromBody] ResendRegistrationOtp request)
        {
            var result = await _mediator.Send(request);
            if (result == null) return BadRequest();
            return Ok(new { result.Hash });
        }
    }
}
