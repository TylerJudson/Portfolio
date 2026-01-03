namespace Splendor.Models
{
    public interface IPotentialGame
    {
        /// <summary>
        /// The Id of the potential game
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the player creating the game
        /// </summary>
        string CreatingPlayerName { get; }

        /// <summary>
        /// The players in the game with their Id's
        /// </summary>
        IReadOnlyDictionary<int, string> Players { get; }

        /// <summary>
        /// The Maximum number of players allowed in the game
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        /// The Time when the potential game was started
        /// </summary>
        DateTime TimeCreated { get; }

        /// <summary>
        /// Adds a player to the game
        /// </summary>
        /// <param name="playerId">The Id of the player to add</param>
        /// <param name="playerName">The name of the player to add</param>
        void AddPlayer(int playerId, string playerName);

        /// <summary>
        /// Removes a player from the game
        /// </summary>
        /// <param name="playerId">The Id of the player to remove</param>
        /// <returns>True if the player was removed, false if the player was not found</returns>
        bool RemovePlayer(int playerId);

    }
}
