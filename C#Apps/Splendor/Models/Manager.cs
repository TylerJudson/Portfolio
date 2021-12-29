namespace Splendor.Models
{
    public class Manager
    {
        public Dictionary<int, IGameBoard> ActiveGames { get; set; } = new Dictionary<int, IGameBoard>();
        public Dictionary<int, IPotentialGame> PendingGames { get; } = new Dictionary<int, IPotentialGame>();


        public Manager(Dictionary<int, IGameBoard> activeGames, Dictionary<int, IPotentialGame> pendingGames)
        {
            ActiveGames = activeGames;
            PendingGames = pendingGames;
        }
    }
}
