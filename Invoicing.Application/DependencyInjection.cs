using Invoicing.Application.Services;
using Invoicing.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Invoicing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}