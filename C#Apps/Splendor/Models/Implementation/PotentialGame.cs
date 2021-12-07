namespace Splendor.Models.Implementation
{
    public class PotentialGame : IPotentialGame
    {
        public int Id { get; }

        public string CreatingPlayerName { get; }

        public List<string> PlayerNames { get; }

        public int MaxPlayers { get; } = 4;

        //TODO - documentation

        public PotentialGame(int id, string creator)
        {
            Id = id;
            CreatingPlayerName = creator;
            PlayerNames = new List<string>();
            PlayerNames.Add(creator);
        }
    }
}
