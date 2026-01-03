namespace Splendor.Models.Implementation
{
    public class ContinueAction : IContinueAction
    {
        public string Message { get; }
        public int ActionCode { get; }

        private List<INoble>? _nobles;
        public IReadOnlyList<INoble> Nobles => _nobles?.AsReadOnly() ?? new List<INoble>().AsReadOnly();

        /// <summary>
        /// Initalizes ContinueAction
        /// </summary>
        /// <param name="message">The message of what action to continue</param>
        /// <param name="actionCode">
        /// The code of the action
        /// <br>0 - Get rid of some tokens</br>
        /// <br>1 - Choose a noble</br>
        /// <br>2 - Get rid of tokens for a return card</br>
        /// </param>
        public ContinueAction(string message, int actionCode)
        {
            Message = message;
            ActionCode = actionCode;
        }
        public ContinueAction(string message, int actionCode, List<INoble> nobles)
        {
            Message = message;
            ActionCode = actionCode;
            _nobles = nobles;
        }
    }
}
