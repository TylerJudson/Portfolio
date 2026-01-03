using Splendor.Repositories;

namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for cleaning up stale games and pending games
    /// </summary>
    public class GameCleanupService : IGameCleanupService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPendingGameRepository _pendingGameRepository;
        private readonly TimeSpan _staleGameTimeout;
        private readonly ILogger<GameCleanupService> _logger;

        /// <summary>
        /// Initializes a new instance of the GameCleanupService
        /// </summary>
        /// <param name="gameRepository">Repository for active games</param>
        /// <param name="pendingGameRepository">Repository for pending games</param>
        /// <param name="staleGameTimeoutMinutes">Number of minutes before a game is considered stale (default: 30)</param>
        /// <param name="logger">Logger instance</param>
        public GameCleanupService(
            IGameRepository gameRepository,
            IPendingGameRepository pendingGameRepository,
            ILogger<GameCleanupService> logger,
            int staleGameTimeoutMinutes = 30)
        {
            _gameRepository = gameRepository;
            _pendingGameRepository = pendingGameRepository;
            _logger = logger;
            _staleGameTimeout = TimeSpan.FromMinutes(staleGameTimeoutMinutes);
        }

        /// <summary>
        /// Removes active games that haven't been updated in the configured timeout period
        /// </summary>
        public async Task RemoveStaleGamesAsync()
        {
            _logger.LogDebug("Starting cleanup of stale active games (timeout: {TimeoutMinutes} minutes)", _staleGameTimeout.TotalMinutes);
            await _gameRepository.RemoveStaleGamesAsync(_staleGameTimeout);
            _logger.LogInformation("Cleanup of stale active games completed");
        }

        /// <summary>
        /// Removes pending games that were created before the configured timeout period
        /// </summary>
        public async Task RemoveStalePendingGamesAsync()
        {
            _logger.LogDebug("Starting cleanup of stale pending games (timeout: {TimeoutMinutes} minutes)", _staleGameTimeout.TotalMinutes);
            await _pendingGameRepository.RemoveStalePendingGamesAsync(_staleGameTimeout);
            _logger.LogInformation("Cleanup of stale pending games completed");
        }
    }
}
