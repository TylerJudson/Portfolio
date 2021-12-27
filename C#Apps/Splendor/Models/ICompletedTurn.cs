namespace Splendor.Models
{
    public interface ICompletedTurn
    {
        /// <summary>
        /// The error during a turn
        /// </summary>
        IError? Error { get; }

        /// <summary>
        /// The action needed to continue
        /// </summary>
        IContinueAction? ContinueAction { get; }

        /// <summary>
        /// The tokens consumed during a given turn
        /// </summary>
        Dictionary<Token, int>? ConsumedTokens { get; }

        /// <summary>
        /// Whether or not the game is over
        /// </summary>
        bool GameOver { get; }
    }
}
