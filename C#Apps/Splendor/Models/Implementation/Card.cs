namespace Splendor.Models.Implementation
{
    public class Card : ICard
    {

        /// <summary>
        /// The name of the image of the card
        /// </summary>
        public string ImageName { get; }
        public uint Level { get; }
        public Token Type { get; }
        public uint PrestigePoints { get; }

        public Dictionary<Token, int> Price { get; }


        /// <summary>
        /// Initializes the card
        /// </summary>
        /// <param name="level">The level of the card 1, 2, or 3</param>
        /// <param name="type">The token type of the card</param>
        /// <param name="prestigePoints">The amount of prestige points awarded by the card</param>
        /// <param name="price">The price of the card</param>
        /// <param name="imageName">The name of the image of the card</param>
        public Card(uint level, Token type, uint prestigePoints, Dictionary<Token, int> price, string imageName)
        {
            Level = level;
            Type = type;
            PrestigePoints = prestigePoints;
            Price = price;
            ImageName = imageName;
            
        }
        public string Render()
        {
            throw new NotImplementedException();
        }
    }
}
