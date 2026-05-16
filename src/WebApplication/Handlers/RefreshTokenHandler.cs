using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplication.Managers;
using WebApplication.Models;

namespace WebApplication.Handlers
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, Auth>
    {
        private readonly IUserManager _userManager;

        public RefreshTokenHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Auth> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            return await _userManager.RefreshAccessTokenAsync(request.Token);
        }
    }
}
