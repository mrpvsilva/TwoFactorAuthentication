using MediatR;

namespace WebApplication.Models
{
    public class RegisterUser : IRequest<RegisterResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
