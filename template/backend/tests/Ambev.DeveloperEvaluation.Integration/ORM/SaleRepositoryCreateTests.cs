using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM;

[Collection("Postgres")]
public class SaleRepositoryCreateTests
{
    private readonly PostgresFixture _fixture;

    public SaleRepositoryCreateTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Given valid sale When creating Then persists with items and generated id")]
    public async Task CreateAsync_ValidSale_PersistsWithItems()
    {
        // Given
        var sale = SaleIntegrationTestData.NewSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 4, 10m);

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);

        // When
        var created = await repo.CreateAsync(sale);

        // Then
        created.Id.Should().NotBe(Guid.Empty);

        await using var readDb = _fixture.CreateContext();
        var loaded = await new SaleRepository(readDb).GetByIdAsync(created.Id);

        loaded.Should().NotBeNull();
        loaded!.Items.Should().ContainSingle(i => i.ProductName == "Beer" && i.Quantity == 4);
        loaded.TotalAmount.Should().Be(36m);
    }

    [Fact(DisplayName = "Given sale with multiple items When creating Then persists all items and total")]
    public async Task CreateAsync_SaleWithMultipleItems_PersistsAllItems()
    {
        // Given
        var sale = SaleIntegrationTestData.NewSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 4, 10m);
        sale.AddItem(Guid.NewGuid(), "Cake", 3, 10m);

        // When
        var created = await _fixture.SeedAsync(sale);

        // Then
        await using var db = _fixture.CreateContext();
        var loaded = await new SaleRepository(db).GetByIdAsync(created.Id);

        loaded.Should().NotBeNull();
        loaded!.Items.Should().HaveCount(2);
        loaded.TotalAmount.Should().Be(66m);
    }

    [Fact(DisplayName = "Given existing sale number When creating duplicate Then throws DbUpdateException")]
    public async Task CreateAsync_DuplicateSaleNumber_ThrowsDbUpdateException()
    {
        // Given
        var saleNumber = $"SALE-DUP-{Guid.NewGuid():N}";
        var first = SaleIntegrationTestData.NewSale(saleNumber);
        first.AddItem(Guid.NewGuid(), "Beer", 1, 10m);
        await _fixture.SeedAsync(first);

        var duplicate = SaleIntegrationTestData.NewSale(saleNumber);
        duplicate.AddItem(Guid.NewGuid(), "Cake", 1, 10m);

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);

        // When
        var act = () => repo.CreateAsync(duplicate);

        // Then
        await act.Should().ThrowAsync<DbUpdateException>();
    }
}
