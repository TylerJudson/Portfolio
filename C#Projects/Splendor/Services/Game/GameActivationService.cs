using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Repositories;
using Splendor.Services.Data;

namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for activating pending games into active games
    /// </summary>
    public class GameActivationService : IGameActivationService
    {
        private readonly IPendingGameRepository _pendingGameRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameDataService _gameDataService;
        private readonly ILogger<GameActivationService> _logger;

        public GameActivationService(
            IPendingGameRepository pendingGameRepository,
            IGameRepository gameRepository,
            IGameDataService gameDataService,
            ILogger<GameActivationService> logger)
        {
            _pendingGameRepository = pendingGameRepository;
            _gameRepository = gameRepository;
            _gameDataService = gameDataService;
            _logger = logger;
        }

        /// <summary>
        /// Activates a pending game, converting it into an active game
        /// </summary>
        /// <param name="gameId">The ID of the pending game to activate</param>
        /// <returns>The activated game board, or null if the pending game doesn't exist</returns>
        public async Task<IGameBoard?> ActivatePendingGameAsync(int gameId)
        {
            _logger.LogDebug("Activating pending game {GameId}", gameId);

            // Get the pending game
            var pendingGame = await _pendingGameRepository.GetPendingGameAsync(gameId);
            if (pendingGame == null)
            {
                _logger.LogWarning("Failed to activate game {GameId}: pending game not found", gameId);
                return null;
            }

            // Create a list of players from the pending game
            var players = new List<IPlayer>();
            foreach (var kvp in pendingGame.Players)
            {
                players.Add(new Player(kvp.Value, kvp.Key));
            }

            // Shuffle the players randomly to determine turn order
            var random = Random.Shared;
            players = players.OrderBy(p => random.Next()).ToList();

            // Create the new game board
            var gameBoard = new GameBoard(players, _gameDataService);

            // Add the game to the active games repository
            await _gameRepository.AddGameAsync(gameId, gameBoard);

            // Remove the game from pending games
            await _pendingGameRepository.RemovePendingGameAsync(gameId);

            _logger.LogInformation("Game {GameId} activated with {PlayerCount} players", gameId, players.Count);

            return gameBoard;
        }
    }
}
