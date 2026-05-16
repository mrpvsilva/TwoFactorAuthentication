using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Validators;

namespace WebApplication.Handlers
{
    public class ResetPasswordHandler : RequestHandler<ResetPassword, bool>
    {
        private readonly ResetPasswordValidator _validator;

        public ResetPasswordHandler(
            NotificationContext notification,
            IUserManager userManager,
            ResetPasswordValidator validator) : base(notification, userManager)
        {
            _validator = validator;
        }

        public async override Task<bool> Handle(ResetPassword request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                Notification.AddNotifications(result);
                return default;
            }

            var success = await UserManager.ResetPasswordAsync(request.Email, request.Code, request.Password);
            if (!success)
            {
                Notification.AddNotification("code", "Invalid or expired code. Please request a new one");
                return default;
            }

            return true;
        }
    }
}
