namespace Splendor.Models
{
    /// <summary>
    /// Contains constant values used throughout the Splendor game
    /// </summary>
    public static class GameConstants
    {
        /// <summary>
        /// The number of prestige points required to trigger the final round
        /// </summary>
        public const int WinningPrestigePoints = 15;

        /// <summary>
        /// The maximum number of cards a player can reserve at one time
        /// </summary>
        public const int MaxReservedCards = 3;

        /// <summary>
        /// The maximum number of tokens a player can hold
        /// </summary>
        public const int MaxTokensPerPlayer = 10;

        /// <summary>
        /// The maximum number of different token types a player can take in one turn (when taking 1 of each)
        /// </summary>
        public const int MaxTokenTypes = 3;

        /// <summary>
        /// The minimum number of tokens that must remain in a stack when taking 2 of the same type
        /// </summary>
        public const int MinTokensInPoolForDouble = 2;

        /// <summary>
        /// The minimum number of tokens required in a stack to allow taking 2 of that type (must leave 2 behind)
        /// </summary>
        public const int MinTokensRequiredForDoubleAction = 4;
    }
}
