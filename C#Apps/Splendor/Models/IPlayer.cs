namespace Splendor.Models
{
    public interface IPlayer
    {
        /// <summary>
        /// The Name of the player
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Id of the player
        /// </summary>
        int Id { get; }

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
        /// <param name="canGetGold">Wether or not the player can get gold</param>
        public ICompletedTurn ExecuteTurn(ITurn turn, bool canGetGold);

        /// <summary>
        /// Checks to see if the player can acquire a noble
        /// </summary>
        /// <param name="noble">The noble to check if the player can acquire</param>
        /// <returns>Whether or not the player can acquire a noble</returns>
        public bool CanAcquireNoble(INoble noble);

        /// <summary>
        /// Checks to see if the player can purchase a card
        /// </summary>
        /// <param name="card">The card to check if the player can acquire</param>
        /// <returns>Whether or not the player can purchase the card</returns>
        public bool CanPurchaseCard(ICard card);
        /// <summary>
        /// Finds the cummulative number of tokens the player has
        /// </summary>
        /// <returns>The cummulative number of tokens the player has</returns>
        public int NumberOfTokens();
    }
}