using MediatR;

namespace WebApplication.Models
{
    public class ForgotPassword : IRequest<bool>
    {
        public string Email { get; set; }
    }
}
