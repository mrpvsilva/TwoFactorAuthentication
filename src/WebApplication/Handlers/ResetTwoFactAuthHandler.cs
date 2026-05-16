using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public class ResetTwoFactAuthHandler : RequestHandler<ResetTwoFactAuth, bool>
    {
        public ResetTwoFactAuthHandler(NotificationContext notification, IUserManager userManager)
            : base(notification, userManager)
        {
        }

        public override async Task<bool> Handle(ResetTwoFactAuth request, CancellationToken cancellationToken)
        {
            var result = await UserManager.ResetTwoFactAuthAsync(request.UserId);

            if (!result)
                Notification.AddNotification("totp", "Não foi possível resetar o código TOTP.");

            return result;
        }
    }
}
