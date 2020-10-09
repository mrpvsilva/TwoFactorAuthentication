using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplication.Managers;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        protected readonly NotificationContext Notification;
        protected readonly IUserManager UserManager;


        protected RequestHandler(
            NotificationContext notification,
            IUserManager userManager)
        {
            Notification = notification;
            UserManager = userManager;
        }
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
