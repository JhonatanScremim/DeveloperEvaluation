using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid sale items.
    /// The generated items will have valid:
    /// - ProductId (non-empty GUID)
    /// - ProductName (non-empty)
    /// - Quantity (between 1 and 20)
    /// - UnitPrice (greater than 0)
    /// </summary>
    private static readonly Faker<CreateSaleItemCommand> createSaleItemFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(1, 100));

    /// <summary>
    /// Configures the Faker to generate valid CreateSaleCommand instances.
    /// The generated commands will have valid:
    /// - SaleDate (UTC)
    /// - CustomerId and CustomerName
    /// - BranchId and BranchName
    /// - At least one valid item
    /// </summary>
    private static readonly Faker<CreateSaleCommand> createSaleHandlerFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.AlphaNumeric(8).ToUpperInvariant()}")
        .RuleFor(s => s.SaleDate, _ => DateTime.UtcNow)
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, _ => createSaleItemFaker.Generate(1));

    /// <summary>
    /// Generates a valid CreateSaleCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CreateSaleCommand with randomly generated data.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return createSaleHandlerFaker.Generate();
    }
}
