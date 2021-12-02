namespace Splendor.Models
{
    public interface IPlayer
    {
        /// <summary>
        /// The Name of the player
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The amount of each token the player has
        /// </summary>
        Dictionary<Token, int> Tokens { get; }

        /// <summary>
        /// The list of cards a player has
        /// </summary>
        List<ICard> Cards { get; }

        /// <summary>
        /// The number of each card type/token the player has 
        /// </summary>
        Dictionary<Token, int> CardTokens { get; }

        /// <summary>
        /// The list of nobles a player has
        /// </summary>
        List<INoble> Nobles { get; }

        /// <summary>
        /// The list of cards a player has on reserve
        /// </summary>
        List<ICard> ReservedCards { get; }

        /// <summary>
        /// The number of prestie points a player has
        /// </summary>
        uint PrestigePoints { get; }

        /// <summary>
        /// Executes the turn for the player
        /// </summary>
        /// <param name="turn">The turn to execute</param>
        public ICompletedTurn ExecuteTurn(ITurn turn);

        /// <summary>
        /// Checks to see if the player can acquire a noble
        /// </summary>
        /// <param name="noble">The noble to check if the player can acquire</param>
        /// <returns>Wether or not the player can acquire a noble</returns>
        public bool CanAcquireNoble(INoble noble);

        /// <summary>
        /// Finds the cummulative number of tokens the player has
        /// </summary>
        /// <returns>The cummulative number of tokens the player has</returns>
        public int NumberOfTokens();
        void Test();
    }
}