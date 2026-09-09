using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class ListSalesHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid ListSalesCommand instances.
    /// The generated commands will have valid:
    /// - Page (greater than 0)
    /// - Size (between 1 and 100)
    /// </summary>
    private static readonly Faker<ListSalesCommand> listSalesHandlerFaker = new Faker<ListSalesCommand>()
        .RuleFor(c => c.Page, 1)
        .RuleFor(c => c.Size, 10)
        .RuleFor(c => c.Order, "saleDate desc")
        .RuleFor(c => c.SaleNumber, f => $"SALE-{f.Random.AlphaNumeric(8).ToUpperInvariant()}")
        .RuleFor(c => c.CustomerName, f => f.Person.FullName)
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName());

    /// <summary>
    /// Generates a valid ListSalesCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid ListSalesCommand with randomly generated data.</returns>
    public static ListSalesCommand GenerateValidCommand()
    {
        return listSalesHandlerFaker.Generate();
    }
}
