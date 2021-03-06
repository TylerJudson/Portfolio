using Microsoft.AspNetCore.Mvc;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Controllers
{
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
                if (gameBoard.IsPaused)
                {
                    return Json("IsPaused");
                }
                return Json(gameBoard.Version);
            }

            // The game has ended if the game isn't active
            return Json("The game has ended.");
        }


        /// <summary>
        /// The end point for when the player ends their turn
        /// </summary>
        /// <param name="TakenTokens">The tokens the player took during their turn</param>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The game</returns>
        [HttpPost]
        [Route("Game/EndTurn/{gameId:int}/{playerId:int}")]
        public JsonResult EndTurn([FromBody] Dictionary<Token, int> TakenTokens, int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                // Make sure taken tokens isn't null
                if (TakenTokens != null)
                {
                    // execute the turn
                    ICompletedTurn completedTurn = gameBoard.ExecuteTurn( new Turn(TakenTokens));

                    // return the error or continue action
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
        /// THe end point for when a player purchases a card
        /// </summary>
        /// <param name="ImageName">The name of the image for the card to purchase</param>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the player</param>
        /// <returns>The game</returns>
        [HttpPost]
        [Route("Game/Purchase/{gameId:int}/{playerId:int}")]
        public JsonResult Purchase([FromBody] string ImageName, int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                // Check for nullality
                if (ImageName != null)
                {
                    ICard Card = null;

                    // If the card is in the level 1 cards
                    if (ImageName[5] == '1')
                    {
                        // loop through the cards and find the card
                        foreach(ICard? card in gameBoard.Level1Cards)
                        {
                            if (card == null)
                            {
                                continue;
                            }
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    // If the card is in the level 2 cards
                    else if (ImageName[5] == '2')
                    {
                        // loop through the cards and find the card
                        foreach (ICard? card in gameBoard.Level2Cards)
                        {
                            if (card == null)
                            {
                                continue;
                            }
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    // if the card is in the level 3 cards
                    else if (ImageName[5] == '3')
                    {
                        // loop through the cards and find the card
                        foreach (ICard? card in gameBoard.Level3Cards)
                        {
                            if (card == null)
                            {
                                continue;
                            }
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    
                    // if the card is still null check the player's reserved cards
                    if (Card == null)
                    {
                        // find the index of the player by matching the ids
                        int playerIndex = 0;
                        for (int i = 0; i < gameBoard.Players.Count; i++)
                        {
                            if (gameBoard.Players[i].Id == playerId)
                            {
                                playerIndex = i;
                            }
                        }

                        // loop through the cards and find the card
                        foreach (ICard card in gameBoard.Players[playerIndex].ReservedCards)
                        {
                            if (card == null)
                            {
                                continue;
                            }
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    
                    // execute the turn
                    ICompletedTurn completedTurn = gameBoard.ExecuteTurn(new Turn(Card));

                    // return the appropriate erros and continue actions
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
        /// The endpoint for when the player reserves a card
        /// </summary>
        /// <param name="ImageName">The name of the image for the cards wanting to purchase</param>
        /// <param name="gameId">The id of the game</param>
        /// <param name="playerId">The id of the plaeyr</param>
        /// <returns>the game</returns>
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
                        foreach (ICard? card in gameBoard.Level1Cards)
                        {
                            if (card == null)
                            {
                                continue;
                            }
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    else if (ImageName[5] == '2')
                    {
                        foreach (ICard? card in gameBoard.Level2Cards)
                        {
                            if (card == null)
                            {
                                continue;
                            }
                            if (card.ImageName == ImageName)
                            {
                                Card = card;
                                break;
                            }
                        }
                    }
                    else if (ImageName[5] == '3')
                    {
                        foreach (ICard? card in gameBoard.Level3Cards)
                        {
                            if (card == null)
                            {
                                continue;
                            }
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

        [HttpPost]
        [Route("Game/Pause/{gameId:int}/{playerId:int}")]
        public JsonResult Pause(int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                gameBoard.IsPaused = true;
            }

            return Json("");
        }

        [HttpPost]
        [Route("Game/Resume/{gameId:int}/{playerId:int}")]
        public JsonResult Resume(int gameId, int playerId)
        {
            // Check to make sure the game is active
            if (ActiveGames.TryGetValue(gameId, out IGameBoard? gameBoard))
            {
                gameBoard.IsPaused = false;
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
            if (WaitingRoomController.PendingGames.TryGetValue(gameId, out IPotentialGame? gameInfo))
            {
                // Create a list of the players
                List<IPlayer> players = new List<IPlayer>();

                // Populate the list with the player names
                foreach (KeyValuePair<int, string> kvp in gameInfo.Players)
                {
                    players.Add(new Player(kvp.Value, kvp.Key));
                }

                // Shuffle the players
                Random random = new Random();
                players = players.OrderBy(p => random.Next()).ToList();

                // Create a new game
                IGameBoard newGame = new GameBoard(players);

                // Add the new game to active games
                ActiveGames.Add(gameId, newGame);

                // Remove the game from pending games
                WaitingRoomController.PendingGames.Remove(gameId);

                // Navigate to the game
                ViewData["GameId"] = gameId;
                ViewData["PlayerId"] = 0;






                List<int> gamesToRemove = new List<int>();
                foreach (KeyValuePair<int, IGameBoard> kvp in ActiveGames)
                {
                    if (kvp.Value != null && kvp.Value.LastTurn != null)
                    {
                        if (kvp.Value.LastTurn.TimeStamp < DateTime.UtcNow.Subtract(new TimeSpan(0, 30, 0)) && !kvp.Value.IsPaused)
                        {
                            gamesToRemove.Add(kvp.Key);
                        }
                    } 
                    else if (kvp.Value != null)
                    {
                        if (kvp.Value.GameStartTimeStamp < DateTime.UtcNow.Subtract(new TimeSpan(0, 30, 0)) && !kvp.Value.IsPaused)
                        {
                            gamesToRemove.Add(kvp.Key);
                        }
                    }
                }

                foreach (int gameIdToRemove in gamesToRemove)
                {
                    ActiveGames.Remove(gameIdToRemove);
                }







                return Redirect("/Game/Index?gameId=" + gameId + "&playerId=0");

            }
            return View("Error");
        }

    }
}
