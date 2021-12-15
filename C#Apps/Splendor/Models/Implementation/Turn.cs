namespace Splendor.Models.Implementation
{
    public class Turn : ITurn
    {
        public Dictionary<Token, int>? TakenTokens { get; set; }

        public ICard? Card { get; set; }

        public ICard? ReservedCard { get; set; }

        public INoble? Noble { get; set; }

        public Turn(Dictionary<Token, int>? takenTokens)
        {
            TakenTokens = takenTokens;
        }
        public Turn(ICard card, bool isReserve=false)
        {
            if (isReserve)
            {
                ReservedCard = card;
            }
            else
            {
                Card = card;
            }
        }
        
    }
}
