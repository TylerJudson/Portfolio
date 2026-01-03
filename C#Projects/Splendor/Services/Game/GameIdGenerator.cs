using Splendor.Repositories;

namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for generating unique game IDs
    /// </summary>
    public class GameIdGenerator : IGameIdGenerator
    {
        private readonly IPendingGameRepository _pendingGameRepository;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly ILogger<GameIdGenerator> _logger;

        public GameIdGenerator(IPendingGameRepository pendingGameRepository, ILogger<GameIdGenerator> logger)
        {
            _pendingGameRepository = pendingGameRepository;
            _logger = logger;
        }

        /// <summary>
        /// Generates a unique game ID that doesn't conflict with existing pending games
        /// </summary>
        /// <returns>A unique game ID</returns>
        public async Task<int> GenerateUniqueIdAsync()
        {
            _logger.LogDebug("Generating unique game ID");

            // Use SemaphoreSlim instead of lock(this) for thread-safe async operation
            await _lock.WaitAsync();
            try
            {
                var random = Random.Shared;
                int gameId;

                // Keep generating random IDs until we find one that doesn't exist
                do
                {
                    gameId = random.Next();
                }
                while (await _pendingGameRepository.PendingGameExistsAsync(gameId));

                _logger.LogDebug("Generated unique game ID {GameId}", gameId);
                return gameId;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
