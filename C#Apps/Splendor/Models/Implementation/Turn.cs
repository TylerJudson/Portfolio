namespace Splendor.Models.Implementation
{
    public class Turn : ITurn
    {
        public Dictionary<Token, int>? TakenTokens { get; set; }

        public ICard? Card { get; set; }

        public ICard? ReservedCard { get; set; }

        public INoble? Noble { get; set; }

        public IContinueAction? ContinueAction { get; set; }

        public string? PlayerName { get; set; }

        public DateTime TimeStamp { get; } = DateTime.UtcNow;

        public Turn(Dictionary<Token, int>? takenTokens, ICard? reservedCard=null)
        {
            TakenTokens = takenTokens;
            ReservedCard = reservedCard;
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
        public Turn(INoble noble)
        {
            Noble = noble;
        }

    }
}
