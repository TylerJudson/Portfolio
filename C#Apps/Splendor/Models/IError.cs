namespace Splendor.Models
{
    public interface IError
    {
        /// <summary>
        /// The message of the error to display to the user
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The code of the error 
        /// <para>
        /// <br>0 - The player has taken too many tokens</br>
        /// <br>1 - The player has too many tokens</br>
        /// <br>2 - The player doesn't have enough tokens for a card</br>
        /// <br>3 - The player has too many reserved cards</br>
        /// <br>4 - The player has taken more than 3 types of tokens</br>
        /// <br>5 - The player doesn't have enough cards for a noble</br>
        /// </para>
        /// </summary>
        int ErrorCode { get; }
    }
}
