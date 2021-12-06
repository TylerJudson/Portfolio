using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
    public class WaitingRoomController : Controller
    {
        public static Dictionary<int, IPotentialGame> pendingGames { get; } = new Dictionary<int, IPotentialGame>();


        public IActionResult Index(int gameId, int playerId)
        {
            IPotentialGame game;
            if (pendingGames.TryGetValue(gameId, out game))
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

        [HttpGet]
        public IActionResult EnterGame([FromQuery] int gameId, [FromQuery] string playerName)
        {
            IPotentialGame game;
            if (pendingGames.TryGetValue(gameId, out game))
            {
                int playerId = -1;
                lock(this)
                {
                    playerId = game.PlayerNames.Count;
                    // TODO: Might be good to see if there are duplicate names and if so append some identifier (such as ip address to the name to differentiate them)
                    game.PlayerNames.Add(playerName);
                }
                return Redirect("/WaitingRoom/Index?gameId=" + gameId + "&playerId=" + playerId);
            }
            // Input was bad or game no longer exists.
            return View("Error");
        }


        [HttpGet]
        [Route("WaitingRoom/State/{gameId:int}/{playerId:int}")]
        public JsonResult State(int gameID, int playerId)
        {
            IPotentialGame game;
            if (pendingGames.TryGetValue(gameID, out game))
            {
                return Json(game);
            }
            // TODO: need to do something here so that the other players in the game know the game started and need to be redirect to the game
            IGameBoard activeGame;
            if (GameController.ActiveGames.TryGetValue(gameID, out activeGame))
            {
                return Json("started");
            }
            return Json("");
        }
    }
}
