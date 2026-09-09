namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Criteria used to filter, sort, and paginate sales in the repository
/// </summary>
public class SaleListCriteria
{
    /// <summary>
    /// Gets or sets the page number to retrieve. Defaults to 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size. Defaults to 10.
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
