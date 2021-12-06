using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
    public class GameController : Controller
    {
        private static IGameBoard GameBoard { get; set; }
        public static Dictionary<int, IGameBoard> ActiveGames { get; set; } = new Dictionary<int, IGameBoard>();

        static GameController() {
            GameBoard = new GameBoard(new List<IPlayer>() { new Player("Bob"), new Player("Jill"), new Player("Zack"), new Player("Sally") });

            GameBoard.Render();

            ActiveGames.Add(0, GameBoard);
        }

        public IActionResult Index(int gameID, int playerID)
        {
            ViewData["GameID"] = gameID;
            ViewData["PlayerID"] = playerID;

            IGameBoard gameBoard;
            if (ActiveGames.TryGetValue(gameID, out gameBoard))
            {
                return View(gameBoard);
            }

            return View(GameBoard);
        }

    
        [HttpGet]
        [Route("Game/State/{gameID:int}/{playerID:int}")]
        public JsonResult State(int gameID, int playerId)
        {
            IGameBoard gameBoard;
            if (ActiveGames.TryGetValue(gameID, out gameBoard))
            {
                return Json(gameBoard);
            }

            return Json(GameBoard);
        }


        [HttpGet]
        [Route("Game/Start")]
        public IActionResult Start([FromQuery]int gameId)
        {
            IPotentialGame gameInfo;
            if (WaitingRoomController.pendingGames.TryGetValue(gameId, out gameInfo))
            {
                List<IPlayer> players = new List<IPlayer>();
                players.Add(new Player(gameInfo.CreatingPlayerName));
                foreach (string playerName in gameInfo.PlayerNames)
                {
                    players.Add(new Player(playerName));
                }
                IGameBoard newGame = new GameBoard(players);
                ActiveGames.Add(gameId, newGame);
                WaitingRoomController.pendingGames.Remove(gameId);


                ViewData["GameID"] = gameId;
                ViewData["PlayerID"] = 0;
                return Redirect("/Game/Index?gameID=" + gameId + "&playerID=0");

            }
            return View("Error");
        }

    }
}
