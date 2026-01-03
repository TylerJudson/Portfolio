using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Repositories;
using Splendor.Services.Data;
using Splendor.Services.Lookup;
using Splendor.Services.Game;
using Splendor.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Splendor.Controllers
{
    // TODO: Add [Authorize] attribute when authentication is implemented
    // This controller handles game state and player actions
    // Authentication will be required to ensure players can only act on their own behalf
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameDataService _gameDataService;
        private readonly ICardLookupService _cardLookup;
        private readonly INobleLookupService _nobleLookup;
        private readonly IPlayerLookupService _playerLookup;
        private readonly IGameActivationService _gameActivation;
        private readonly IGameCleanupService _gameCleanup;
        private readonly ILogger<GameController> _logger;

        public GameController(
            IGameRepository gameRepository,
            IGameDataService gameDataService,
            ICardLookupService cardLookup,
            INobleLookupService nobleLookup,
            IPlayerLookupService playerLookup,
            IGameActivationService gameActivation,
            IGameCleanupService gameCleanup,
            ILogger<GameController> logger)
        {
            _gameRepository = gameRepository;
            _gameDataService = gameDataService;
            _cardLookup = cardLookup;
            _nobleLookup = nobleLookup;
            _playerLookup = playerLookup;
            _gameActivation = gameActivation;
            _gameCleanup = gameCleanup;
            _logger = logger;
        }

        /// <summary>
        /// Renders the Game
        /// </summary>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The view of the gameboard</returns>
        public async Task<IActionResult> Index(
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Index: {Errors}", errors);
                return View("Error");
            }

            _logger.LogDebug("Index called for game {GameId}, player {PlayerId}", gameId, playerId);

            ViewData["GameId"] = gameId;
            ViewData["PlayerId"] = playerId;

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard != null)
            {
                return View(gameBoard);
            }

            _logger.LogWarning("Game {GameId} not found for player {PlayerId}", gameId, playerId);
            return Redirect("~/ ");
        }



        /// <summary>
        /// Returns information to update the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>Json object of how to update the game</returns>
        [HttpGet]
        [Route("Game/State/{gameId:int}/{playerId:int}")]
        public async Task<JsonResult> State(
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in State: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            _logger.LogDebug("State called for game {GameId}, player {PlayerId}", gameId, playerId);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard != null)
            {
                if (gameBoard.IsPaused)
                {
                    _logger.LogDebug("Game {GameId} is paused", gameId);
                    return Json(ApiResponse<string>.Ok("IsPaused"));
                }
                return Json(ApiResponse<int>.Ok(gameBoard.Version));
            }

            // The game has ended if the game isn't active
            _logger.LogDebug("Game {GameId} not found (game has ended)", gameId);
            return Json(ApiResponse<string>.Ok("The game has ended."));
        }


        /// <summary>
        /// The end point for when the player ends their turn
        /// </summary>
        /// <param name="TakenTokens">The tokens the player took during their turn</param>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The game</returns>
        [HttpPost]
        [Route("Game/EndTurn/{gameId:int}/{playerId:int}")]
        [RequestSizeLimit(10240)] // 10KB max request size
        public async Task<JsonResult> EndTurn(
            [FromBody] Dictionary<Token, int> TakenTokens,
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in EndTurn: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            _logger.LogDebug("EndTurn called for game {GameId}, player {PlayerId}", gameId, playerId);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard == null)
            {
                _logger.LogWarning("Game {GameId} not found in EndTurn", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            if (TakenTokens != null)
            {
                ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(TakenTokens));

                if (completedTurn.Error != null)
                {
                    _logger.LogWarning("Turn error in game {GameId}: {Error}", gameId, completedTurn.Error.Message);
                    return Json(ApiResponse<IError>.Ok(completedTurn.Error));
                }
                else if (completedTurn.ContinueAction != null)
                {
                    _logger.LogDebug("Turn in game {GameId} requires continue action", gameId);
                    return Json(ApiResponse<IContinueAction>.Ok(completedTurn.ContinueAction));
                }
            }

            _logger.LogDebug("Turn completed successfully for game {GameId}, player {PlayerId}", gameId, playerId);
            return Json(ApiResponse<IGameBoard>.Ok(gameBoard));
        }



        /// <summary>
        /// THe end point for when a player purchases a card
        /// </summary>
        /// <param name="ImageName">The name of the image for the card to purchase</param>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The game</returns>
        [HttpPost]
        [Route("Game/Purchase/{gameId:int}/{playerId:int}")]
        [RequestSizeLimit(10240)] // 10KB max request size
        public async Task<JsonResult> Purchase(
            [FromBody, Required, MinLength(1)] string ImageName,
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Purchase: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            // Sanitize the image name to prevent path traversal
            string? sanitizedImageName = StringSanitizer.SanitizeImageName(ImageName);
            if (sanitizedImageName == null)
            {
                _logger.LogWarning("Invalid ImageName in Purchase call: {ImageName}", ImageName);
                return Json(ApiResponse<string>.Fail("Invalid image name format", 400));
            }

            _logger.LogDebug("Purchase called for game {GameId}, player {PlayerId}, card {ImageName}", gameId, playerId, sanitizedImageName);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard == null)
            {
                _logger.LogWarning("Game {GameId} not found in Purchase", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            ICard? card = _cardLookup.FindCardByImageName(gameBoard, sanitizedImageName);

            if (card == null)
            {
                IPlayer? player = _playerLookup.FindPlayerById(gameBoard, playerId);
                if (player != null)
                {
                    card = _cardLookup.FindReservedCardByImageName(player, sanitizedImageName);
                }
            }

            ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(card));

            if (completedTurn.Error != null)
            {
                _logger.LogWarning("Purchase error in game {GameId}: {Error}", gameId, completedTurn.Error.Message);
                return Json(ApiResponse<IError>.Ok(completedTurn.Error));
            }
            else if (completedTurn.ContinueAction != null)
            {
                _logger.LogDebug("Purchase in game {GameId} requires continue action", gameId);
                return Json(ApiResponse<IContinueAction>.Ok(completedTurn.ContinueAction));
            }

            _logger.LogDebug("Purchase completed successfully for game {GameId}, player {PlayerId}", gameId, playerId);
            return Json(ApiResponse<IGameBoard>.Ok(gameBoard));
        }


        /// <summary>
        /// The endpoint for when the player reserves a card
        /// </summary>
        /// <param name="ImageName">The name of the image for the cards wanting to purchase</param>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the plaeyr</param>
        /// <returns>the game</returns>
        [HttpPost]
        [Route("Game/Reserve/{gameId:int}/{playerId:int}")]
        [RequestSizeLimit(10240)] // 10KB max request size
        public async Task<JsonResult> Reserve(
            [FromBody, Required, MinLength(1)] string ImageName,
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Reserve: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            // Sanitize the image name to prevent path traversal
            string? sanitizedImageName = StringSanitizer.SanitizeImageName(ImageName);
            if (sanitizedImageName == null)
            {
                _logger.LogWarning("Invalid ImageName in Reserve call: {ImageName}", ImageName);
                return Json(ApiResponse<string>.Fail("Invalid image name format", 400));
            }

            _logger.LogDebug("Reserve called for game {GameId}, player {PlayerId}, card {ImageName}", gameId, playerId, sanitizedImageName);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard == null)
            {
                _logger.LogWarning("Game {GameId} not found in Reserve", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            ICard? card = _cardLookup.FindCardByImageName(gameBoard, sanitizedImageName);

            ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(card, true));

            if (completedTurn.Error != null)
            {
                _logger.LogWarning("Reserve error in game {GameId}: {Error}", gameId, completedTurn.Error.Message);
                return Json(ApiResponse<IError>.Ok(completedTurn.Error));
            }
            else if (completedTurn.ContinueAction != null)
            {
                _logger.LogDebug("Reserve in game {GameId} requires continue action", gameId);
                return Json(ApiResponse<IContinueAction>.Ok(completedTurn.ContinueAction));
            }

            _logger.LogDebug("Reserve completed successfully for game {GameId}, player {PlayerId}", gameId, playerId);
            return Json(ApiResponse<IGameBoard>.Ok(gameBoard));
        }



        [HttpPost]
        [Route("Game/Noble/{gameId:int}/{playerId:int}")]
        [RequestSizeLimit(10240)] // 10KB max request size
        public async Task<JsonResult> Noble(
            [FromBody, Required, MinLength(1)] string ImageName,
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Noble: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            // Sanitize the image name to prevent path traversal
            string? sanitizedImageName = StringSanitizer.SanitizeImageName(ImageName);
            if (sanitizedImageName == null)
            {
                _logger.LogWarning("Invalid ImageName in Noble call: {ImageName}", ImageName);
                return Json(ApiResponse<string>.Fail("Invalid image name format", 400));
            }

            _logger.LogDebug("Noble called for game {GameId}, player {PlayerId}, noble {ImageName}", gameId, playerId, sanitizedImageName);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard == null)
            {
                _logger.LogWarning("Game {GameId} not found in Noble", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            INoble? noble = _nobleLookup.FindNobleByImageName(gameBoard, sanitizedImageName);

            ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(noble));

            if (completedTurn.Error != null)
            {
                _logger.LogWarning("Noble error in game {GameId}: {Error}", gameId, completedTurn.Error.Message);
                return Json(ApiResponse<IError>.Ok(completedTurn.Error));
            }
            else if (completedTurn.ContinueAction != null)
            {
                _logger.LogDebug("Noble in game {GameId} requires continue action", gameId);
                return Json(ApiResponse<IContinueAction>.Ok(completedTurn.ContinueAction));
            }

            _logger.LogDebug("Noble completed successfully for game {GameId}, player {PlayerId}", gameId, playerId);
            return Json(ApiResponse<IGameBoard>.Ok(gameBoard));
        }


        [HttpPost]
        [Route("Game/Return/{gameId:int}/{playerId:int}")]
        [RequestSizeLimit(10240)] // 10KB max request size
        public async Task<JsonResult> Return(
            [FromBody, Required] ReturnRequest returnRequest,
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Return: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            _logger.LogDebug("Return called for game {GameId}, player {PlayerId}", gameId, playerId);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard == null)
            {
                _logger.LogWarning("Game {GameId} not found in Return", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            ICard? card = null;
            if (returnRequest.ReservingCardImageName != null)
            {
                // Sanitize the image name to prevent path traversal
                string? sanitizedImageName = StringSanitizer.SanitizeImageName(returnRequest.ReservingCardImageName);
                if (sanitizedImageName == null)
                {
                    _logger.LogWarning("Invalid ReservingCardImageName in Return call: {ImageName}", returnRequest.ReservingCardImageName);
                    return Json(ApiResponse<string>.Fail("Invalid image name format", 400));
                }
                card = _cardLookup.FindCardByImageName(gameBoard, sanitizedImageName);
            }

            ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(returnRequest.Tokens, card));

            if (completedTurn.Error != null)
            {
                _logger.LogWarning("Return error in game {GameId}: {Error}", gameId, completedTurn.Error.Message);
                return Json(ApiResponse<IError>.Ok(completedTurn.Error));
            }
            else if (completedTurn.ContinueAction != null)
            {
                _logger.LogDebug("Return in game {GameId} requires continue action", gameId);
                return Json(ApiResponse<IContinueAction>.Ok(completedTurn.ContinueAction));
            }

            _logger.LogDebug("Return completed successfully for game {GameId}, player {PlayerId}", gameId, playerId);
            return Json(ApiResponse<IGameBoard>.Ok(gameBoard));
        }

        [HttpPost]
        [Route("Game/Pause/{gameId:int}/{playerId:int}")]
        public async Task<JsonResult> Pause(
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Pause: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            _logger.LogDebug("Pause called for game {GameId}, player {PlayerId}", gameId, playerId);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard == null)
            {
                _logger.LogWarning("Game {GameId} not found in Pause", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            gameBoard.IsPaused = true;
            _logger.LogInformation("Game {GameId} paused by player {PlayerId}", gameId, playerId);
            return Json(ApiResponse<string>.Ok("Game paused"));
        }

        [HttpPost]
        [Route("Game/Resume/{gameId:int}/{playerId:int}")]
        public async Task<JsonResult> Resume(
            [Range(1, int.MaxValue)] int gameId,
            [Range(0, int.MaxValue)] int playerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Resume: {Errors}", errors);
                return Json(ApiResponse<string>.Fail($"Validation failed: {errors}", 400));
            }

            _logger.LogDebug("Resume called for game {GameId}, player {PlayerId}", gameId, playerId);

            IGameBoard? gameBoard = await _gameRepository.GetGameAsync(gameId);
            if (gameBoard == null)
            {
                _logger.LogWarning("Game {GameId} not found in Resume", gameId);
                return Json(ApiResponse<string>.Fail("Game not found", 404));
            }

            gameBoard.IsPaused = false;
            _logger.LogInformation("Game {GameId} resumed by player {PlayerId}", gameId, playerId);
            return Json(ApiResponse<string>.Ok("Game resumed"));
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <returns>Redericts to the index view</returns>
        [HttpGet]
        [Route("Game/Start")]
        public async Task<IActionResult> Start([FromQuery, Range(1, int.MaxValue)] int gameId)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in Start: {Errors}", errors);
                return View("Error");
            }

            _logger.LogDebug("Start called for game {GameId}", gameId);

            IGameBoard? newGame = await _gameActivation.ActivatePendingGameAsync(gameId);
            if (newGame != null)
            {
                _logger.LogInformation("Game {GameId} started successfully", gameId);
                await _gameCleanup.RemoveStaleGamesAsync();
                return Redirect("/Game/Index?gameId=" + gameId + "&playerId=0");
            }

            _logger.LogWarning("Failed to start game {GameId}: activation returned null", gameId);
            return View("Error");
        }

    }
}
