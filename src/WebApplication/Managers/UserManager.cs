using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
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
            var unverified = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == user.Email && !x.EmailVerified);
            if (unverified != null) _ctx.Users.Remove(unverified);

            user.HashPassword();
            await _ctx.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUnverifiedByEmailAsync(string email) =>
            await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email && !x.EmailVerified);

        public async Task<bool> MarkEmailVerifiedAsync(Guid id)
        {
            var user = await _ctx.Users.FindAsync(id);
            if (user == null) return false;

            user.EmailVerified = true;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<TwoFactAuth> GetTwoFactAuthAsync(string email)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return null;

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
            if (user == null) return default;

            user.Key = secret;
            await _ctx.SaveChangesAsync();

            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            return new Auth { AccessToken = _jwtService.GenerateToken(user), RefreshToken = refreshToken };
        }

        public async Task<Auth> VerifyCodeAsync(Guid id, string code)
        {
            var user = await _ctx.Users.FindAsync(id);

            if (user == null || !VerifyTotp(user.Key, code))
                return default;

            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            return new Auth { AccessToken = _jwtService.GenerateToken(user), RefreshToken = refreshToken };
        }

        public async Task<Auth> RefreshAccessTokenAsync(string refreshToken)
        {
            var stored = await _ctx.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (stored == null || !stored.IsActive)
                return default;

            stored.IsRevoked = true;

            var newRefreshToken = await GenerateRefreshTokenAsync(stored.UserId);

            return new Auth
            {
                AccessToken = _jwtService.GenerateToken(stored.User),
                RefreshToken = newRefreshToken
            };
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var stored = await _ctx.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (stored == null) return;

            stored.IsRevoked = true;
            await _ctx.SaveChangesAsync();
        }

        private async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var expired = await _ctx.RefreshTokens
                .Where(x => x.UserId == userId && (x.IsRevoked || x.ExpiresAt < DateTime.UtcNow))
                .ToListAsync();
            _ctx.RefreshTokens.RemoveRange(expired);

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            await _ctx.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _ctx.SaveChangesAsync();

            return token;
        }

        public async Task<string> GeneratePasswordResetCodeAsync(string email)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return null;

            var code = (100000 + (BitConverter.ToUInt32(RandomNumberGenerator.GetBytes(4)) % 900000)).ToString();
            user.PasswordResetCode = BCrypt.Net.BCrypt.HashPassword(code);
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(15);
            await _ctx.SaveChangesAsync();

            return code;
        }

        public async Task<bool> VerifyPasswordResetCodeAsync(string email, string code)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null || user.PasswordResetExpiry == null) return false;
            if (DateTime.UtcNow > user.PasswordResetExpiry) return false;

            return BCrypt.Net.BCrypt.Verify(code, user.PasswordResetCode);
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null || user.PasswordResetExpiry == null) return false;
            if (DateTime.UtcNow > user.PasswordResetExpiry) return false;
            if (!BCrypt.Net.BCrypt.Verify(code, user.PasswordResetCode)) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetCode = null;
            user.PasswordResetExpiry = null;
            await _ctx.SaveChangesAsync();

            return true;
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
