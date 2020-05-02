using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Web;
using OtpNet;
using WebApplication.Models;

namespace WebApplication.Managers
{
    public class UserManager : IUserManager
    {
        private readonly TFAContext _ctx;

        public UserManager(TFAContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<User> AddUserAsync(User user)
        {
            user.HashPasword();
            await _ctx.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return user;
        }

        public async Task<TwoFactAuth> GetTwoFactAuthAsync(string email)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);

            return new TwoFactAuth
            {
                AuthenticatorUri = user.AuthenticatorUri,
                HasTwoFactorAuth = user.HasTwoFactorAuth,
                Hash = user.Id
            };
        }

        public async Task<User> PasswordSignInAsync(string email, string password)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);

            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : default;
        }

        public async Task<Auth> AddTwoFactorTokenAsync(Guid id, string authenticatorUri, string code)
        {

            string secret = HttpUtility.ParseQueryString(new Uri(authenticatorUri).Query).Get("secret");  

            if (VerifyTotp(secret, code))
            {
                var user = await _ctx.Users.FindAsync(id);
                user.Key = secret;
                await _ctx.SaveChangesAsync();
                return new Auth { AccessToken = "" };
            }

            return default;

        }       

        public async Task<Auth> VerifyCodeAsync(Guid id, string code)
        {

            var user = await _ctx.Users.FindAsync(id);

            if (VerifyTotp(user.Key, code))
            {                
                return new Auth { AccessToken = "" };
            }

            return default;
        }

        private bool VerifyTotp(string secret, string code)
        {
            return new Totp(Base32Encoding.ToBytes(secret)).VerifyTotp(code, out long timeStepMatched);
        }
    }


}
