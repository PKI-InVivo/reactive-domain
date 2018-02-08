using ReactiveDomain.Testing;
using Xunit;

namespace ReactiveDomain.Domain.Tests
{
    [CollectionDefinition(nameof(EmbeddedEventStoreCollection))]
    public class EmbeddedEventStoreCollection : ICollectionFixture<EmbeddedEventStoreFixture>
    {
    }
}