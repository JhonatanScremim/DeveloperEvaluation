using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional;

/// <summary>
/// Web application factory that hosts the API against a Testcontainers PostgreSQL database.
/// </summary>
public class SalesWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("developerevaluation_functional")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private string _connectionString = string.Empty;

    /// <summary>
    /// Starts the PostgreSQL container and applies database migrations.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        await db.Database.MigrateAsync();
    }

    /// <summary>
    /// Configures the test host to use the container connection string.
    /// </summary>
    /// <param name="builder">The web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString
            });
        });
    }

    /// <summary>
    /// Stops and disposes the PostgreSQL container and the web host.
    /// </summary>
    async Task IAsyncLifetime.DisposeAsync()
    {
        await _container.DisposeAsync();
        await base.DisposeAsync();
    }
}
