using Application.Abstract;
using Application.Services.Auth;
using Application.Services.UserAuth;
using Application.Services.Users;

namespace TemplateWebApi.Helpers;

public static class ServiceContainer
{
    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IGithubAuthService, GithubAuthService>();
        services.AddScoped<IEmailAuthService, EmailAuthService>();
        return services;
    }
}
