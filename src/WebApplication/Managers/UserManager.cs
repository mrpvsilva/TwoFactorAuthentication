using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Web;
using OtpNet;
using WebApplication.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Jwt;
using System.Text;
using System.Text.Encodings.Web;

namespace WebApplication.Managers
{
    public class UserManager : IUserManager
    {
        private readonly TfaContext _ctx;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly UrlEncoder _urlEncoder;

        public UserManager(
            TfaContext ctx,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            UrlEncoder urlEncoder
            )
        {
            _ctx = ctx;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
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


            if (VerifyTotp(secret, code))
            {
                var user = await _ctx.Users.FindAsync(id);
                user.Key = secret;
                await _ctx.SaveChangesAsync();
                return new Auth { AccessToken = WriteToken(user) };
            }

            return default;

        }

        public async Task<Auth> VerifyCodeAsync(Guid id, string code)
        {

            var user = await _ctx.Users.FindAsync(id);

            if (VerifyTotp(user.Key, code))
            {
                return new Auth { AccessToken = WriteToken(user) };
            }

            return default;
        }

        private bool VerifyTotp(string secret, string code)
        {
            return new Totp(Base32Encoding.ToBytes(secret)).VerifyTotp(code, out long timeStepMatched);
        }

        private string WriteToken(User user)
        {
            ClaimsIdentity identity = new ClaimsIdentity(

                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString())
                    }
                );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao +
                TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });

            return handler.WriteToken(securityToken);
        }

        private string GenerateQrCodeUri(User user, string unformattedKey)
        {
            if (user == null || user.HasTwoFactorAuth)
                return default;

            return string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6", _urlEncoder.Encode("TwoFactAuth"), _urlEncoder.Encode(user.Email), unformattedKey);
        }

        private string FormatKey(User user, string unformattedKey)
        {
            if (user == null || user.HasTwoFactorAuth)
                return default;

            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }
    }
}
