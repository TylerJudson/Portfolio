using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
    public class GameController : Controller
    {
        private static IGameBoard GameBoard { get; set; }

        static GameController() {
            GameBoard = new GameBoard(new List<IPlayer>() { new Player("Bob"), new Player("Jill"), new Player("Zack"), new Player("Sally") });

            GameBoard.Render();
        }

        public IActionResult Index(uint gameID, uint playerID)
        {
            ViewData["GameID"] = gameID;
            ViewData["PlayerID"] = playerID;
            return View(GameBoard);
        }

    
        [HttpGet]
        [Route("Game/State/{gameID:int}/{playerID:int}")]
        public JsonResult State(int gameID, int playerId)
        {
            return Json(GameBoard);
        }
        

    }
}
