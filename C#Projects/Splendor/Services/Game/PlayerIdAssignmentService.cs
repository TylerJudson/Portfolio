using Splendor.Models;

namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for assigning player IDs in pending games
    /// </summary>
    public class PlayerIdAssignmentService : IPlayerIdAssignmentService
    {
        private readonly ILogger<PlayerIdAssignmentService> _logger;

        public PlayerIdAssignmentService(ILogger<PlayerIdAssignmentService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the next available player ID for a game
        /// </summary>
        /// <param name="game">The pending game</param>
        /// <returns>The next available player ID</returns>
        public int GetNextPlayerId(IPotentialGame game)
        {
            int playerId;
            if (game.Players.Count == 0)
            {
                playerId = 0;
            }
            else
            {
                // Get the maximum player ID and add 1
                playerId = game.Players.Keys.Max() + 1;
            }

            _logger.LogDebug("Assigned player ID {PlayerId} for game with {PlayerCount} existing players", playerId, game.Players.Count);
            return playerId;
        }
    }
}
