using WebApplication.Data;
using WebApplication.Notifications;
using Microsoft.EntityFrameworkCore;
using MediatR;
using WebApplication.Managers;
using WebApplication.Validators;
using WebApplication.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class Register
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<NotificationContext>();
            services.AddDbContext<TFAContext>(x => x.UseSqlite("Data Source = tfa.db"));
            services.AddMediatR(typeof(WebApplication.Startup));
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<RegisterUserValidator>();
            services.AddScoped<AuthValidator>();

            services.AddJwtBearer();

        }

        public static void AddJwtBearer(this IServiceCollection services)
        {

            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();

            new ConfigureFromConfigurationOptions<TokenConfigurations>(configuration.GetSection("TokenConfigurations")).Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = signingConfigurations.Key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = tokenConfigurations.Issuer,
                    ValidAudience = tokenConfigurations.Audience
                };
            });

        }
    }
}
