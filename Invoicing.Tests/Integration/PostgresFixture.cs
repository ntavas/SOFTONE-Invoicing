using Invoicing.Tests.Shared;
using Testcontainers.PostgreSql;
using Xunit;

namespace Invoicing.Tests.Integration;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pg;

    public string ConnectionString => _pg.GetConnectionString();

    public PostgresFixture()
    {
        _pg = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("invoicing_db_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _pg.StartAsync();
        await SqlScripts.EnsureSchemaAndSeedsAsync(ConnectionString);
    }

    public Task DisposeAsync() => _pg.DisposeAsync().AsTask();
}