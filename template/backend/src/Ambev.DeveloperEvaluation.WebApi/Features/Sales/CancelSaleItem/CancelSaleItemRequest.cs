namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

/// <summary>
/// Request model for cancelling a specific item within a sale
/// </summary>
public class CancelSaleItemRequest
{
    /// <summary>
    /// The unique identifier of the sale that owns the item
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// The unique identifier of the item to cancel
    /// </summary>
    public Guid ItemId { get; set; }
}
