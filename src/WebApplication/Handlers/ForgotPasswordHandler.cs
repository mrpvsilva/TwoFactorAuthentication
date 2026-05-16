using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Services;
using WebApplication.Validators;

namespace WebApplication.Handlers
{
    public class ForgotPasswordHandler : RequestHandler<ForgotPassword, bool>
    {
        private readonly ForgotPasswordValidator _validator;
        private readonly IEmailService _emailService;

        public ForgotPasswordHandler(
            NotificationContext notification,
            IUserManager userManager,
            ForgotPasswordValidator validator,
            IEmailService emailService) : base(notification, userManager)
        {
            _validator = validator;
            _emailService = emailService;
        }

        public async override Task<bool> Handle(ForgotPassword request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                Notification.AddNotifications(result);
                return default;
            }

            var code = await UserManager.GeneratePasswordResetCodeAsync(request.Email);

            await _emailService.SendAsync(
                request.Email,
                "Password Reset Code",
                BuildEmailBody(code));

            return true;
        }

        private static string BuildEmailBody(string code) => $@"
<!DOCTYPE html>
<html>
<body style=""font-family:Arial,sans-serif;background:#f4f4f4;padding:20px;"">
  <div style=""max-width:420px;margin:0 auto;background:#fff;padding:32px;border-radius:8px;"">
    <h2 style=""color:#333;margin-top:0;"">Password Reset</h2>
    <p style=""color:#555;"">Use the code below to reset your password. It expires in <strong>15 minutes</strong>.</p>
    <div style=""font-size:36px;font-weight:bold;text-align:center;letter-spacing:10px;padding:20px;background:#f0f4ff;border-radius:6px;color:#2563eb;"">
      {code}
    </div>
    <p style=""color:#888;font-size:13px;margin-bottom:0;"">If you didn't request this, you can safely ignore this email.</p>
  </div>
</body>
</html>";
    }
}
