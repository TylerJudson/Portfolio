using Microsoft.AspNetCore.Mvc;
using SecretHitler.Models;
using SecretHitler.Models.Implementation;

namespace SecretHitler.Controllers
{
    public class WaitingRoomController : Controller
    {
        /// <summary>
        /// The list of potential games waiting to start
        /// </summary>
        public static Dictionary<int, IPotentialGame> PendingGames { get; set; } = new Dictionary<int, IPotentialGame>();


        public IActionResult Index(int gameId, int playerId)
        {
            // Make sure the game id is in the pending games
            if (PendingGames.TryGetValue(gameId, out IPotentialGame? game))
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
        /// Directs to the create a new game page and Creates a new game
        /// </summary>
        /// <returns>The view of the new game</returns>
        public IActionResult NewGame()
        {
            return View();
        }

        /// <summary>
        /// Directs to the ListGame page and Lists the games available to join
        /// </summary>
        /// <returns></returns>
        public IActionResult ListGames()
        {
            List<IPotentialGame> availableGamesToJoin = PendingGames.Values.ToList();
            return View(availableGamesToJoin);
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
                while (PendingGames.ContainsKey(potentialGameId))
                {
                    // Get a new random Id
                    potentialGameId = random.Next();
                }

                // create a new pending game
                IPotentialGame pendingGame = new PotentialGame(potentialGameId, playerName);

                // Add it to the list
                PendingGames.Add(potentialGameId, pendingGame);
            }



            // Remove Pending Games that have been pending for 30 mins

            List<int> pendingGamesToRemove = new List<int>();
            foreach (KeyValuePair<int, IPotentialGame> kvp in PendingGames)
            {
                if (kvp.Value != null)
                {
                    if (kvp.Value.TimeCreated < DateTime.UtcNow.Subtract(new TimeSpan(0, 30, 0)))
                    {
                        pendingGamesToRemove.Add(kvp.Key);
                    }
                }

            }
            // TODO: DOCUMENTATION
            foreach (int pendingGameIdToRemove in pendingGamesToRemove)
            {
                PendingGames.Remove(pendingGameIdToRemove);
            }





            // Go to the game info view
            return Redirect("/WaitingRoom/Index?gameId=" + potentialGameId + "&playerId=0");
        }


        /// <summary>
        /// //TODO: DOCUMENTATION
        /// </summary>
        /// <returns></returns>
        public IActionResult ListGamesState()
        {
            List<IPotentialGame> availableGamesToJoin = PendingGames.Values.ToList();
            availableGamesToJoin = availableGamesToJoin.FindAll(e => e.Players.Count < e.MaxPlayers);
            return Json(availableGamesToJoin);
        }


        /// <summary>
        /// TODO: DOCUMENTATION
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EnterGame([FromQuery] int gameId, [FromQuery] string playerName)
        {
            // Try to get the game
            if (PendingGames.TryGetValue(gameId, out IPotentialGame? game))
            {
                // If there are too many players display a message
                if (game.Players.Count >= game.MaxPlayers)
                {
                    TempData["message"] = "The Game has too many players.";
                    return RedirectToAction("ListGames");
                }

                int playerId = -1;
                lock (this)
                {
                    playerId = game.Players.Keys.Max() + 1;
                    game.Players.Add(playerId, playerName);
                }
                return Redirect("/WaitingRoom/Index?gameId=" + gameId + "&playerId=" + playerId);
            }
            // Input was bad or game no longer exists.
            TempData["message"] = "The Game has already started.";
            return RedirectToAction("ListGames");
        }
    }
}
