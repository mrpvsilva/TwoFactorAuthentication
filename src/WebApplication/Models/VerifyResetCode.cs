using MediatR;

namespace WebApplication.Models
{
    public class VerifyResetCode : IRequest<bool>
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
