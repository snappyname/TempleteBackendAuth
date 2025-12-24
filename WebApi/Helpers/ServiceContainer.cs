using Application.Abstract;
using Application.Services.UserAuth;

namespace TemplateWebApi.Helpers;

public static class ServiceContainer
{
    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        return services;
    }
}
