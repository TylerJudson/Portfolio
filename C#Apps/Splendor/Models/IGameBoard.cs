namespace Splendor.Models
{
    public interface IGameBoard
    {

        /// <summary>
        /// The Time when the game was started
        /// </summary>
        DateTime GameStartTimeStamp { get; }
        /// <summary>
        /// The version number the game is currently at
        /// </summary>
        int Version { get; }

        /// <summary>
        /// The Last Turn executed by the gameboard
        /// </summary>
        ITurn? LastTurn { get; }

        /// <summary>
        /// The Turns executed by the gameboard
        /// </summary>
        List<ITurn> Turns { get; }
        /// <summary>
        /// The stack of cards for level 1
        /// </summary>
        ICardStack CardStackLevel1 { get; }

        /// <summary>
        /// The stack of cards for level 2
        /// </summary>
        ICardStack CardStackLevel2 { get; }

        /// <summary>
        /// The stack of cards for level 3
        /// </summary>
        ICardStack CardStackLevel3 { get; }

        /// <summary>
        /// The current level 1 cards that are on the board
        /// </summary>
        ICard[] Level1Cards { get; }

        /// <summary>
        /// The current level 2 cards that are on the board
        /// </summary>
        ICard[] Level2Cards { get; }

        /// <summary>
        /// The current level 3 cards that are on the board
        /// </summary>
        ICard[] Level3Cards { get; }

        /// <summary>
        /// The players that are playing the game
        /// </summary>
        List <IPlayer> Players { get; }

        /// <summary>
        /// The number of tokens in each stack
        /// </summary>
        Dictionary<Token, int> TokenStacks { get; }

        /// <summary>
        /// The current Nobles that are on the board
        /// </summary>
        List<INoble> Nobles { get; }

        /// <summary>
        /// The index of the player who's turn it currently is
        /// </summary>
        int CurrentPlayer { get; }

        /// <summary>
        /// Executes the turn for the game board
        /// </summary>
        /// <param name="turn">The turn to execute</param>
        public ICompletedTurn ExecuteTurn(ITurn turn);

        /// <summary>
        /// Whether or not it's the last round
        /// </summary>
        public bool LastRound { get; }

        /// <summary>
        /// Whether or not the game has ended
        /// </summary>
        public bool GameOver { get; }

    }
}