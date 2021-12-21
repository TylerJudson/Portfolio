namespace Splendor.Models.Implementation
{
    public class ContinueAction : IContinueAction
    {
        public string Message { get; }
        public int ActionCode { get; }
        public List<INoble>? Nobles { get; }

        /// <summary>
        /// Initalizes ContinueAction
        /// </summary>
        /// <param name="message">The message of what action to continue</param>
        /// <param name="actionCode">
        /// The code of the action
        /// <br>0 - Get rid of some tokens</br>
        /// <br>1 - Choose a noble</br>
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
            Nobles = nobles;
        }
    }
}
