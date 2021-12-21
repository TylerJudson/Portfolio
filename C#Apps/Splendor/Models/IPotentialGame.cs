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
        Dictionary<int, string> Players { get; }

        /// <summary>
        /// The Maximum number of players allowed in the game
        /// </summary>
        public int MaxPlayers { get; }

    }
}
