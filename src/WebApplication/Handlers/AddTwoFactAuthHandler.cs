using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public class AddTwoFactAuthHandler : RequestHandler<AddTwoFactAuth, Auth>
    {
        public AddTwoFactAuthHandler(NotificationContext notification, IUserManager userManager) : base(notification, userManager)
        {
        }

        public async override Task<Auth> Handle(AddTwoFactAuth request, CancellationToken cancellationToken)
        {
            var auth = await UserManager.AddTwoFactorTokenAsync(request.Hash, request.AuthenticatorUri, request.Code);

            if (auth == null)
            {
                Notification.AddNotification("code", "Code invalid");
            }

            return auth;

        }
    }
}
