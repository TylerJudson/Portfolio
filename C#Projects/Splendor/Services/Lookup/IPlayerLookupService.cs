using Splendor.Models;

namespace Splendor.Services.Lookup
{
    /// <summary>
    /// Service for looking up players in a game
    /// </summary>
    public interface IPlayerLookupService
    {
        /// <summary>
        /// Finds a player by their ID
        /// </summary>
        /// <param name="board">The game board containing the players</param>
        /// <param name="playerId">The ID of the player to find</param>
        /// <returns>The player if found, null otherwise</returns>
        IPlayer? FindPlayerById(IGameBoard board, int playerId);
    }
}
