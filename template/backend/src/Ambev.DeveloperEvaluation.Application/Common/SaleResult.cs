namespace Ambev.DeveloperEvaluation.Application.Common;

/// <summary>
/// Represents the response returned after successfully creating a new sale.
/// </summary>
/// <remarks>
/// This response contains the unique identifier of the newly created sale,
/// which can be used for subsequent operations or reference.
/// </remarks>
public class SaleResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created sale.
    /// </summary>
    /// <value>A GUID that uniquely identifies the created sale in the system.</value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer associated with the sale.
    /// </summary>
    public ExternalIdentityResult Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public ExternalIdentityResult Branch { get; set; } = new();

    /// <summary>
    /// Gets or sets the total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sale has been cancelled.
    /// </summary>
    public bool Cancelled { get; set; }

    /// <summary>
    /// Gets or sets the items included in the sale.
    /// </summary>
    public List<SaleItemResult> Items { get; set; } = new();
}
