using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public class VerifyEmailOtpHandler : IRequestHandler<VerifyEmailOtp, Auth>
    {
        private readonly IEmailOtpManager _emailOtpManager;
        private readonly NotificationContext _notification;

        public VerifyEmailOtpHandler(IEmailOtpManager emailOtpManager, NotificationContext notification)
        {
            _emailOtpManager = emailOtpManager;
            _notification = notification;
        }

        public async Task<Auth> Handle(VerifyEmailOtp request, CancellationToken cancellationToken)
        {
            var auth = await _emailOtpManager.VerifyAsync(request.Hash, request.Code);
            if (auth == null)
                _notification.AddNotification("code", "Código inválido ou expirado. Tente novamente");
            return auth;
        }
    }
}
