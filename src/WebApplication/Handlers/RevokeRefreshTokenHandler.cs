using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplication.Managers;
using WebApplication.Models;

namespace WebApplication.Handlers
{
    public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshToken, bool>
    {
        private readonly IUserManager _userManager;

        public RevokeRefreshTokenHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(RevokeRefreshToken request, CancellationToken cancellationToken)
        {
            await _userManager.RevokeRefreshTokenAsync(request.Token);
            return true;
        }
    }
}
