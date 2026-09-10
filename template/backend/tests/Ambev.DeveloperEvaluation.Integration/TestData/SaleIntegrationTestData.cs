using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Integration.TestData;

public static class SaleIntegrationTestData
{
    public static Sale NewSale(string? saleNumber = null)
    {
        return new Sale
        {
            SaleNumber = saleNumber ?? $"SALE-{Guid.NewGuid():N}",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Main Branch"
        };
    }
}
