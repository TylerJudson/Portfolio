namespace Splendor.Data.Entities
{
    /// <summary>
    /// Database entity for persisting pending games (waiting room)
    /// </summary>
    public class PendingGameEntity
    {
        /// <summary>
        /// The pending game ID (primary key)
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// Name of the player who created the game
        /// </summary>
        public string CreatingPlayerName { get; set; } = string.Empty;

        /// <summary>
        /// Serialized JSON representation of the Players dictionary
        /// </summary>
        public string PlayersJson { get; set; } = string.Empty;

        /// <summary>
        /// Maximum number of players allowed
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Timestamp when the pending game was created
        /// </summary>
        public DateTime TimeCreated { get; set; }
    }
}
