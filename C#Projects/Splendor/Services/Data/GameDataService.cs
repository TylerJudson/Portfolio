using Splendor.Models;
using Splendor.Models.Implementation;
using System.Text.Json;

namespace Splendor.Services.Data
{
    /// <summary>
    /// Service implementation for loading game data from JSON files
    /// </summary>
    public class GameDataService : IGameDataService
    {
        private readonly string _dataFilePath;
        private GameData? _cachedGameData;
        private readonly object _lockObject = new object();
        private readonly ILogger<GameDataService> _logger;

        /// <summary>
        /// Initializes a new instance of GameDataService
        /// </summary>
        /// <param name="webHostEnvironment">The web host environment for accessing wwwroot</param>
        /// <param name="logger">Logger instance</param>
        public GameDataService(IWebHostEnvironment webHostEnvironment, ILogger<GameDataService> logger)
        {
            _dataFilePath = Path.Combine(webHostEnvironment.WebRootPath, "Data", "gameData.json");
            _logger = logger;
        }

        /// <summary>
        /// Loads the game data from JSON file (with caching)
        /// </summary>
        private GameData LoadGameData()
        {
            if (_cachedGameData != null)
            {
                _logger.LogDebug("Using cached game data");
                return _cachedGameData;
            }

            lock (_lockObject)
            {
                // Double-check pattern for thread safety
                if (_cachedGameData != null)
                {
                    _logger.LogDebug("Using cached game data (double-check)");
                    return _cachedGameData;
                }

                try
                {
                    _logger.LogInformation("Loading game data from {DataFilePath}", _dataFilePath);

                    if (!File.Exists(_dataFilePath))
                    {
                        throw new FileNotFoundException($"Game data file not found at path: {_dataFilePath}");
                    }

                    string jsonContent = File.ReadAllText(_dataFilePath);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = {
                            new TokenDictionaryConverter(),
                            new System.Text.Json.Serialization.JsonStringEnumConverter()
                        }
                    };

                    _cachedGameData = JsonSerializer.Deserialize<GameData>(jsonContent, options);

                    if (_cachedGameData == null)
                    {
                        throw new InvalidOperationException("Failed to deserialize game data - result was null");
                    }

                    _logger.LogInformation("Game data loaded successfully from {DataFilePath}", _dataFilePath);
                    return _cachedGameData;
                }
                catch (JsonException ex)
                {
                    _logger.LogError("Failed to parse game data JSON file: {Message}", ex.Message);
                    throw new InvalidOperationException($"Failed to parse game data JSON file: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to load game data: {Message}", ex.Message);
                    throw new InvalidOperationException($"Failed to load game data: {ex.Message}", ex);
                }
            }
        }

        public List<ICard> LoadLevel1Cards()
        {
            var gameData = LoadGameData();
            return gameData.Cards.Level1.Select(dto => (ICard)new Card(
                dto.Level,
                dto.Type,
                dto.PrestigePoints,
                dto.Price,
                dto.ImageName
            )).ToList();
        }

        public List<ICard> LoadLevel2Cards()
        {
            var gameData = LoadGameData();
            return gameData.Cards.Level2.Select(dto => (ICard)new Card(
                dto.Level,
                dto.Type,
                dto.PrestigePoints,
                dto.Price,
                dto.ImageName
            )).ToList();
        }

        public List<ICard> LoadLevel3Cards()
        {
            var gameData = LoadGameData();
            return gameData.Cards.Level3.Select(dto => (ICard)new Card(
                dto.Level,
                dto.Type,
                dto.PrestigePoints,
                dto.Price,
                dto.ImageName
            )).ToList();
        }

        public List<INoble> LoadNobles()
        {
            var gameData = LoadGameData();
            return gameData.Nobles.Select(dto => (INoble)new Noble(
                dto.Criteria,
                dto.ImageName
            )).ToList();
        }

        public Dictionary<Token, int> GetTokenConfig(int playerCount)
        {
            var gameData = LoadGameData();

            if (!gameData.TokenConfig.ContainsKey(playerCount.ToString()))
            {
                throw new ArgumentException($"Token configuration not found for {playerCount} players. Valid values are 2, 3, or 4.");
            }

            return gameData.TokenConfig[playerCount.ToString()];
        }

        #region DTOs for JSON Deserialization

        private class GameData
        {
            public CardCollections Cards { get; set; } = new CardCollections();
            public List<NobleDto> Nobles { get; set; } = new List<NobleDto>();
            public Dictionary<string, Dictionary<Token, int>> TokenConfig { get; set; } = new Dictionary<string, Dictionary<Token, int>>();
        }

        private class CardCollections
        {
            public List<CardDto> Level1 { get; set; } = new List<CardDto>();
            public List<CardDto> Level2 { get; set; } = new List<CardDto>();
            public List<CardDto> Level3 { get; set; } = new List<CardDto>();
        }

        private class CardDto
        {
            public uint Level { get; set; }
            public Token Type { get; set; }
            public uint PrestigePoints { get; set; }
            public Dictionary<Token, int> Price { get; set; } = new Dictionary<Token, int>();
            public string ImageName { get; set; } = string.Empty;
        }

        private class NobleDto
        {
            public Dictionary<Token, int> Criteria { get; set; } = new Dictionary<Token, int>();
            public uint PrestigePoints { get; set; }
            public string ImageName { get; set; } = string.Empty;
        }

        #endregion

        #region Custom JSON Converter for Token Dictionary

        /// <summary>
        /// Custom JSON converter to handle Token enum as dictionary keys
        /// </summary>
        private class TokenDictionaryConverter : System.Text.Json.Serialization.JsonConverter<Dictionary<Token, int>>
        {
            public override Dictionary<Token, int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected StartObject token");
                }

                var dictionary = new Dictionary<Token, int>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dictionary;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException("Expected PropertyName token");
                    }

                    string? propertyName = reader.GetString();
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        throw new JsonException("Property name was null or empty");
                    }

                    if (!Enum.TryParse<Token>(propertyName, out Token tokenKey))
                    {
                        throw new JsonException($"Unable to parse '{propertyName}' as Token enum");
                    }

                    reader.Read();
                    int value = reader.GetInt32();
                    dictionary[tokenKey] = value;
                }

                throw new JsonException("Expected EndObject token");
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<Token, int> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                foreach (var kvp in value)
                {
                    writer.WriteNumber(kvp.Key.ToString(), kvp.Value);
                }
                writer.WriteEndObject();
            }
        }

        #endregion
    }
}
