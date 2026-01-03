using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Repositories;
using Splendor.Services.Game;
using Splendor.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Splendor.Controllers
{
    // TODO: Add [Authorize] attribute when authentication is implemented
    // This controller handles pre-game lobby and player registration
    // Authentication will be required to prevent malicious game creation/joining
    public class WaitingRoomController : Controller
    {
        private readonly IPendingGameRepository _pendingGameRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameIdGenerator _gameIdGenerator;
        private readonly IPlayerIdAssignmentService _playerIdAssignment;
        private readonly IGameCleanupService _gameCleanup;
        private readonly ILogger<WaitingRoomController> _logger;

        public WaitingRoomController(
            IPendingGameRepository pendingGameRepository,
            IGameRepository gameRepository,
            IGameIdGenerator gameIdGenerator,
            IPlayerIdAssignmentService playerIdAssignment,
            IGameCleanupService gameCleanup,
            ILogger<WaitingRoomController> logger)
        {
            _pendingGameRepository = pendingGameRepository;
            _gameRepository = gameRepository;
            _gameIdGenerator = gameIdGenerator;
            _playerIdAssignment = playerIdAssignment;
            _gameCleanup = gameCleanup;
            _logger = logger;
        }

        /// <summary>
        /// Starts up the game?
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>The view of the game</returns>
        public async Task<IActionResult> Index(
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in WaitingRoom Index: {Errors}", errors);
                return View("Error");
            }

            _logger.LogDebug("WaitingRoom Index called for game {GameId}, player {PlayerId}", gameId, playerId);

            IPotentialGame? game = await _pendingGameRepository.GetPendingGameAsync(gameId);
            if (game != null)
            {
                ViewData["GameId"] = gameId;
                ViewData["PlayerId"] = playerId;
                ViewData["IsCreator"] = playerId == 0 ? "true" : "false";
                return View("Index", game);
            }

            // Input was bad or game no longer exists.
            _logger.LogWarning("Pending game {GameId} not found for player {PlayerId}", gameId, playerId);
            return View("Error");
        }

        /// <summary>
        /// Creates a new game
        /// </summary>
        /// <returns>The view of the new game</returns>
        public IActionResult NewGame()
        {
            return View();
        }

        /// <summary>
        /// Creates a new potential game
        /// </summary>
        /// <param name="playerName">The name of the creator</param>
        /// <returns>The view of the game info</returns>
        [HttpGet]
        public async Task<IActionResult> NewGameInfo([FromQuery, Required, MinLength(1)] string playerName)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in NewGameInfo: {Errors}", errors);
                return View("Error");
            }

            // Sanitize player name to prevent XSS
            string? sanitizedPlayerName = StringSanitizer.SanitizePlayerName(playerName);
            if (sanitizedPlayerName == null)
            {
                _logger.LogWarning("Invalid player name in NewGameInfo: {PlayerName}", playerName);
                return View("Error");
            }

            _logger.LogDebug("NewGameInfo called with player name: {PlayerName}", sanitizedPlayerName);

            int gameId = await _gameIdGenerator.GenerateUniqueIdAsync();
            IPotentialGame pendingGame = new PotentialGame(gameId, sanitizedPlayerName);
            await _pendingGameRepository.AddPendingGameAsync(gameId, pendingGame);

            _logger.LogInformation("New pending game created: gameId={GameId}, creator={PlayerName}", gameId, sanitizedPlayerName);

            await _gameCleanup.RemoveStalePendingGamesAsync();

            return Redirect("/WaitingRoom/Index?gameId=" + gameId + "&playerId=0");
        }

        /// <summary>
        /// Lists the games available to join
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ListGames()
        {
            _logger.LogDebug("ListGames called");

            Dictionary<int, IPotentialGame> pendingGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            List<IPotentialGame> availableGamesToJoin = pendingGames.Values.ToList();

            _logger.LogDebug("Found {GameCount} pending games available to join", availableGamesToJoin.Count);

            return View(availableGamesToJoin);
        }

        public async Task<IActionResult> ListGamesState()
        {
            _logger.LogDebug("ListGamesState called");

            Dictionary<int, IPotentialGame> pendingGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            List<IPotentialGame> availableGamesToJoin = pendingGames.Values
                .Where(e => e.Players.Count < e.MaxPlayers)
                .ToList();

            _logger.LogDebug("Found {GameCount} available games with open slots", availableGamesToJoin.Count);

            return Json(ApiResponse<List<IPotentialGame>>.Ok(availableGamesToJoin));
        }


        [HttpGet]
        public async Task<IActionResult> EnterGame(
            [FromQuery, Range(1, int.MaxValue)] int gameId,
            [FromQuery, Required, MinLength(1)] string playerName)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in EnterGame: {Errors}", errors);
                TempData["message"] = "Invalid game ID or player name.";
                return RedirectToAction("ListGames");
            }

            // Sanitize player name to prevent XSS
            string? sanitizedPlayerName = StringSanitizer.SanitizePlayerName(playerName);
            if (sanitizedPlayerName == null)
            {
                _logger.LogWarning("Invalid player name in EnterGame: {PlayerName}", playerName);
                TempData["message"] = "Invalid player name.";
                return RedirectToAction("ListGames");
            }

            _logger.LogDebug("EnterGame called for game {GameId} by player {PlayerName}", gameId, sanitizedPlayerName);

            IPotentialGame? game = await _pendingGameRepository.GetPendingGameAsync(gameId);
            if (game != null)
            {
                if (game.Players.Count >= game.MaxPlayers)
                {
                    _logger.LogWarning("Cannot enter game {GameId}: too many players ({CurrentCount}/{MaxCount})", gameId, game.Players.Count, game.MaxPlayers);
                    TempData["message"] = "The Game has too many players.";
                    return RedirectToAction("ListGames");
                }

                int playerId = _playerIdAssignment.GetNextPlayerId(game);
                game.AddPlayer(playerId, sanitizedPlayerName);
                await _pendingGameRepository.UpdatePendingGameAsync(gameId, game);

                _logger.LogInformation("Player {PlayerName} (ID {PlayerId}) joined game {GameId}", sanitizedPlayerName, playerId, gameId);

                return Redirect("/WaitingRoom/Index?gameId=" + gameId + "&playerId=" + playerId);
            }

            // Input was bad or game no longer exists.
            _logger.LogWarning("Cannot enter game {GameId}: game not found or already started", gameId);
            TempData["message"] = "The Game has already started.";
            return RedirectToAction("ListGames");
        }


        /// <summary>
        /// Gets the state of the game info
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>The potential game</returns>
        [HttpGet]
        [Route("WaitingRoom/State/{gameId:int}/{playerId:int}")]
        public async Task<JsonResult> State(
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in WaitingRoom State: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            _logger.LogDebug("WaitingRoom State called for game {GameId}, player {PlayerId}", gameId, playerId);

            IPotentialGame? game = await _pendingGameRepository.GetPendingGameAsync(gameId);
            if (game != null)
            {
                if (!game.Players.ContainsKey(playerId))
                {
                    _logger.LogWarning("Player {PlayerId} was removed from game {GameId}", playerId, gameId);
                    return Json(ApiResponse<string>.Ok("removed from game"));
                }

                return Json(ApiResponse<IPotentialGame>.Ok(game));
            }

            // If the game is in the active games then start the game for the other players
            bool gameExists = await _gameRepository.GameExistsAsync(gameId);
            if (gameExists)
            {
                _logger.LogDebug("Game {GameId} has started", gameId);
                return Json(ApiResponse<string>.Ok("started"));
            }

            _logger.LogWarning("Game {GameId} not found in WaitingRoom State", gameId);
            return Json(ApiResponse<string>.Fail("Game not found", 404));
        }

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>The potential game</returns>
        [HttpGet]
        [Route("WaitingRoom/RemovePlayer/{gameId:int}/{playerId:int}")]
        public async Task<JsonResult> RemovePlayer(
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in RemovePlayer: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            _logger.LogDebug("RemovePlayer called for game {GameId}, player {PlayerId}", gameId, playerId);

            IPotentialGame? game = await _pendingGameRepository.GetPendingGameAsync(gameId);
            if (game == null)
            {
                _logger.LogWarning("Game {GameId} not found in RemovePlayer", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            game.RemovePlayer(playerId);
            await _pendingGameRepository.UpdatePendingGameAsync(gameId, game);

            _logger.LogInformation("Player {PlayerId} removed from game {GameId}", playerId, gameId);

            return Json(ApiResponse<IPotentialGame>.Ok(game));
        }
    }
}
