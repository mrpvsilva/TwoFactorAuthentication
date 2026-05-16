using MediatR;

namespace WebApplication.Models
{
    public class RefreshTokenRequest : IRequest<Auth>
    {
        public string Token { get; set; }
    }
}
