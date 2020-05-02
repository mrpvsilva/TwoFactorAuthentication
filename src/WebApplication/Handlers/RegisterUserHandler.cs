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
    public class RegisterUserHandler : RequestHandler<RegisterUser, User>
    {
        private readonly IUserManager _manager;
        private readonly RegisterUserValidator _validator;
        public RegisterUserHandler(
            NotificationContext notification,
            IUserManager manager,
            RegisterUserValidator validator) : base(notification)
        {
            _manager = manager;
            _validator = validator;
        }

        public async override Task<User> Handle(RegisterUser request, CancellationToken cancellationToken)
        {
            var user = Mapper.Map(request).ToANew<User>();

            var result = await _validator.ValidateAsync(user);

            if (!result.IsValid)
            {
                Notification.AddNotifications(result);
                return default;
            }

            await _manager.AddUserAsync(user);

            return user;

        }
    }
}
