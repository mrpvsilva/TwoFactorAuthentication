using MediatR;
using WebApplication.Entities;

namespace WebApplication.Models
{
    public class RegisterUser : IRequest<User>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
