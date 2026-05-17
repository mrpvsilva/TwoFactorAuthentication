using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public class VerifyRegistrationOtpHandler : RequestHandler<VerifyRegistrationOtp, bool>
    {
        private readonly IEmailOtpManager _emailOtpManager;

        public VerifyRegistrationOtpHandler(
            NotificationContext notification,
            IUserManager userManager,
            IEmailOtpManager emailOtpManager) : base(notification, userManager)
        {
            _emailOtpManager = emailOtpManager;
        }

        public async override Task<bool> Handle(VerifyRegistrationOtp request, CancellationToken cancellationToken)
        {
            var valid = await _emailOtpManager.VerifyCodeOnlyAsync(request.Hash, request.Code);

            if (!valid)
            {
                Notification.AddNotification("Code", "Código inválido ou expirado");
                return false;
            }

            await UserManager.MarkEmailVerifiedAsync(request.Hash);
            return true;
        }
    }
}
