namespace Splendor.Models.Implementation
{
    public class PotentialGame : IPotentialGame
    {
        public int Id { get; }

        public string CreatingPlayerName { get; }

        private Dictionary<int, string> _players;
        public IReadOnlyDictionary<int, string> Players => _players;

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
            _players = new Dictionary<int, string>();
            _players.Add(0, creator);
        }

        public void AddPlayer(int playerId, string playerName)
        {
            _players.Add(playerId, playerName);
        }

        public bool RemovePlayer(int playerId)
        {
            return _players.Remove(playerId);
        }
    }
}
