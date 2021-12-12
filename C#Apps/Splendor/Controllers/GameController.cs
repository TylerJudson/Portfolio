using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
    public class GameController : Controller
    {
        public static Dictionary<int, IGameBoard> ActiveGames { get; set; } = new Dictionary<int, IGameBoard>();

        public IActionResult Index(int gameId, int playerId)
        {
            ViewData["GameId"] = gameId;
            ViewData["PlayerId"] = playerId;

            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                return View(gameBoard);
            }

            return View("Error");
        }



        /// <summary>
        /// Returns information to update the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <param name="playerId">The Id of the player</param>
        /// <returns>Json object of how to update the game</returns>
        [HttpGet]
        [Route("Game/State/{gameId:int}/{playerId:int}")]
        public JsonResult State(int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                return Json(gameBoard);
            }

            // The game has ended if the game isn't active
            return Json("The game has ended.");
        }


        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <returns>A View of what to display</returns>
        [HttpGet]
        [Route("Game/Start")]
        public IActionResult Start([FromQuery]int gameId)
        {
            // Get the Game out of pending games
            if (WaitingRoomController.pendingGames.TryGetValue(gameId, out IPotentialGame? gameInfo))
            {
                // Create a list of the players
                List<IPlayer> players = new List<IPlayer>();

                // Populate the list with the player names
                foreach (string playerName in gameInfo.Players.Values)
                {
                    players.Add(new Player(playerName));
                }

                // Create a new game
                IGameBoard newGame = new GameBoard(players);

                // Add the new game to active games
                ActiveGames.Add(gameId, newGame);

                // Remove the game from pending games
                WaitingRoomController.pendingGames.Remove(gameId);

                // Navigate to the game
                ViewData["GameId"] = gameId;
                ViewData["PlayerId"] = 0;

                return Redirect("/Game/Index?gameId=" + gameId + "&playerId=0");

            }
            return View("Error");
        }

    }
}
