namespace SecretHitler.Models
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
        Dictionary<int, string> Players { get; }

        /// <summary>
        /// The Maximum number of players allowed in the game
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        /// The minimum number of players allowed in the game
        /// </summary>
        public int MinPlayers { get; }

        /// <summary>
        /// The Time when the potential game was started
        /// </summary>
        DateTime TimeCreated { get; }
    }
}
