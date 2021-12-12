namespace Splendor.Models
{
    public interface IPotentialGame
    {
        int Id { get; }

        string CreatingPlayerName { get; }

        Dictionary<int, string> Players { get; }

        public int MaxPlayers { get; }

    }
}
