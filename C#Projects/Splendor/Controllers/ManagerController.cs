using Microsoft.AspNetCore.Mvc;
using Splendor.Models;

namespace Splendor.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View(new Manager(GameController.ActiveGames, WaitingRoomController.PendingGames));
        }




        [HttpGet]
        public IActionResult DeletePendingGame(int id)
        {
            WaitingRoomController.PendingGames.Remove(id);

            return Redirect("~/manager");
        }


        [HttpGet]
        public IActionResult DeleteActiveGame(int id)
        {
            GameController.ActiveGames.Remove(id);

            return Redirect("~/manager");
        }
    }
}
