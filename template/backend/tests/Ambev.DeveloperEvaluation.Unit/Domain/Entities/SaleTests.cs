using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover item addition, discount rules, and cancellation scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that adding an item includes it in the sale and updates the total amount.
    /// </summary>
    [Fact(DisplayName = "Sale should include item and update total when an item is added")]
    public void Given_ValidSale_When_ItemAdded_Then_ItemShouldBeIncludedAndTotalUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();

        // Act
        sale.AddItem(productId, "Beer", 2, 10m);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(productId, sale.Items[0].ProductId);
        Assert.Equal(20m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that adding an item to a cancelled sale throws a domain exception.
    /// </summary>
    [Fact(DisplayName = "Adding an item to a cancelled sale should throw a domain exception")]
    public void Given_CancelledSale_When_ItemAdded_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancelled = true;

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.AddItem(Guid.NewGuid(), "Beer", 2, 10m));
    }

    /// <summary>
    /// Tests that purchases of 4 identical items receive a 10% discount.
    /// </summary>
    [Fact(DisplayName = "Four identical items should receive a 10% discount")]
    public void Given_FourIdenticalItems_When_ItemAdded_Then_ShouldApplyTenPercentDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.AddItem(Guid.NewGuid(), "Beer", 4, 10m);

        // Assert
        Assert.Equal(4m, sale.Items[0].Discount);
        Assert.Equal(36m, sale.Items[0].TotalAmount);
        Assert.Equal(36m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that purchases of 10 identical items receive a 20% discount.
    /// </summary>
    [Fact(DisplayName = "Ten identical items should receive a 20% discount")]
    public void Given_TenIdenticalItems_When_ItemAdded_Then_ShouldApplyTwentyPercentDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.AddItem(Guid.NewGuid(), "Beer", 10, 10m);

        // Assert
        Assert.Equal(20m, sale.Items[0].Discount);
        Assert.Equal(80m, sale.Items[0].TotalAmount);
        Assert.Equal(80m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that purchases below 4 identical items do not receive a discount.
    /// </summary>
    [Fact(DisplayName = "Three identical items should not receive a discount")]
    public void Given_ThreeIdenticalItems_When_ItemAdded_Then_ShouldNotApplyDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.AddItem(Guid.NewGuid(), "Beer", 3, 10m);

        // Assert
        Assert.Equal(0m, sale.Items[0].Discount);
        Assert.Equal(30m, sale.Items[0].TotalAmount);
        Assert.Equal(30m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that selling more than 20 identical items throws a domain exception.
    /// </summary>
    [Fact(DisplayName = "Adding more than 20 identical items should throw a domain exception")]
    public void Given_QuantityAboveLimit_When_ItemAdded_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.AddItem(Guid.NewGuid(), "Beer", 21, 10m));
    }

    /// <summary>
    /// Tests that cancelling a sale marks the sale and its items as cancelled.
    /// </summary>
    [Fact(DisplayName = "Cancelling a sale should mark the sale and items as cancelled")]
    public void Given_ValidSale_When_Cancelled_Then_SaleAndItemsShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 2, 10m);

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.Cancelled);
        Assert.True(sale.Items[0].Cancelled);
        Assert.Equal(0m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that cancelling an already cancelled sale throws a domain exception.
    /// </summary>
    [Fact(DisplayName = "Cancelling an already cancelled sale should throw a domain exception")]
    public void Given_CancelledSale_When_CancelledAgain_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.Cancel());
    }

    /// <summary>
    /// Tests that cancelling a sale item updates the item and the sale total.
    /// </summary>
    [Fact(DisplayName = "Cancelling a sale item should mark the item as cancelled and update the total")]
    public void Given_SaleWithItems_When_ItemCancelled_Then_ItemShouldBeCancelledAndTotalUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 2, 10m);
        sale.AddItem(Guid.NewGuid(), "Cake", 1, 10m);
        sale.Items[0].Id = Guid.NewGuid();
        sale.Items[1].Id = Guid.NewGuid();

        // Act
        sale.CancelItem(sale.Items[0].Id);

        // Assert
        Assert.True(sale.Items[0].Cancelled);
        Assert.False(sale.Items[1].Cancelled);
        Assert.False(sale.Cancelled);
        Assert.Equal(10m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that cancelling the last remaining item also cancels the sale.
    /// </summary>
    [Fact(DisplayName = "Cancelling the last remaining item should cancel the sale")]
    public void Given_SaleWithSingleItem_When_ItemCancelled_Then_SaleShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Beer", 2, 10m);
        sale.Items[0].Id = Guid.NewGuid();

        // Act
        sale.CancelItem(sale.Items[0].Id);

        // Assert
        Assert.True(sale.Items[0].Cancelled);
        Assert.True(sale.Cancelled);
        Assert.Equal(0m, sale.TotalAmount);
    }
}
