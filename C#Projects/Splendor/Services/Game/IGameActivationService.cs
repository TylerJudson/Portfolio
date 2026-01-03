using Splendor.Models;

namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for activating pending games into active games
    /// </summary>
    public interface IGameActivationService
    {
        /// <summary>
        /// Activates a pending game, converting it into an active game
        /// </summary>
        /// <param name="gameId">The ID of the pending game to activate</param>
        /// <returns>The activated game board, or null if the pending game doesn't exist</returns>
        Task<IGameBoard?> ActivatePendingGameAsync(int gameId);
    }
}
