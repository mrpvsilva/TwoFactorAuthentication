using MediatR;

namespace WebApplication.Models
{
    public class ResetPassword : IRequest<bool>
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
    }
}
