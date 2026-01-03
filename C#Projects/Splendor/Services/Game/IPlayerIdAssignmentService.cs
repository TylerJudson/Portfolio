using Splendor.Models;

namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for assigning player IDs in pending games
    /// </summary>
    public interface IPlayerIdAssignmentService
    {
        /// <summary>
        /// Gets the next available player ID for a game
        /// </summary>
        /// <param name="game">The pending game</param>
        /// <returns>The next available player ID</returns>
        int GetNextPlayerId(IPotentialGame game);
    }
}
