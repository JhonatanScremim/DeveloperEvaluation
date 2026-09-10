using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CancelSaleItemHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CancelSaleItemCommand instances.
    /// The generated commands will have non-empty sale and item IDs.
    /// </summary>
    private static readonly Faker<CancelSaleItemCommand> cancelSaleItemHandlerFaker = new Faker<CancelSaleItemCommand>()
        .CustomInstantiator(f => new CancelSaleItemCommand(f.Random.Guid(), f.Random.Guid()));

    /// <summary>
    /// Generates a valid CancelSaleItemCommand with randomized data.
    /// The generated command will have valid identifiers
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CancelSaleItemCommand with randomly generated IDs.</returns>
    public static CancelSaleItemCommand GenerateValidCommand()
    {
        return cancelSaleItemHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CancelSaleItemCommand for a specific sale and item.
    /// </summary>
    /// <param name="saleId">The sale identifier.</param>
    /// <param name="itemId">The item identifier.</param>
    /// <returns>A valid CancelSaleItemCommand with the specified IDs.</returns>
    public static CancelSaleItemCommand GenerateValidCommand(Guid saleId, Guid itemId)
    {
        return new CancelSaleItemCommand(saleId, itemId);
    }
}
