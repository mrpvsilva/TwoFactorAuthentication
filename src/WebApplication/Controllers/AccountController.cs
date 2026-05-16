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
            var user = await _mediator.Send(register);
            return Ok(new { user?.Id, user?.Email });
        }
    }
}