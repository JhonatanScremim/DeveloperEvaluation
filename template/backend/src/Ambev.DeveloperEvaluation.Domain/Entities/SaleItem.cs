using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product item within a sale.
/// This entity follows domain-driven design principles and applies quantity-based discount rules.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets the unique identifier of the sale that owns this item.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets the unique identifier of the product.
    /// Uses the External Identities pattern with denormalized product name.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets the denormalized name of the product.
    /// Must not be null or empty.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the quantity of identical items.
    /// Must be greater than zero and cannot exceed 20.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// Must be greater than zero.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the discount amount applied to this item based on quantity rules.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets the total amount for this item after applying discounts.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets a value indicating whether the item has been cancelled.
    /// </summary>
    public bool Cancelled { get; set; }

    /// <summary>
    /// Creates a new sale item with the specified product details and calculated discount.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="productName">The denormalized name of the product.</param>
    /// <param name="quantity">The quantity of identical items. Cannot exceed 20.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    /// <returns>A new <see cref="SaleItem"/> with discount and total calculated.</returns>
    public static SaleItem Create(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product is required");

        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name is required");

        var item = new SaleItem
        {
            ProductId = productId,
            ProductName = productName.Trim()
        };

        item.SetQuantityAndPrice(quantity, unitPrice);
        return item;
    }

    /// <summary>
    /// Sets the quantity and unit price, recalculating discount and total amount.
    /// </summary>
    /// <param name="quantity">The quantity of identical items. Cannot exceed 20.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    public void SetQuantityAndPrice(int quantity, decimal unitPrice)
    {
        if (Cancelled)
            throw new DomainException("Cannot modify a cancelled item");

        var calculator = new SalesDiscountCalculator();
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = calculator.CalculateDiscountAmount(quantity, unitPrice);
        TotalAmount = calculator.CalculateTotal(quantity, unitPrice);
    }

    /// <summary>
    /// Cancels the item and clears its discount and total amount.
    /// </summary>
    public void Cancel()
    {
        if (Cancelled)
            throw new DomainException("Item is already cancelled");

        Cancelled = true;
        Discount = 0;
        TotalAmount = 0;
    }
}
