using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Notifications;

namespace WebApplication.Handlers
{
    public class VerifyCodeHandler : RequestHandler<VerifyCode, Auth>
    {
        public VerifyCodeHandler(NotificationContext notification, IUserManager userManager) : base(notification, userManager)
        {
        }

        public async override Task<Auth> Handle(VerifyCode request, CancellationToken cancellationToken)
        {
            return await UserManager.VerifyCodeAsync(request.Hash, request.Code);
        }
    }
}
