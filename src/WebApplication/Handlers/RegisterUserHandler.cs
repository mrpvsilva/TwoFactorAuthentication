using System.Threading;
using System.Threading.Tasks;
using AgileObjects.AgileMapper;
using WebApplication.Entities;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;
using WebApplication.Validators;

namespace WebApplication.Handlers
{
    public class RegisterUserHandler : RequestHandler<RegisterUser, RegisterResult>
    {
        private readonly RegisterUserValidator _validator;
        private readonly IEmailOtpManager _emailOtpManager;

        public RegisterUserHandler(
            NotificationContext notification,
            IUserManager userManager,
            IEmailOtpManager emailOtpManager,
            RegisterUserValidator validator) : base(notification, userManager)
        {
            _emailOtpManager = emailOtpManager;
            _validator = validator;
        }

        public async override Task<RegisterResult> Handle(RegisterUser request, CancellationToken cancellationToken)
        {
            var user = Mapper.Map(request).ToANew<User>();

            var result = await _validator.ValidateAsync(user);

            if (!result.IsValid)
            {
                Notification.AddNotifications(result);
                return default;
            }

            await UserManager.AddUserAsync(user);
            await _emailOtpManager.SendAsync(user.Id);

            return new RegisterResult { Hash = user.Id };
        }
    }
}
