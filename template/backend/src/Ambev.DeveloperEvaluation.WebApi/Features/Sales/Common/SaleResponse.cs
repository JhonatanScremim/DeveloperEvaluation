namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// API response model for CreateSale operation
/// </summary>
public class SaleResponse
{
    /// <summary>
    /// The unique identifier of the created sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The sale number
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The date when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The customer associated with the sale
    /// </summary>
    public ExternalIdentityResponse Customer { get; set; } = new();

    /// <summary>
    /// The branch where the sale was made
    /// </summary>
    public ExternalIdentityResponse Branch { get; set; } = new();

    /// <summary>
    /// The total amount of the sale
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indicates whether the sale has been cancelled
    /// </summary>
    public bool Cancelled { get; set; }

    /// <summary>
    /// The items included in the sale
    /// </summary>
    public List<SaleItemResponse> Items { get; set; } = new();
}
