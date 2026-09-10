using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.Functional.TestData;

/// <summary>
/// Provides request payloads for Sales API functional tests.
/// </summary>
public static class SaleApiTestData
{
    /// <summary>
    /// Creates a valid <see cref="CreateSaleRequest"/> with the specified number of items.
    /// </summary>
    /// <param name="saleNumber">Optional sale number. When omitted, a unique value is generated.</param>
    /// <param name="itemCount">The number of items to include.</param>
    /// <returns>A valid create sale request.</returns>
    public static CreateSaleRequest NewCreateRequest(string? saleNumber = null, int itemCount = 1)
    {
        var items = new List<CreateSaleItemRequest>();
        for (var i = 0; i < itemCount; i++)
        {
            items.Add(new CreateSaleItemRequest
            {
                ProductId = Guid.NewGuid(),
                ProductName = i == 0 ? "Beer" : "Cake",
                Quantity = i == 0 ? 4 : 2,
                UnitPrice = 10m
            });
        }

        return new CreateSaleRequest
        {
            SaleNumber = saleNumber ?? $"SALE-{Guid.NewGuid():N}",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Main Branch",
            Items = items
        };
    }

    /// <summary>
    /// Creates a valid <see cref="UpdateSaleRequest"/> with updated customer and items.
    /// </summary>
    /// <returns>A valid update sale request.</returns>
    public static UpdateSaleRequest NewUpdateRequest()
    {
        return new UpdateSaleRequest
        {
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Jane Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "North Branch",
            Items =
            [
                new UpdateSaleItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Wine",
                    Quantity = 10,
                    UnitPrice = 20m
                }
            ]
        };
    }
}
