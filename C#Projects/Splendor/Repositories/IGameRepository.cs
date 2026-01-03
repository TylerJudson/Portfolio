using Splendor.Models;

namespace Splendor.Repositories
{
    /// <summary>
    /// Repository interface for managing active game persistence
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Gets a game by ID
        /// </summary>
        Task<IGameBoard?> GetGameAsync(int gameId);

        /// <summary>
        /// Gets all active games
        /// </summary>
        Task<Dictionary<int, IGameBoard>> GetAllGamesAsync();

        /// <summary>
        /// Adds a new game
        /// </summary>
        Task AddGameAsync(int gameId, IGameBoard gameBoard);

        /// <summary>
        /// Updates an existing game
        /// </summary>
        Task UpdateGameAsync(int gameId, IGameBoard gameBoard);

        /// <summary>
        /// Removes a game
        /// </summary>
        Task RemoveGameAsync(int gameId);

        /// <summary>
        /// Removes games that haven't been updated in the specified time span
        /// </summary>
        Task RemoveStaleGamesAsync(TimeSpan maxAge);

        /// <summary>
        /// Checks if a game exists
        /// </summary>
        Task<bool> GameExistsAsync(int gameId);
    }
}
