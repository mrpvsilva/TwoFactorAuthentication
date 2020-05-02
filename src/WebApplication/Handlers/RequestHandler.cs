using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        protected readonly NotificationContext Notification;


        public RequestHandler(NotificationContext notification)
        {
            Notification = notification;
        }
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
