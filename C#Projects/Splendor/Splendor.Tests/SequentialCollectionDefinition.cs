using Xunit;

namespace Splendor.Tests
{
    /// <summary>
    /// Collection definition for tests that must run sequentially.
    /// Used for integration tests that share static state (ActiveGames, PendingGames dictionaries).
    /// </summary>
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SequentialCollectionDefinition
    {
        // This class is never instantiated. It exists only to define the collection.
    }
}
