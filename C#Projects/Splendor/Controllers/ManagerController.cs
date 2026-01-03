using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Splendor.Controllers
{
    // TODO: Add [Authorize(Roles = "Admin")] attribute when authentication is implemented
    // This controller provides administrative functions for managing games
    // CRITICAL: This must be protected with admin-only authentication before production use
    public class ManagerController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPendingGameRepository _pendingGameRepository;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(
            IGameRepository gameRepository,
            IPendingGameRepository pendingGameRepository,
            ILogger<ManagerController> logger)
        {
            _gameRepository = gameRepository;
            _pendingGameRepository = pendingGameRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogDebug("Manager Index called");

            Dictionary<int, IGameBoard> activeGames = await _gameRepository.GetAllGamesAsync();
            Dictionary<int, IPotentialGame> pendingGames = await _pendingGameRepository.GetAllPendingGamesAsync();

            _logger.LogDebug("Manager displaying {ActiveCount} active games and {PendingCount} pending games", activeGames.Count, pendingGames.Count);

            return View(new Manager(activeGames, pendingGames));
        }

        [HttpGet]
        public async Task<IActionResult> DeletePendingGame([Range(1, int.MaxValue)] int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in DeletePendingGame: {Errors}", errors);
                return Redirect("~/manager");
            }

            _logger.LogDebug("DeletePendingGame called for game {GameId}", id);

            await _pendingGameRepository.RemovePendingGameAsync(id);

            _logger.LogInformation("Pending game {GameId} deleted", id);

            return Redirect("~/manager");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteActiveGame([Range(1, int.MaxValue)] int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed in DeleteActiveGame: {Errors}", errors);
                return Redirect("~/manager");
            }

            _logger.LogDebug("DeleteActiveGame called for game {GameId}", id);

            await _gameRepository.RemoveGameAsync(id);

            _logger.LogInformation("Active game {GameId} deleted", id);

            return Redirect("~/manager");
        }
    }
}
