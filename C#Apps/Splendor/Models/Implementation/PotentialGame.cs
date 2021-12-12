namespace Splendor.Models.Implementation
{
    public class PotentialGame : IPotentialGame
    {
        public int Id { get; }

        public string CreatingPlayerName { get; }

        public Dictionary<int, string> Players { get; }

        public int MaxPlayers { get; } = 4;

        //TODO - documentation

        public PotentialGame(int id, string creator)
        {
            Id = id;
            CreatingPlayerName = creator;
            Players = new Dictionary<int, string>();
            Players.Add(0, creator);
        }
    }
}
