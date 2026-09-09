using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Command for creating a new sale.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for creating a sale,
/// including sale number, date, customer, branch, and items.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request
/// that returns a <see cref="SaleResult"/>.
///
/// The data provided in this command is validated using the
/// <see cref="CreateSaleCommandValidator"/> which extends
/// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly
/// populated and follow the required rules.
/// </remarks>
public class CreateSaleCommand : IRequest<SaleResult>
{
    /// <summary>
    /// Gets or sets the sale number of the sale to be created.
    /// If omitted, a sale number is generated automatically.
    /// </summary>
    public string? SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized name of the customer.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the branch where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized name of the branch.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the items included in the sale.
    /// </summary>
    public List<CreateSaleItemCommand> Items { get; set; } = new();
}

/// <summary>
/// Command item representing a product to be added to the sale.
/// </summary>
public class CreateSaleItemCommand
{
    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized name of the product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of identical items.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
