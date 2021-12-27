using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
    // TODO - Documentation
    public class GameController : Controller
    {
        /// <summary>
        /// The active games currently being played
        /// </summary>
        public static Dictionary<int, IGameBoard> ActiveGames { get; set; } = new Dictionary<int, IGameBoard>();

        /// <summary>
        /// Renders the Game
        /// </summary>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The view of the gameboard</returns>
        public IActionResult Index(int gameId, int playerId)
        {
            ViewData["GameId"] = gameId;
            ViewData["PlayerId"] = playerId;

            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                return View(gameBoard);
            }

            return Redirect("~/ ");
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

        [HttpPost]
        [Route("Game/EndTurn/{gameId:int}/{playerId:int}")]
        public JsonResult EndTurn([FromBody] Dictionary<Token, int> TakenTokens, int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                if (TakenTokens != null)
                {

                    ICompletedTurn completedTurn = gameBoard.ExecuteTurn( new Turn(TakenTokens));

                    if (completedTurn.Error != null)
                    {
                        return Json(completedTurn.Error);
                    }
                    else if (completedTurn.ContinueAction != null)
                    {
                        return Json(completedTurn.ContinueAction);
                    }
                }


                return Json(gameBoard);
            }


            return Json("");
        }



        [HttpPost]
        [Route("Game/Purchase/{gameId:int}/{playerId:int}")]
        public JsonResult Purchase([FromBody] string ImageName, int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                if (ImageName != null)
                {
                    ICard Card = null;

                    if (ImageName[5] == '1')
                    {
                        foreach(ICard card in gameBoard.Level1Cards)
                        {
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    else if (ImageName[5] == '2')
                    {
                        foreach (ICard card in gameBoard.Level2Cards)
                        {
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    else if (ImageName[5] == '3')
                    {
                        foreach (ICard card in gameBoard.Level3Cards)
                        {
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    
                    if (Card == null)
                    {
                        // Check the reserved cards first
                        int playerIndex = 0;
                        for (int i = 0; i < gameBoard.Players.Count; i++)
                        {
                            if (gameBoard.Players[i].Id == playerId)
                            {
                                playerIndex = i;
                            }
                        }
                        foreach (ICard card in gameBoard.Players[playerIndex].ReservedCards)
                        {
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                        
                    ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(Card));

                    if (completedTurn.Error != null)
                    {
                        return Json(completedTurn.Error);
                    }
                    else if (completedTurn.ContinueAction != null)
                    {
                        return Json(completedTurn.ContinueAction);
                    }
                }
                return Json(gameBoard);
            }


            return Json("");
        }


        [HttpPost]
        [Route("Game/Reserve/{gameId:int}/{playerId:int}")]
        public JsonResult Reserve([FromBody] string ImageName, int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                if (ImageName != null)
                {
                    ICard Card = null;
                    if (ImageName[5] == '1')
                    {
                        foreach (ICard card in gameBoard.Level1Cards)
                        {
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    else if (ImageName[5] == '2')
                    {
                        foreach (ICard card in gameBoard.Level2Cards)
                        {
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    else if (ImageName[5] == '3')
                    {
                        foreach (ICard card in gameBoard.Level3Cards)
                        {
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }

                    ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(Card, true));

                    if (completedTurn.Error != null)
                    {
                        return Json(completedTurn.Error);
                    }
                    else if (completedTurn.ContinueAction != null)
                    {
                        return Json(completedTurn.ContinueAction);
                    }
                }
                return Json(gameBoard);
            }


            return Json("");
        }



        [HttpPost]
        [Route("Game/Noble/{gameId:int}/{playerId:int}")]
        public JsonResult Noble([FromBody] string ImageName, int gameId, int playerId)
        {   
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                // Check to make sure the image isn't null
                if (ImageName != null)
                {
                    INoble? Noble = null;

                    // Find the noble on the gameboard
                    foreach(INoble noble in gameBoard.Nobles)
                    {
                        if (noble.ImageName == ImageName)
                        {
                            Noble = noble;
                            break;
                        }
                    }

                    ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(Noble));

                    if (completedTurn.Error != null)
                    {
                        return Json(completedTurn.Error);
                    }
                    else if (completedTurn.ContinueAction != null)
                    {
                        return Json(completedTurn.ContinueAction);
                    }
                }
                return Json(gameBoard);
            }


            return Json("");
        }


        [HttpPost]
        [Route("Game/Return/{gameId:int}/{playerId:int}")]
        public JsonResult Return([FromBody] ReturnRequest returnRequest, int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                if (returnRequest != null)
                {
                    ICard? Card = null;
                    if (returnRequest.ReservingCardImageName != null)
                    {
                        if (returnRequest.ReservingCardImageName[5] == '1')
                        {
                            foreach (ICard card in gameBoard.Level1Cards)
                            {
                                if (card.ImageName == returnRequest.ReservingCardImageName)
                                {
                                    Card = card;
                                    break;
                                }
                            }
                        }
                        else if (returnRequest.ReservingCardImageName[5] == '2')
                        {
                            foreach (ICard card in gameBoard.Level2Cards)
                            {
                                if (card.ImageName == returnRequest.ReservingCardImageName)
                                {
                                    Card = card;
                                    break;
                                }
                            }
                        }
                        else if (returnRequest.ReservingCardImageName[5] == '3')
                        {
                            foreach (ICard card in gameBoard.Level3Cards)
                            {
                                if (card.ImageName == returnRequest.ReservingCardImageName)
                                {
                                    Card = card;
                                    break;
                                }
                            }
                        }

                    }

                    ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(returnRequest.Tokens, Card));

                    if (completedTurn.Error != null)
                    {
                        return Json(completedTurn.Error);
                    }
                    else if (completedTurn.ContinueAction != null)
                    {
                        return Json(completedTurn.ContinueAction);
                    }
                }


                return Json(gameBoard);
            }


            return Json("");
        }



        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="gameId">The Id of the game</param>
        /// <returns>Redericts to the index view</returns>
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
                foreach (KeyValuePair<int, string> kvp in gameInfo.Players)
                {
                    players.Add(new Player(kvp.Value, kvp.Key));
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
