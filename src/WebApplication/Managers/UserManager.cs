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

namespace WebApplication.Managers
{
    public class UserManager : IUserManager
    {
        private readonly TFAContext _ctx;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;

        public UserManager(
            TFAContext ctx,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations
            )
        {
            _ctx = ctx;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
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
    }
}
