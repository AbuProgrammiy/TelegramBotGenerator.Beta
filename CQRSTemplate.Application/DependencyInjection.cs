using CQRSTemplate.Application.Services.AuthServices;
using CQRSTemplate.Application.Services.EmailServices;
using CQRSTemplate.Application.Services.PasswordServices;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CQRSTemplate.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPasswordService, PasswordService>();

            return services;
        }
    }
}
