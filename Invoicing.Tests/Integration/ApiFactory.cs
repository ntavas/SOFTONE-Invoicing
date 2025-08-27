using Invoicing.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Invoicing.Tests.Integration;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _pgConn;

    public ApiFactory(string postgresConnectionString) => _pgConn = postgresConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = _pgConn
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove the app’s DbContext registration
            var desc = services.Single(s => s.ServiceType == typeof(DbContextOptions<InvoicingDbContext>));
            services.Remove(desc);

            // Add DbContext targeting the test container
            services.AddDbContext<InvoicingDbContext>(opt =>
            {
                opt.UseNpgsql(_pgConn);
                opt.UseSnakeCaseNamingConvention();
            });
        });
    }
}