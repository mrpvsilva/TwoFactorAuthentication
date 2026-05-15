using WebApplication.Data;
using WebApplication.Notifications;
using Microsoft.EntityFrameworkCore;
using MediatR;
using WebApplication.Managers;
using WebApplication.Validators;
using WebApplication.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Register
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<NotificationContext>();
            services.AddDbContext<TfaContext>(x => x.UseSqlite("Data Source = tfa.db"));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<WebApplication.Startup>());
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<RegisterUserValidator>();
            services.AddScoped<AuthValidator>();

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
