using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public class SendEmailOtpHandler : IRequestHandler<SendEmailOtp, bool>
    {
        private readonly IEmailOtpManager _emailOtpManager;
        private readonly NotificationContext _notification;

        public SendEmailOtpHandler(IEmailOtpManager emailOtpManager, NotificationContext notification)
        {
            _emailOtpManager = emailOtpManager;
            _notification = notification;
        }

        public async Task<bool> Handle(SendEmailOtp request, CancellationToken cancellationToken)
        {
            var result = await _emailOtpManager.SendAsync(request.Hash);
            if (!result)
                _notification.AddNotification("hash", "Usuário não encontrado");
            return result;
        }
    }
}
