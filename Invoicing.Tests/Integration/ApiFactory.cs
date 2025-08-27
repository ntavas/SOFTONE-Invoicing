using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Invoicing.Tests.Integration;

/// <summary>
/// Spins up the API against the test Postgres. Override the connection string in-memory.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _pgConn;

    public ApiFactory(string postgresConnectionString) => _pgConn = postgresConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = _pgConn
            };
            cfg.AddInMemoryCollection(dict);
        });
    }
}