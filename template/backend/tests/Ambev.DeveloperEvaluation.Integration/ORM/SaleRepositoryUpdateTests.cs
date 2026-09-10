using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM;

[Collection("Postgres")]
public class SaleRepositoryUpdateTests
{
    private readonly PostgresFixture _fixture;

    public SaleRepositoryUpdateTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Given existing sale When updating customer and items Then persists changes")]
    public async Task UpdateAsync_ChangeCustomerAndReplaceItems_PersistsChanges()
    {
        // Given
        var sale = SaleIntegrationTestData.NewSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 4, 10m);
        var created = await _fixture.SeedAsync(sale);

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);
        var existing = await repo.GetByIdAsync(created.Id);
        existing!.CustomerName = "Jane Customer";
        existing.BranchName = "North Branch";
        existing.ReplaceItems([(Guid.NewGuid(), "Wine", 10, 20m)]);

        // When
        await repo.UpdateAsync(existing);

        // Then
        await using var readDb = _fixture.CreateContext();
        var loaded = await new SaleRepository(readDb).GetByIdAsync(created.Id);

        loaded.Should().NotBeNull();
        loaded!.CustomerName.Should().Be("Jane Customer");
        loaded.BranchName.Should().Be("North Branch");
        loaded.Items.Should().ContainSingle(i => i.ProductName == "Wine" && i.Quantity == 10);
        loaded.TotalAmount.Should().Be(160m);
    }

    [Fact(DisplayName = "Given active sale When cancelling Then persists cancelled state")]
    public async Task UpdateAsync_CancelSale_PersistsCancelledState()
    {
        // Given
        var sale = SaleIntegrationTestData.NewSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 4, 10m);
        sale.AddItem(Guid.NewGuid(), "Cake", 2, 10m);
        var created = await _fixture.SeedAsync(sale);

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);
        var existing = await repo.GetByIdAsync(created.Id);
        existing!.Cancel();

        // When
        await repo.UpdateAsync(existing);

        // Then
        await using var readDb = _fixture.CreateContext();
        var loaded = await new SaleRepository(readDb).GetByIdAsync(created.Id);

        loaded.Should().NotBeNull();
        loaded!.Cancelled.Should().BeTrue();
        loaded.Items.Should().OnlyContain(i => i.Cancelled);
        loaded.TotalAmount.Should().Be(0m);
    }

    [Fact(DisplayName = "Given sale with items When cancelling one item Then recalculates total")]
    public async Task UpdateAsync_CancelItem_PersistsAndRecalculatesTotal()
    {
        // Given
        var sale = SaleIntegrationTestData.NewSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 4, 10m);
        sale.AddItem(Guid.NewGuid(), "Cake", 2, 10m);
        var created = await _fixture.SeedAsync(sale);
        var beerId = created.Items.Single(i => i.ProductName == "Beer").Id;

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);
        var existing = await repo.GetByIdAsync(created.Id);
        existing!.CancelItem(beerId);

        // When
        await repo.UpdateAsync(existing);

        // Then
        await using var readDb = _fixture.CreateContext();
        var loaded = await new SaleRepository(readDb).GetByIdAsync(created.Id);

        loaded.Should().NotBeNull();
        loaded!.Cancelled.Should().BeFalse();
        loaded.Items.Single(i => i.Id == beerId).Cancelled.Should().BeTrue();
        loaded.TotalAmount.Should().Be(20m);
    }
}
