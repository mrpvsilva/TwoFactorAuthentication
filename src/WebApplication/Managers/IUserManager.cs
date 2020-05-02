using System;
using System.Threading.Tasks;
using WebApplication.Entities;
using WebApplication.Models;

namespace WebApplication.Managers
{
    public interface IUserManager
    {
        Task<User> AddUserAsync(User user);
        Task<User> PasswordSignInAsync(string email, string password);
        Task<TwoFactAuth> GetTwoFactAuthAsync(string email);
        Task<Auth> AddTwoFactorTokenAsync(Guid id, string AuthenticatorUri, string code);
        Task<Auth> VerifyCodeAsync(Guid id, string code);
    }
}
