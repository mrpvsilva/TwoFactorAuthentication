using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Validators;

namespace WebApplication.Handlers
{
    public class VerifyResetCodeHandler : RequestHandler<VerifyResetCode, bool>
    {
        private readonly VerifyResetCodeValidator _validator;

        public VerifyResetCodeHandler(
            NotificationContext notification,
            IUserManager userManager,
            VerifyResetCodeValidator validator) : base(notification, userManager)
        {
            _validator = validator;
        }

        public async override Task<bool> Handle(VerifyResetCode request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                Notification.AddNotifications(result);
                return default;
            }

            var valid = await UserManager.VerifyPasswordResetCodeAsync(request.Email, request.Code);
            if (!valid)
            {
                Notification.AddNotification("code", "Invalid or expired code. Please request a new one");
                return default;
            }

            return true;
        }
    }
}
