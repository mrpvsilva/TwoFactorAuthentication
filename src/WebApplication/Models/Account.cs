using MediatR;

namespace WebApplication.Models
{
    public class Account : IRequest<TwoFactAuth>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
}
