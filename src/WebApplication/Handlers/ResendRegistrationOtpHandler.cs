using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public class ResendRegistrationOtpHandler : RequestHandler<ResendRegistrationOtp, RegisterResult>
    {
        private readonly IEmailOtpManager _emailOtpManager;

        public ResendRegistrationOtpHandler(
            NotificationContext notification,
            IUserManager userManager,
            IEmailOtpManager emailOtpManager) : base(notification, userManager)
        {
            _emailOtpManager = emailOtpManager;
        }

        public async override Task<RegisterResult> Handle(ResendRegistrationOtp request, CancellationToken cancellationToken)
        {
            var user = await UserManager.GetUnverifiedByEmailAsync(request.Email);

            if (user == null)
            {
                Notification.AddNotification("Email", "Nenhum cadastro pendente de verificação encontrado para este e-mail");
                return default;
            }

            await _emailOtpManager.SendAsync(user.Id);

            return new RegisterResult { Hash = user.Id };
        }
    }
}
