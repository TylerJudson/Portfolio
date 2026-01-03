namespace Splendor.Models.Implementation
{
    public class Turn : ITurn
    {
        private Dictionary<Token, int>? _takenTokens;
        public IReadOnlyDictionary<Token, int>? TakenTokens => _takenTokens;

        public ICard? Card { get; set; }

        public ICard? ReservedCard { get; set; }

        public INoble? Noble { get; set; }

        public IContinueAction? ContinueAction { get; set; }

        public string? PlayerName { get; set; }

        public DateTime TimeStamp { get; } = DateTime.UtcNow;

        public Turn(Dictionary<Token, int>? takenTokens, ICard? reservedCard=null)
        {
            _takenTokens = takenTokens;
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
