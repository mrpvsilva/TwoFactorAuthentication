using MediatR;

namespace WebApplication.Models
{
    public class ResendRegistrationOtp : IRequest<RegisterResult>
    {
        public string Email { get; set; }
    }
}
