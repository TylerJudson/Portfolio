namespace Splendor.Models.Implementation
{
    public class CardStack : ICardStack
    {

        public uint Level { get; }

        private List<ICard> _cards;
        public IReadOnlyList<ICard> Cards => _cards.AsReadOnly();


        /// <summary>
        /// Initializes the CardStack
        /// </summary>
        /// <param name="level">The level of the card 1, 2, or 3</param>
        /// <param name="count">The number of cards in the stack</param>
        /// <param name="cards">The list of cards in the stack</param>
        public CardStack(uint level, uint count, List<ICard> cards)
        {
            Level = level;
            _cards = cards;
        }

        public ICard? Draw()
        {
            ICard ret = null;

            if (_cards.Count == 0)
            {
                return ret;
            }

            Random random = Random.Shared;
            int randomNum = random.Next(_cards.Count);

            ret = _cards[randomNum];
            _cards.RemoveAt(randomNum);


            return ret;
        }

        public string Render()
        {
            throw new NotImplementedException();
        }
    }
}
