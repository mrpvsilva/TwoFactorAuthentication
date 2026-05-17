using System;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Managers
{
    public interface IEmailOtpManager
    {
        Task<bool> SendAsync(Guid userId);
        Task<Auth> VerifyAsync(Guid userId, string code);
    }
}
