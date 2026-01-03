namespace Splendor.Models.Implementation
{
    public class CardStack : ICardStack
    {
        
        public uint Level { get; }
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
            Cards = cards;
        }

        public ICard? Draw()
        {
            ICard ret = null;

            if (Cards.Count == 0)
            {
                return ret;
            }

            Random random = Random.Shared;
            int randomNum = random.Next(Cards.Count);

            ret = Cards[randomNum];
            Cards.RemoveAt(randomNum);


            return ret;
        }

        public string Render()
        {
            throw new NotImplementedException();
        }
    }
}
