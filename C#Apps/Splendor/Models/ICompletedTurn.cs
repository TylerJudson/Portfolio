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
    }
}
