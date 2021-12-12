using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
    public class WaitingRoomController : Controller
    {
        /// <summary>
        /// The list of potential games waiting to start
        /// </summary>
        public static Dictionary<int, IPotentialGame> pendingGames { get; } = new Dictionary<int, IPotentialGame>();


        public IActionResult Index(int gameId, int playerId)
        {
            if (pendingGames.TryGetValue(gameId, out IPotentialGame? game))
            {
                ViewData["GameId"] = gameId;
                ViewData["PlayerId"] = playerId;
                ViewData["IsCreator"] = playerId == 0 ? "true" : "false";
                return View("Index", game);
            }
            // Input was bad or game no longer exists.
            return View("Error");
        }

        public IActionResult NewGame()
        {
            return View();
        }

        /// <summary>
        /// Creates a new potential game
        /// </summary>
        /// <param name="playerName">The name of the creator</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult NewGameInfo([FromQuery] string playerName)
        {
            Random random = new Random();
            int potentialGameId = -1;
            lock (this)
            {
                potentialGameId = random.Next();
                while (pendingGames.ContainsKey(potentialGameId))
                {
                    potentialGameId = random.Next();
                }
                IPotentialGame pendingGame = new PotentialGame(potentialGameId, playerName);
                pendingGames.Add(potentialGameId, pendingGame);

            }
            return Redirect("/WaitingRoom/Index?gameId=" + potentialGameId + "&playerId=0");
        }

        public IActionResult ListGames()
        {
            List<IPotentialGame> availableGamesToJoin = pendingGames.Values.ToList();
            return View(availableGamesToJoin);
        }

        public IActionResult ListGamesState()
        {
            List<IPotentialGame> availableGamesToJoin = pendingGames.Values.ToList();
            availableGamesToJoin = availableGamesToJoin.FindAll(e => e.Players.Count < e.MaxPlayers);
            return Json(availableGamesToJoin);
        }


        [HttpGet]
        public IActionResult EnterGame([FromQuery] int gameId, [FromQuery] string playerName)
        {
            // Try to get the game
            if (pendingGames.TryGetValue(gameId, out IPotentialGame? game))
            {
                // If there are too many players display a message
                if (game.Players.Count >= game.MaxPlayers)
                {
                    TempData["message"] = "The Game has too many players.";
                    return RedirectToAction("ListGames");
                }

                int playerId = -1;
                lock(this)
                {
                    playerId = game.Players.Keys.Max() + 1;
                    // TODO: Might be good to see if there are duplicate names and if so append some identifier (such as ip address to the name to differentiate them)
                    game.Players.Add(playerId, playerName);
                }
                return Redirect("/WaitingRoom/Index?gameId=" + gameId + "&playerId=" + playerId);
            }
            // Input was bad or game no longer exists.
            TempData["message"] = "The Game has already started.";
            return RedirectToAction("ListGames");
        }


        [HttpGet]
        [Route("WaitingRoom/State/{gameId:int}/{playerId:int}")]
        public JsonResult State(int gameID, int playerId)
        {

            if (pendingGames.TryGetValue(gameID, out IPotentialGame? game))
            {
                if (!game.Players.ContainsKey(playerId))
                {
                    return Json("removed from game");
                }
                return Json(game);
            }
            if (GameController.ActiveGames.TryGetValue(gameID, out IGameBoard? activeGame))
            {
                return Json("started");
            }
            
            return Json("");
        }

        [HttpGet]
        [Route("WaitingRoom/RemovePlayer/{gameId:int}/{playerId:int}")]
        public JsonResult RemovePlayer(int gameID, int playerId)
        {

            if (pendingGames.TryGetValue(gameID, out IPotentialGame? game))
            {
                game.Players.Remove(playerId);

                return Json(game);
            }
            
            return Json("");
        }
    }
}
