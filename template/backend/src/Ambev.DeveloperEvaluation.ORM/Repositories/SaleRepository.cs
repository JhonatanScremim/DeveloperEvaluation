using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    public readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Retrieves a paginated list of sales matching the specified criteria
    /// </summary>
    /// <param name="criteria">The filter, sorting, and pagination criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The matching sales and the total number of records</returns>
    public async Task<(List<Sale>, int TotalCount)> ListAsync(
        SaleListCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var page = criteria.Page <= 0 ? 1 : criteria.Page;
        var size = criteria.Size <= 0 ? 10 : Math.Min(criteria.Size, 100);

        var query = _context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.SaleNumber))
            query = query.Where(s => s.SaleNumber.Contains(criteria.SaleNumber));

        if (!string.IsNullOrWhiteSpace(criteria.CustomerName))
            query = query.Where(s => s.CustomerName.Contains(criteria.CustomerName));

        if (!string.IsNullOrWhiteSpace(criteria.BranchName))
            query = query.Where(s => s.BranchName.Contains(criteria.BranchName));

        if (criteria.MinDate.HasValue)
            query = query.Where(s => s.SaleDate >= criteria.MinDate.Value);

        if (criteria.MaxDate.HasValue)
            query = query.Where(s => s.SaleDate <= criteria.MaxDate.Value);

        if (criteria.Cancelled.HasValue)
            query = query.Where(s => s.Cancelled == criteria.Cancelled.Value);

        if (string.IsNullOrWhiteSpace(criteria.Order))
        {
            query = query.OrderByDescending(s => s.SaleDate);
        }
        else
        {
            var tokens = criteria.Order.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = tokens[0].ToLowerInvariant();
            var descending = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = (field, descending) switch
            {
                ("salenumber", true) => query.OrderByDescending(s => s.SaleNumber),
                ("salenumber", false) => query.OrderBy(s => s.SaleNumber),
                ("saledate", true) => query.OrderByDescending(s => s.SaleDate),
                ("saledate", false) => query.OrderBy(s => s.SaleDate),
                ("totalamount", true) => query.OrderByDescending(s => s.TotalAmount),
                ("totalamount", false) => query.OrderBy(s => s.TotalAmount),
                ("customername", true) => query.OrderByDescending(s => s.CustomerName),
                ("customername", false) => query.OrderBy(s => s.CustomerName),
                ("branchname", true) => query.OrderByDescending(s => s.BranchName),
                ("branchname", false) => query.OrderBy(s => s.BranchName),
                ("cancelled", true) => query.OrderByDescending(s => s.Cancelled),
                ("cancelled", false) => query.OrderBy(s => s.Cancelled),
                ("id", true) => query.OrderByDescending(s => s.Id),
                ("id", false) => query.OrderBy(s => s.Id),
                _ => query.OrderByDescending(s => s.SaleDate)
            };
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var sales = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (sales, totalCount);
    }
}
