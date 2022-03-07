namespace Splendor.Models.Implementation
{
    public class CompletedTurn : ICompletedTurn
    {
        public IError? Error { get; }
        public IContinueAction? ContinueAction { get; }
        public Dictionary<Token, int>? ConsumedTokens { get; }
        public bool GameOver { get; } = false;

        /// <summary>
        /// Initializes CompletedTurn
        /// </summary>
        /// <param name="error">The error during a turn</param>
        /// <param name="continueAction">The aciton needed to continue</param>
        public CompletedTurn(IError error, IContinueAction? continueAction = null)
        {
            Error = error;
            ContinueAction = continueAction;
        }

        /// <summary>
        /// Initializes CompletedTurn
        /// </summary>
        /// <param name="continueAction">The action need to continue</param>
        /// <param name="error">The error during a turn</param>
        public CompletedTurn(IContinueAction continueAction, Dictionary<Token, int>? consumedTokens, IError? error = null)
        {
            ContinueAction = continueAction;
            Error = error;
        }

        /// <summary>
        /// Initializes CompletedTurn
        /// </summary>
        /// <param name="consumedTokens">The tokens consumed during a given turn</param>
        public CompletedTurn(Dictionary<Token, int> consumedTokens)
        {
            ConsumedTokens = consumedTokens;
        }

        /// <summary>
        /// Initializes CompletedTurn
        /// </summary>
        /// <param name="gameOver">Whether the game is over or not</param>
        public CompletedTurn(bool gameOver)
        {
            GameOver = gameOver;
        }

        /// <summary>
        /// Initializes CompletedTurn
        /// </summary>
        public CompletedTurn() { }
    }
}
