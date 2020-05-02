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
using System;
using Microsoft.AspNetCore.Authorization;

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

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }
    }
}
