namespace Splendor.Models
{
    public interface ICardStack : IGameObject
    {
        /// <summary>
        /// The Level of the card 1, 2, or 3
        /// </summary>
        uint Level { get; }

        /// <summary>
        /// The number of cards in the stack
        /// </summary>
        uint Count { get; }

        /// <summary>
        /// The list of cards in the stack
        /// </summary>
        List<ICard> Cards { get; }
    
        /// <summary>
        /// Randomly returns a card in the stack
        /// </summary>
        /// <returns>a randomly selected card</returns>
        ICard Draw();
    }
}
