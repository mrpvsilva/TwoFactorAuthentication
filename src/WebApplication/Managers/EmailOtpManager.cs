using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Entities;
using WebApplication.Jwt;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Managers
{
    public class EmailOtpManager : IEmailOtpManager
    {
        private readonly TfaContext _ctx;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;

        public EmailOtpManager(TfaContext ctx, IEmailService emailService, IJwtService jwtService)
        {
            _ctx = ctx;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        public async Task<bool> SendAsync(Guid userId)
        {
            var user = await _ctx.Users.FindAsync(userId);
            if (user == null) return false;

            var existing = await _ctx.EmailOtpCodes
                .Where(x => x.UserId == userId && !x.IsUsed)
                .ToListAsync();
            foreach (var e in existing)
                e.IsUsed = true;

            var code = (100000 + (BitConverter.ToUInt32(RandomNumberGenerator.GetBytes(4)) % 900000)).ToString();

            await _ctx.EmailOtpCodes.AddAsync(new EmailOtpCode
            {
                UserId = userId,
                Code = BCrypt.Net.BCrypt.HashPassword(code),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            });

            await _ctx.SaveChangesAsync();

            await _emailService.SendAsync(
                user.Email,
                "Código de verificação",
                $"<p>Seu código de verificação é: <strong>{code}</strong></p><p>Este código expira em 10 minutos.</p>"
            );

            return true;
        }

        public async Task<Auth> VerifyAsync(Guid userId, string code)
        {
            var otp = await _ctx.EmailOtpCodes
                .Include(x => x.User)
                .Where(x => x.UserId == userId && !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.ExpiresAt)
                .FirstOrDefaultAsync();

            if (otp == null || !BCrypt.Net.BCrypt.Verify(code, otp.Code))
                return default;

            otp.IsUsed = true;
            await _ctx.SaveChangesAsync();

            var refreshToken = await GenerateRefreshTokenAsync(userId);
            return new Auth { AccessToken = _jwtService.GenerateToken(otp.User), RefreshToken = refreshToken };
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
    }
}
