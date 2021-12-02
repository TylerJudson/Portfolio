namespace Splendor.Models
{
    public interface IContinueAction
    {
        /// <summary>
        /// The message of what action to continue
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The code of the action
        /// </summary>
        int ActionCode { get; }
    }
}
