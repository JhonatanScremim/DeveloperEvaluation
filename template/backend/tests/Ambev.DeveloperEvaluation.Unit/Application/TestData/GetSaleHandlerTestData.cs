using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class GetSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetSaleCommand instances.
    /// The generated commands will have a non-empty sale ID.
    /// </summary>
    private static readonly Faker<GetSaleCommand> getSaleHandlerFaker = new Faker<GetSaleCommand>()
        .CustomInstantiator(f => new GetSaleCommand(f.Random.Guid()));

    /// <summary>
    /// Generates a valid GetSaleCommand with randomized data.
    /// The generated command will have a valid identifier
    /// that meets the system's validation requirements.
    /// </summary>
    /// <returns>A valid GetSaleCommand with a randomly generated ID.</returns>
    public static GetSaleCommand GenerateValidCommand()
    {
        return getSaleHandlerFaker.Generate();
    }
}
