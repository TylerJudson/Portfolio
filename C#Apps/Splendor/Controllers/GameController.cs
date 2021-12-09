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

            IGameBoard gameBoard;
            if (ActiveGames.TryGetValue(gameId, out gameBoard))
            {
                return View(gameBoard);
            }

            return View("Error");
        }

    
        [HttpGet]
        [Route("Game/State/{gameId:int}/{playerId:int}")]
        public JsonResult State(int gameId, int playerId)
        {
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                return Json(gameBoard);
            }

            return Json(null);
        }


        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Game/Start")]
        public IActionResult Start([FromQuery]int gameId)
        {
            if (WaitingRoomController.pendingGames.TryGetValue(gameId, out IPotentialGame? gameInfo))
            {
                List<IPlayer> players = new List<IPlayer>();
                foreach (string playerName in gameInfo.PlayerNames)
                {
                    players.Add(new Player(playerName));
                }
                IGameBoard newGame = new GameBoard(players);
                ActiveGames.Add(gameId, newGame);
                WaitingRoomController.pendingGames.Remove(gameId);


                ViewData["GameId"] = gameId;
                ViewData["PlayerId"] = 0;
                return Redirect("/Game/Index?gameId=" + gameId + "&playerId=0");

            }
            return View("Error");
        }

    }
}
