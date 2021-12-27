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

        /// <summary>
        /// Starts up the game?
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>The view of the game</returns>
        public IActionResult Index(int gameId, int playerId)
        {
            // Make sure the game id is in the pending games
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
        public IActionResult NewGameInfo([FromQuery] string playerName)
        {
            Random random = new Random();
            int potentialGameId = -1;
            lock (this)
            {
                // Get a random id
                potentialGameId = random.Next();
                
                // Make sure the id isn't already in pending games
                while (pendingGames.ContainsKey(potentialGameId))
                {
                    // Get a new random Id
                    potentialGameId = random.Next();
                }

                // create a new pending game
                IPotentialGame pendingGame = new PotentialGame(potentialGameId, playerName);

                // Add it to the list
                pendingGames.Add(potentialGameId, pendingGame);

            }
            // Go to the game info view
            return Redirect("/WaitingRoom/Index?gameId=" + potentialGameId + "&playerId=0");
        }

        /// <summary>
        /// Lists the games available to join
        /// </summary>
        /// <returns></returns>
        public IActionResult ListGames()
        {
            List<IPotentialGame> availableGamesToJoin = pendingGames.Values.ToList();
            return View(availableGamesToJoin);
        }

        public IActionResult ListGamesState()
        {
            // TODO - documentation
            List<IPotentialGame> availableGamesToJoin = pendingGames.Values.ToList();
            availableGamesToJoin = availableGamesToJoin.FindAll(e => e.Players.Count < e.MaxPlayers);
            return Json(availableGamesToJoin);
        }


        [HttpGet]
        public IActionResult EnterGame([FromQuery] int gameId, [FromQuery] string playerName)
        {
            // TODO - documentation
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


        /// <summary>
        /// Gets the state of the game info
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>The potential game</returns>
        [HttpGet]
        [Route("WaitingRoom/State/{gameId:int}/{playerId:int}")]
        public JsonResult State(int gameId, int playerId)
        {
            // Try to get the game from pending game
            if (pendingGames.TryGetValue(gameId, out IPotentialGame? game))
            {
                // Make sure the game has the playerId
                if (!game.Players.ContainsKey(playerId))
                {
                    return Json("removed from game");
                }

                return Json(game);
            }
            // If the game is in the active games then start the game for the other players
            if (GameController.ActiveGames.TryGetValue(gameId, out IGameBoard? activeGame))
            {
                return Json("started");
            }
            
            return Json("");
        }

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>The potential game</returns>
        [HttpGet]
        [Route("WaitingRoom/RemovePlayer/{gameId:int}/{playerId:int}")]
        public JsonResult RemovePlayer(int gameId, int playerId)
        {
            // Make sure the game is in pending games
            if (pendingGames.TryGetValue(gameId, out IPotentialGame? game))
            {
                // Remove the player
                game.Players.Remove(playerId);
                return Json(game);
            }
            
            return Json("");
        }
    }
}
