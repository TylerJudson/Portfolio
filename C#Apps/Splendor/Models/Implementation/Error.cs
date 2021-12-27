namespace Splendor.Models.Implementation
{
    public class Error : IError
    {
        public string Message { get; }

        public int ErrorCode { get; }

        /// <summary>
        /// An error object
        /// </summary>
        /// <param name="message">The message to the user of what went wrong</param>
        /// <param name="errorCode">
        /// The error code
        /// <br>0 - The player has taken too many tokens</br>
        /// <br>1 - The player has too many tokens</br>
        /// <br>2 - The player doesn't have enough tokens for a card</br>
        /// <br>3 - The player has too many reserved cards</br>
        /// <br>4 - The player has taken more than 3 types of tokens</br>
        /// <br>5 - The player doesn't have enough cards for a noble</br>
        /// <br>6 - The player tried to take a gold token</br>
        /// <br>7 - The player doesn't have enough tokens to return</br>
        /// <br>8 - The player tries do a turn when the game is over</br>
        /// </param>
        public Error(string message, int errorCode)
        {
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
