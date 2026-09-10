using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CancelSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CancelSaleCommand instances.
    /// The generated commands will have a non-empty sale ID.
    /// </summary>
    private static readonly Faker<CancelSaleCommand> cancelSaleHandlerFaker = new Faker<CancelSaleCommand>()
        .CustomInstantiator(f => new CancelSaleCommand(f.Random.Guid()));

    /// <summary>
    /// Generates a valid CancelSaleCommand with randomized data.
    /// The generated command will have a valid identifier
    /// that meets the system's validation requirements.
    /// </summary>
    /// <returns>A valid CancelSaleCommand with a randomly generated ID.</returns>
    public static CancelSaleCommand GenerateValidCommand()
    {
        return cancelSaleHandlerFaker.Generate();
    }
}
