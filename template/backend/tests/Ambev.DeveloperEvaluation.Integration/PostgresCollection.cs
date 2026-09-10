using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

[CollectionDefinition("Postgres")]
public class PostgresCollection : ICollectionFixture<PostgresFixture>
{
}
