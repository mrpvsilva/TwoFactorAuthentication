using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplication.Managers;
using WebApplication.Models;

namespace WebApplication.Behaviors
{
    public class PostLoginEmailOtpBehavior : IPipelineBehavior<Account, TwoFactAuth>
    {
        private readonly IEmailOtpManager _emailOtpManager;

        public PostLoginEmailOtpBehavior(IEmailOtpManager emailOtpManager)
        {
            _emailOtpManager = emailOtpManager;
        }

        public async Task<TwoFactAuth> Handle(Account request, RequestHandlerDelegate<TwoFactAuth> next, CancellationToken cancellationToken)
        {
            var result = await next();

            if (result != null && result.Hash != Guid.Empty)
                await _emailOtpManager.SendAsync(result.Hash);

            return result;
        }
    }
}
