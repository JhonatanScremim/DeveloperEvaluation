using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("developerevaluation_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public DefaultContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseNpgsql(
                _container.GetConnectionString(),
                b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM"))
            .Options;

        return new DefaultContext(options);
    }

    public async Task<Sale> SeedAsync(Sale sale)
    {
        await using var context = CreateContext();
        return await new SaleRepository(context).CreateAsync(sale);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
