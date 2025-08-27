using Npgsql;

namespace Invoicing.Tests.Shared;

/// <summary>
/// Loads and executes the same SQL files the app uses to avoid duplication.
/// If files are missing , it falls back to an inline schema.
/// </summary>
public static class SqlScripts
{
    public static async Task EnsureSchemaAndSeedsAsync(string connectionString, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(ct);

        var root = FindSolutionRoot();
        var scripts = new[]
        {
            Path.Combine(root, "ops", "pg-init", "01_schema.sql"),
            Path.Combine(root, "ops", "pg-init", "02_seed_companies.sql"),
            Path.Combine(root, "ops", "pg-init", "03_seed_users.sql"),
        };

        if (scripts.All(File.Exists))
        {
            foreach (var file in scripts)
            {
                var sql = await File.ReadAllTextAsync(file, ct);
                await ExecuteSqlAsync(conn, sql, ct);
            }
            return;
        }
        
        await ExecuteSqlAsync(conn, TestConstants.MinimalSchema, ct);
    }

    private static async Task ExecuteSqlAsync(NpgsqlConnection conn, string sql, CancellationToken ct)
    {
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static string FindSolutionRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (!string.IsNullOrEmpty(dir) && !File.Exists(Path.Combine(dir, "Invoicing.sln")))
        {
            dir = Directory.GetParent(dir)?.FullName ?? "";
        }
        return string.IsNullOrEmpty(dir) ? Directory.GetCurrentDirectory() : dir;
    }
}