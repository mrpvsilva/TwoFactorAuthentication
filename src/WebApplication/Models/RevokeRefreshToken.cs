using MediatR;

namespace WebApplication.Models
{
    public class RevokeRefreshToken : IRequest<bool>
    {
        public string Token { get; set; }
    }
}
