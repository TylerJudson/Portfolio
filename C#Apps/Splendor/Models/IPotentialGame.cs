namespace Splendor.Models
{
    public interface IPotentialGame
    {
        int Id { get; }

        string CreatingPlayerName { get; }

        List<string> PlayerNames { get; }

        public int MaxPlayers { get; }

    }
}
