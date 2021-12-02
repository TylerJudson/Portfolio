namespace Splendor.Models.Implementation
{
    public class Turn : ITurn
    {
        public Dictionary<Token, int>? TakenTokens { get; }

        public ICard? Card { get; }

        public ICard? ReservedCard { get; }

        public INoble? Noble { get; }
    }
}
