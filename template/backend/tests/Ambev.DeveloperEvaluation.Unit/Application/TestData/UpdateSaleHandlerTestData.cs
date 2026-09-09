using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class UpdateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid sale items.
    /// The generated items will have valid:
    /// - ProductId (non-empty GUID)
    /// - ProductName (non-empty)
    /// - Quantity (between 1 and 20)
    /// - UnitPrice (greater than 0)
    /// </summary>
    private static readonly Faker<UpdateSaleItemCommand> updateSaleItemFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100));

    /// <summary>
    /// Configures the Faker to generate valid UpdateSaleCommand instances.
    /// The generated commands will have valid:
    /// - Id (non-empty GUID)
    /// - SaleDate (UTC)
    /// - CustomerId and CustomerName
    /// - BranchId and BranchName
    /// - At least one valid item
    /// </summary>
    private static readonly Faker<UpdateSaleCommand> updateSaleHandlerFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleDate, _ => DateTime.UtcNow)
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, _ => updateSaleItemFaker.Generate(1));

    /// <summary>
    /// Generates a valid UpdateSaleCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid UpdateSaleCommand with randomly generated data.</returns>
    public static UpdateSaleCommand GenerateValidCommand()
    {
        return updateSaleHandlerFaker.Generate();
    }
}
