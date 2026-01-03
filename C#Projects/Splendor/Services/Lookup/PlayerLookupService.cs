using Splendor.Models;

namespace Splendor.Services.Lookup
{
    /// <summary>
    /// Service for looking up players in a game
    /// </summary>
    public class PlayerLookupService : IPlayerLookupService
    {
        /// <summary>
        /// Finds a player by their ID
        /// </summary>
        /// <param name="board">The game board containing the players</param>
        /// <param name="playerId">The ID of the player to find</param>
        /// <returns>The player if found, null otherwise</returns>
        public IPlayer? FindPlayerById(IGameBoard board, int playerId)
        {
            // Search through all players in the game
            foreach (var player in board.Players)
            {
                if (player.Id == playerId)
                {
                    return player;
                }
            }

            return null;
        }
    }
}
