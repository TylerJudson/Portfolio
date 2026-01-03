namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for cleaning up stale games and pending games
    /// </summary>
    public interface IGameCleanupService
    {
        /// <summary>
        /// Removes active games that haven't been updated in the configured timeout period
        /// </summary>
        Task RemoveStaleGamesAsync();

        /// <summary>
        /// Removes pending games that were created before the configured timeout period
        /// </summary>
        Task RemoveStalePendingGamesAsync();
    }
}
