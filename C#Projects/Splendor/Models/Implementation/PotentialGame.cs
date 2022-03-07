namespace Splendor.Models.Implementation
{
    public class PotentialGame : IPotentialGame
    {
        public int Id { get; }

        public string CreatingPlayerName { get; }

        public Dictionary<int, string> Players { get; }

        public int MaxPlayers { get; } = 4;

        public DateTime TimeCreated { get; } = DateTime.UtcNow;

        /// <summary>
        /// Initializes the potential game
        /// </summary>
        /// <param name="id">The Id of the potential game</param>
        /// <param name="creator">The name of the creator</param>
        public PotentialGame(int id, string creator)
        {
            Id = id;
            CreatingPlayerName = creator;
            Players = new Dictionary<int, string>();
            Players.Add(0, creator);
        }
    }
}
