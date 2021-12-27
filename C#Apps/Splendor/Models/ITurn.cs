namespace Splendor.Models
{
    public interface ITurn
    {
        /// <summary>
        /// The tokens taken during a given turn. Has to be 1 of at most 3 different tokens or 2 of the same token
        /// </summary>
        Dictionary<Token, int>? TakenTokens { get; }

        /// <summary>
        /// The card purchased during a given turn
        /// </summary>
        ICard? Card { get; }
        
        /// <summary>
        /// The card reserved during a given turn
        /// </summary>
        ICard? ReservedCard { get; }

        /// <summary>
        /// The noble achevied during a given turn
        /// </summary>
        INoble? Noble { get; }

        /// <summary>
        /// The action needed to continue
        /// </summary>
        IContinueAction? ContinueAction { get; set; }

        /// <summary>
        /// The name of the player who is doing the turn
        /// </summary>
        string? PlayerName { get; set; }
    }
}
