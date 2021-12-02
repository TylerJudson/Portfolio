using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
    public class GameController : Controller
    {
        private static IGameBoard GameBoard { get; set; }

        public GameController() {
            GameBoard = new GameBoard(new List<IPlayer>() { new Player("Bob"), new Player("Jill"), new Player("Zack") });

            GameBoard.Render();
        }

        public IActionResult Index(uint gameID, uint playerID)
        {
            ViewData["GameID"] = gameID;
            ViewData["PlayerID"] = playerID;
            return View(GameBoard);
        }

    

        

    }
}
