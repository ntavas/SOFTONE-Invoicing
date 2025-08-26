using Invoicing.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Invoicing.Infrastructure.Persistence;
using Invoicing.Infrastructure.Repositories;

namespace Invoicing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Postgres")
                 ?? throw new InvalidOperationException("Missing connection string 'Postgres'.");

        services.AddDbContext<InvoicingDbContext>(opt =>
            opt.UseNpgsql(cs, npg => npg.EnableRetryOnFailure())
                .UseSnakeCaseNamingConvention());
        
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}