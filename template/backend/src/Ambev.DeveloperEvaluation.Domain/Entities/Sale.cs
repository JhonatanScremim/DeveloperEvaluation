using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale in the system with customer, branch, and item information.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets the sale number.
    /// Must not be null or empty and uniquely identifies the sale.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets the unique identifier of the customer associated with the sale.
    /// Uses the External Identities pattern with denormalized customer name.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets the denormalized name of the customer associated with the sale.
    /// Must not be null or empty.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier of the branch where the sale was made.
    /// Uses the External Identities pattern with denormalized branch name.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets the denormalized name of the branch where the sale was made.
    /// Must not be null or empty.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total amount of the sale.
    /// Calculated from the non-cancelled items including applied discounts.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets a value indicating whether the sale has been cancelled.
    /// </summary>
    public bool Cancelled { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the list of items included in the sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a product item to the sale and recalculates the total amount.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="productName">The denormalized name of the product.</param>
    /// <param name="quantity">The quantity of identical items. Cannot exceed 20.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (Cancelled)
            throw new DomainException("Cannot modify a cancelled sale");

        Items.Add(SaleItem.Create(productId, productName, quantity, unitPrice));
        TotalAmount = Items.Where(i => !i.Cancelled).Sum(i => i.TotalAmount);
        UpdatedAt = DateTime.UtcNow;
    }
}
