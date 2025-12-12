using Application.Abstract;
using Application.Services;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace TemplateWebApi.Helpers;

public static class ServiceContainer
{
    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}