namespace Splendor.Models.Implementation
{
    public class Turn : ITurn
    {
        public Dictionary<Token, int>? TakenTokens { get; set; }

        public ICard? Card { get; set; }

        public ICard? ReservedCard { get; set; }

        public INoble? Noble { get; set; }
    }
}
