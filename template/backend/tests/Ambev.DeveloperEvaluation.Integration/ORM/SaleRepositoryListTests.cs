using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM;

[Collection("Postgres")]
public class SaleRepositoryListTests
{
    private readonly PostgresFixture _fixture;

    public SaleRepositoryListTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Given sales with different numbers When filtering by sale number Then returns only matches")]
    public async Task ListAsync_FilterBySaleNumber_ReturnsMatching()
    {
        // Given
        var prefix = $"LIST-{Guid.NewGuid():N}";
        var target = SaleIntegrationTestData.NewSale($"{prefix}-A");
        target.AddItem(Guid.NewGuid(), "Beer", 1, 10m);
        var other = SaleIntegrationTestData.NewSale($"OTHER-{Guid.NewGuid():N}");
        other.AddItem(Guid.NewGuid(), "Cake", 1, 10m);

        await _fixture.SeedAsync(target);
        await _fixture.SeedAsync(other);

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);

        // When
        var (sales, totalCount) = await repo.ListAsync(new SaleListCriteria
        {
            SaleNumber = prefix,
            Page = 1,
            Size = 10
        });

        // Then
        totalCount.Should().Be(1);
        sales.Should().ContainSingle(s => s.SaleNumber == target.SaleNumber);
    }

    [Fact(DisplayName = "Given several sales When listing with page size Then returns page and total count")]
    public async Task ListAsync_Pagination_ReturnsPageAndTotalCount()
    {
        // Given
        var prefix = $"PAGE-{Guid.NewGuid():N}";
        for (var i = 0; i < 3; i++)
        {
            var sale = SaleIntegrationTestData.NewSale($"{prefix}-{i}");
            sale.AddItem(Guid.NewGuid(), "Beer", 1, 10m);
            await _fixture.SeedAsync(sale);
        }

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);

        // When
        var (sales, totalCount) = await repo.ListAsync(new SaleListCriteria
        {
            SaleNumber = prefix,
            Page = 1,
            Size = 2,
            Order = "saleNumber asc"
        });

        // Then
        totalCount.Should().Be(3);
        sales.Should().HaveCount(2);
        sales.Select(s => s.SaleNumber).Should().BeInAscendingOrder();
    }

    [Fact(DisplayName = "Given active and cancelled sales When filtering cancelled Then returns only cancelled")]
    public async Task ListAsync_FilterByCancelled_ReturnsOnlyCancelled()
    {
        // Given
        var prefix = $"CX-{Guid.NewGuid():N}";
        var active = SaleIntegrationTestData.NewSale($"{prefix}-A");
        active.AddItem(Guid.NewGuid(), "Beer", 1, 10m);

        var cancelled = SaleIntegrationTestData.NewSale($"{prefix}-C");
        cancelled.AddItem(Guid.NewGuid(), "Cake", 1, 10m);
        cancelled.Cancel();

        await _fixture.SeedAsync(active);
        await _fixture.SeedAsync(cancelled);

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);

        // When
        var (sales, totalCount) = await repo.ListAsync(new SaleListCriteria
        {
            SaleNumber = prefix,
            Cancelled = true,
            Page = 1,
            Size = 10
        });

        // Then
        totalCount.Should().Be(1);
        sales.Should().ContainSingle(s => s.SaleNumber == cancelled.SaleNumber && s.Cancelled);
    }
}
