using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Web;
using OtpNet;
using WebApplication.Models;
using WebApplication.Jwt;
using System.Text;
using System.Text.Encodings.Web;

namespace WebApplication.Managers
{
    public class UserManager : IUserManager
    {
        private readonly TfaContext _ctx;
        private readonly IJwtService _jwtService;
        private readonly UrlEncoder _urlEncoder;

        public UserManager(TfaContext ctx, IJwtService jwtService, UrlEncoder urlEncoder)
        {
            _ctx = ctx;
            _jwtService = jwtService;
            _urlEncoder = urlEncoder;
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
            string unformattedKey = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey());

            return new TwoFactAuth
            {
                AuthenticatorUri = GenerateQrCodeUri(user, unformattedKey),
                HasTwoFactorAuth = user.HasTwoFactorAuth,
                Hash = user.Id,
                SharedKey = FormatKey(user, unformattedKey)
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

            if (!VerifyTotp(secret, code))
                return default;

            var user = await _ctx.Users.FindAsync(id);
            user.Key = secret;
            await _ctx.SaveChangesAsync();

            return new Auth { AccessToken = _jwtService.GenerateToken(user) };
        }

        public async Task<Auth> VerifyCodeAsync(Guid id, string code)
        {
            var user = await _ctx.Users.FindAsync(id);

            if (!VerifyTotp(user.Key, code))
                return default;

            return new Auth { AccessToken = _jwtService.GenerateToken(user) };
        }

        private bool VerifyTotp(string secret, string code) =>
            new Totp(Base32Encoding.ToBytes(secret)).VerifyTotp(code, out _);

        private string GenerateQrCodeUri(User user, string unformattedKey)
        {
            if (user == null || user.HasTwoFactorAuth)
                return default;

            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                _urlEncoder.Encode("TwoFactAuth"),
                _urlEncoder.Encode(user.Email),
                unformattedKey);
        }

        private string FormatKey(User user, string unformattedKey)
        {
            if (user == null || user.HasTwoFactorAuth)
                return default;

            var result = new StringBuilder();
            int pos = 0;
            while (pos + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey, pos, 4).Append(' ');
                pos += 4;
            }
            if (pos < unformattedKey.Length)
                result.Append(unformattedKey, pos, unformattedKey.Length - pos);

            return result.ToString().ToLowerInvariant();
        }
    }
}
