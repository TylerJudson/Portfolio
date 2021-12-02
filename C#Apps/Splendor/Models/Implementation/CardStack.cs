namespace Splendor.Models.Implementation
{
    public class CardStack : ICardStack
    {
        
        public uint Level { get; }
        public uint Count { get; private set; }
        public List<ICard> Cards { get; }


        /// <summary>
        /// Initializes the CardStack
        /// </summary>
        /// <param name="level">The level of the card 1, 2, or 3</param>
        /// <param name="count">The number of cards in the stack</param>
        /// <param name="cards">The list of cards in the stack</param>
        public CardStack(uint level, uint count, List<ICard> cards)
        {
            Level = level;
            Count = count;
            Cards = cards;
        }

        public ICard Draw()
        {
            Random random = new Random();
            int randomNum = random.Next(Cards.Count);

            ICard ret = Cards[randomNum];
            Cards.RemoveAt(randomNum);
            Count--;

            return ret;
        }

        public string Render()
        {
            throw new NotImplementedException();
        }
    }
}
