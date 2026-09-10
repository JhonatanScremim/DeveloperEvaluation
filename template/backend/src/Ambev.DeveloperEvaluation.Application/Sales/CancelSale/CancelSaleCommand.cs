using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command for cancelling a sale by their ID
/// </summary>
public record CancelSaleCommand : IRequest<SaleResult>
{
    /// <summary>
    /// The unique identifier of the sale to cancel
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of CancelSaleCommand
    /// </summary>
    /// <param name="id">The ID of the sale to cancel</param>
    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}
