using System;
using System.IO;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Notifications;
using WebApplication.Behaviors;
using WebApplication.Managers;
using WebApplication.Models;
using WebApplication.Validators;
using WebApplication.Jwt;
using WebApplication.Services;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class Register
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<NotificationContext>();
            services.AddDbContext<TfaContext>(x =>
                x.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<WebApplication.Startup>());
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IEmailOtpManager, EmailOtpManager>();
            services.AddScoped(typeof(IPipelineBehavior<Account, TwoFactAuth>), typeof(PostLoginEmailOtpBehavior));
            services.AddScoped<RegisterUserValidator>();
            services.AddScoped<AuthValidator>();
            services.AddScoped<ForgotPasswordValidator>();
            services.AddScoped<VerifyResetCodeValidator>();
            services.AddScoped<ResetPasswordValidator>();

            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.AddHttpClient();
            services.AddScoped<IEmailService, EmailService>();

            services.AddJwtAuthentication(configuration);
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("TokenConfigurations");
            var tokenConfig = section.Get<TokenConfigurations>();

            services.Configure<TokenConfigurations>(section);

            var keyPath = Path.IsPathRooted(tokenConfig.PrivateKeyPath)
                ? tokenConfig.PrivateKeyPath
                : Path.Combine(Directory.GetCurrentDirectory(), tokenConfig.PrivateKeyPath);

            var privateKeyPem = File.ReadAllText(keyPath);
            var signingConfig = new SigningConfigurations(privateKeyPem);
            services.AddSingleton(signingConfig);
            services.AddScoped<IJwtService, JwtService>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingConfig.Key,
                        ValidateIssuer = true,
                        ValidIssuer = tokenConfig.Issuer,
                        ValidateAudience = true,
                        ValidAudience = tokenConfig.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}
