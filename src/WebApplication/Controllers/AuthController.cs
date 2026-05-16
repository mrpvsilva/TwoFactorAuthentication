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
            return Ok(await _mediator.Send(addTwoFactAuth));
        }

        [HttpPost("VerifyCode")]
        [EnableRateLimiting(RateLimitingExtensions.Auth2Fa)]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCode verify)
        {
            return Ok(await _mediator.Send(verify));
        }

    }

}