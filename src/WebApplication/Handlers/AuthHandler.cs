using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Validators;

namespace WebApplication.Handlers
{
    public class AuthHandler : RequestHandler<Account, TwoFactAuth>
    {

        private readonly AuthValidator _validator;

        public AuthHandler(
            NotificationContext notification,
            IUserManager userManager,
           AuthValidator validator) : base(notification, userManager)
        {
            _validator = validator;
        }

        public async override Task<TwoFactAuth> Handle(Account request, CancellationToken cancellationToken)
        {

            var result = await _validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                Notification.AddNotifications(result);
                return default;
            }

            return await UserManager.GetTwoFactAuthAsync(request.Email);            

        }
    }
}
