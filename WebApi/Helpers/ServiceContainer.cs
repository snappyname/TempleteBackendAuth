using Application.Abstract;
using Application.Abstract.Auth;
using Application.Abstract.SignalR;
using Application.Abstract.Users;
using Application.Services.Auth;
using Application.Services.SignalR;
using Application.Services.SignalR.Extensions;
using Application.Services.Users;
using Microsoft.AspNetCore.SignalR;

namespace TemplateWebApi.Helpers;

public static class ServiceContainer
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddScopedServices()
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IGithubAuthService, GithubAuthService>();
            services.AddScoped<IEmailAuthService, EmailAuthService>();
            services.AddScoped<IBroadcastService, BroadcastService>();
            return services;
        }

        public IServiceCollection AddSingletonServices()
        {
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            return services;
        }
    }
}
