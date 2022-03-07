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

        /// <summary>
        /// The Nobles the player can obtain
        /// </summary>
        List<INoble> Nobles { get; }
    }
}
