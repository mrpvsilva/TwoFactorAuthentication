using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Entities;

namespace WebApplication.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly TokenConfigurations _tokenConfig;
        private readonly SigningConfigurations _signingConfig;

        public JwtService(IOptions<TokenConfigurations> tokenConfig, SigningConfigurations signingConfig)
        {
            _tokenConfig = tokenConfig.Value;
            _signingConfig = signingConfig;
        }

        public string GenerateToken(User user)
        {
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _tokenConfig.Issuer,
                Audience = _tokenConfig.Audience,
                Subject = new ClaimsIdentity(claims),
                NotBefore = now,
                IssuedAt = now,
                Expires = now.AddSeconds(_tokenConfig.Seconds),
                SigningCredentials = _signingConfig.SigningCredentials
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(descriptor));
        }
    }
}
