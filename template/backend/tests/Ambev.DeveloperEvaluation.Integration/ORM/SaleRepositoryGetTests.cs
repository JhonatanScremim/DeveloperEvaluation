using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM;

[Collection("Postgres")]
public class SaleRepositoryGetTests
{
    private readonly PostgresFixture _fixture;

    public SaleRepositoryGetTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Given existing sale When getting by id Then returns sale with items")]
    public async Task GetByIdAsync_ExistingSale_ReturnsWithItems()
    {
        // Given
        var sale = SaleIntegrationTestData.NewSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 4, 10m);
        var created = await _fixture.SeedAsync(sale);

        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);

        // When
        var loaded = await repo.GetByIdAsync(created.Id);

        // Then
        loaded.Should().NotBeNull();
        loaded!.Id.Should().Be(created.Id);
        loaded.Items.Should().ContainSingle(i => i.ProductName == "Beer");
        loaded.TotalAmount.Should().Be(36m);
    }

    [Fact(DisplayName = "Given unknown id When getting by id Then returns null")]
    public async Task GetByIdAsync_NonExistingSale_ReturnsNull()
    {
        // Given
        await using var db = _fixture.CreateContext();
        var repo = new SaleRepository(db);

        // When
        var loaded = await repo.GetByIdAsync(Guid.NewGuid());

        // Then
        loaded.Should().BeNull();
    }
}
