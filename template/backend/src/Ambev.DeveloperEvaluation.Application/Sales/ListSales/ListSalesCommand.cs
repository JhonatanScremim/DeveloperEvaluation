using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Command for retrieving a paginated list of sales.
/// </summary>
public class ListSalesCommand : IRequest<ListSalesResult>
{
    /// <summary>
    /// Gets or sets the page number to retrieve. Defaults to 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size. Defaults to 10 and cannot exceed 100.
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Gets or sets the ordering expression, for example "saleDate desc".
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    /// Gets or sets an optional sale number filter.
    /// </summary>
    public string? SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets an optional customer name filter.
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Gets or sets an optional branch name filter.
    /// </summary>
    public string? BranchName { get; set; }

    /// <summary>
    /// Gets or sets the optional minimum sale date filter.
    /// </summary>
    public DateTime? MinDate { get; set; }

    /// <summary>
    /// Gets or sets the optional maximum sale date filter.
    /// </summary>
    public DateTime? MaxDate { get; set; }

    /// <summary>
    /// Gets or sets an optional cancelled status filter.
    /// </summary>
    public bool? Cancelled { get; set; }
}

/// <summary>
/// Response model for ListSales operation
/// </summary>
public class ListSalesResult
{
    /// <summary>
    /// The sales returned for the current page
    /// </summary>
    public List<SaleResult> Data { get; set; } = new List<SaleResult>();

    /// <summary>
    /// The current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The total number of sales matching the criteria
    /// </summary>
    public int TotalCount { get; set; }
}
