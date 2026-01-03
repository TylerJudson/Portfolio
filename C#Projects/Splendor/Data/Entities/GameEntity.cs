namespace Splendor.Data.Entities
{
    /// <summary>
    /// Database entity for persisting active games
    /// </summary>
    public class GameEntity
    {
        /// <summary>
        /// The game ID (primary key)
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// Serialized JSON representation of the IGameBoard
        /// </summary>
        public string GameStateJson { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the game was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the game was last updated
        /// </summary>
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        /// Whether the game is paused
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Whether the game is over
        /// </summary>
        public bool GameOver { get; set; }

        /// <summary>
        /// Current version number of the game (for optimistic concurrency)
        /// </summary>
        public int Version { get; set; }
    }
}
