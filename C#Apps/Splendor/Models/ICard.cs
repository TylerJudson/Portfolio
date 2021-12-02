namespace Splendor.Models
{
    public interface ICard : IGameObject
    {
        /// <summary>
        /// The name of the image of the card
        /// </summary>
        string ImageName { get; }
        /// <summary>
        /// The level of the card 1, 2, or 3
        /// </summary>
        uint Level { get; }
        /// <summary>
        /// The type of the card EX: Diamond, Emerald, Ruby
        /// </summary>
        Token Type { get; }

        /// <summary>
        /// The number of prestige points the card contains
        /// </summary>
        uint PrestigePoints { get; }

        /// <summary>
        /// The price it takes to purchase a card
        /// </summary>
        Dictionary<Token, int> Price { get; }

    }
}
