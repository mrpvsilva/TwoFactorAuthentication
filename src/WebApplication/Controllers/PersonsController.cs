using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bogus;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.Run(() =>
            {
                var data = new Faker<Person>()
                   .RuleFor(x => x.Id, f => f.Random.Guid())
                   .RuleFor(c => c.FirstName, f => f.Person.FirstName)
                   .RuleFor(c => c.Email, f => f.Person.Email)
                   .Generate(50);

                return Ok(data);
            });
        }
    }

    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
    }
}