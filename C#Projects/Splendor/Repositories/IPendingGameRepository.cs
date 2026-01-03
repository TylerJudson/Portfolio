using Splendor.Models;

namespace Splendor.Repositories
{
    /// <summary>
    /// Repository interface for managing pending game persistence
    /// </summary>
    public interface IPendingGameRepository
    {
        /// <summary>
        /// Gets a pending game by ID
        /// </summary>
        Task<IPotentialGame?> GetPendingGameAsync(int gameId);

        /// <summary>
        /// Gets all pending games
        /// </summary>
        Task<Dictionary<int, IPotentialGame>> GetAllPendingGamesAsync();

        /// <summary>
        /// Adds a new pending game
        /// </summary>
        Task AddPendingGameAsync(int gameId, IPotentialGame potentialGame);

        /// <summary>
        /// Updates an existing pending game
        /// </summary>
        Task UpdatePendingGameAsync(int gameId, IPotentialGame potentialGame);

        /// <summary>
        /// Removes a pending game
        /// </summary>
        Task RemovePendingGameAsync(int gameId);

        /// <summary>
        /// Removes pending games that were created before the specified time span
        /// </summary>
        Task RemoveStalePendingGamesAsync(TimeSpan maxAge);

        /// <summary>
        /// Checks if a pending game exists
        /// </summary>
        Task<bool> PendingGameExistsAsync(int gameId);
    }
}
