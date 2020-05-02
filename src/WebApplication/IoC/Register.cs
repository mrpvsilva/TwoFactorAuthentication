using WebApplication.Data;
using WebApplication.Notifications;
using Microsoft.EntityFrameworkCore;
using MediatR;
using WebApplication.Managers;
using WebApplication.Validators;

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
        }
    }
}
