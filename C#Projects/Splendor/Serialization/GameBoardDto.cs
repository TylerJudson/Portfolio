using Splendor.Models;

namespace Splendor.Serialization
{
    /// <summary>
    /// Data Transfer Object for serializing IGameBoard
    /// Maps interfaces to concrete types for JSON serialization
    /// </summary>
    public class GameBoardDto
    {
        public DateTime GameStartTimeStamp { get; set; }
        public int Version { get; set; }
        public TurnDto? LastTurn { get; set; }
        public List<TurnDto> Turns { get; set; } = new List<TurnDto>();
        public CardStackDto CardStackLevel1 { get; set; } = null!;
        public CardStackDto CardStackLevel2 { get; set; } = null!;
        public CardStackDto CardStackLevel3 { get; set; } = null!;
        public CardDto?[] Level1Cards { get; set; } = new CardDto?[4];
        public CardDto?[] Level2Cards { get; set; } = new CardDto?[4];
        public CardDto?[] Level3Cards { get; set; } = new CardDto?[4];
        public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();
        public Dictionary<Token, int> TokenStacks { get; set; } = new Dictionary<Token, int>();
        public List<NobleDto> Nobles { get; set; } = new List<NobleDto>();
        public int CurrentPlayer { get; set; }
        public bool LastRound { get; set; }
        public bool GameOver { get; set; }
        public bool IsPaused { get; set; }
    }

    public class PlayerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<Token, int> Tokens { get; set; } = new Dictionary<Token, int>();
        public List<CardDto> Cards { get; set; } = new List<CardDto>();
        public Dictionary<Token, int> CardTokens { get; set; } = new Dictionary<Token, int>();
        public List<NobleDto> Nobles { get; set; } = new List<NobleDto>();
        public List<CardDto> ReservedCards { get; set; } = new List<CardDto>();
        public uint PrestigePoints { get; set; }
    }

    public class CardDto
    {
        public string ImageName { get; set; } = string.Empty;
        public uint Level { get; set; }
        public Token Type { get; set; }
        public uint PrestigePoints { get; set; }
        public Dictionary<Token, int> Price { get; set; } = new Dictionary<Token, int>();
    }

    public class CardStackDto
    {
        public uint Level { get; set; }
        public List<CardDto> Cards { get; set; } = new List<CardDto>();
    }

    public class NobleDto
    {
        public string ImageName { get; set; } = string.Empty;
        public Dictionary<Token, int> Criteria { get; set; } = new Dictionary<Token, int>();
        public uint PrestigePoints { get; set; }
    }

    public class TurnDto
    {
        public Dictionary<Token, int>? TakenTokens { get; set; }
        public CardDto? Card { get; set; }
        public CardDto? ReservedCard { get; set; }
        public uint ReserveDeckLevel { get; set; }
        public NobleDto? Noble { get; set; }
        public ContinueActionDto? ContinueAction { get; set; }
        public string? PlayerName { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class ContinueActionDto
    {
        public string Message { get; set; } = string.Empty;
        public int ActionCode { get; set; }
        public List<NobleDto>? Nobles { get; set; }
    }
}
