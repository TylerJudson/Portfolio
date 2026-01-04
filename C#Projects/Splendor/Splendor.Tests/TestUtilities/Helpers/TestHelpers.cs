using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Repositories;
using Splendor.Services.Data;
using Splendor.Services.Game;
using Splendor.Services.Lookup;
using System.Reflection;

namespace Splendor.Tests.TestUtilities.Helpers
{
    /// <summary>
    /// Helper class for creating mock objects and test infrastructure
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Creates a mock IGameDataService with pre-loaded test data
        /// </summary>
        public static IGameDataService CreateMockGameDataService()
        {
            return new MockGameDataService();
        }

        /// <summary>
        /// Creates an in-memory IGameRepository backed by a Dictionary
        /// </summary>
        public static IGameRepository CreateInMemoryGameRepository()
        {
            return new InMemoryGameRepository();
        }

        /// <summary>
        /// Creates an in-memory IPendingGameRepository backed by a Dictionary
        /// </summary>
        public static IPendingGameRepository CreateInMemoryPendingGameRepository()
        {
            return new InMemoryPendingGameRepository();
        }

        /// <summary>
        /// Creates a mock ICardLookupService
        /// </summary>
        public static ICardLookupService CreateMockCardLookupService()
        {
            return new CardLookupService(CreateMockLogger<CardLookupService>());
        }

        /// <summary>
        /// Creates a mock INobleLookupService
        /// </summary>
        public static INobleLookupService CreateMockNobleLookupService()
        {
            return new NobleLookupService(CreateMockLogger<NobleLookupService>());
        }

        /// <summary>
        /// Creates a mock IPlayerLookupService
        /// </summary>
        public static IPlayerLookupService CreateMockPlayerLookupService()
        {
            return new PlayerLookupService();
        }

        /// <summary>
        /// Creates a mock IGameActivationService
        /// </summary>
        public static IGameActivationService CreateMockGameActivationService(
            IPendingGameRepository pendingGameRepo,
            IGameRepository gameRepo,
            IGameDataService gameDataService)
        {
            return new GameActivationService(pendingGameRepo, gameRepo, gameDataService, CreateMockLogger<GameActivationService>());
        }

        /// <summary>
        /// Creates a mock IGameCleanupService
        /// </summary>
        public static IGameCleanupService CreateMockGameCleanupService(
            IGameRepository gameRepo,
            IPendingGameRepository pendingGameRepo)
        {
            return new GameCleanupService(gameRepo, pendingGameRepo, CreateMockLogger<GameCleanupService>(), 30);
        }

        /// <summary>
        /// Creates a mock IGameIdGenerator
        /// </summary>
        public static IGameIdGenerator CreateMockGameIdGenerator(IPendingGameRepository pendingGameRepo)
        {
            return new MockGameIdGenerator(pendingGameRepo);
        }

        /// <summary>
        /// Creates a mock IPlayerIdAssignmentService
        /// </summary>
        public static IPlayerIdAssignmentService CreateMockPlayerIdAssignmentService()
        {
            return new PlayerIdAssignmentService(CreateMockLogger<PlayerIdAssignmentService>());
        }

        /// <summary>
        /// Creates a mock ILogger for testing
        /// </summary>
        public static ILogger<T> CreateMockLogger<T>()
        {
            return Mock.Of<ILogger<T>>();
        }

        #region Mock Implementations

        /// <summary>
        /// Mock implementation of IGameDataService for testing that uses actual game data
        /// </summary>
        private class MockGameDataService : IGameDataService
        {
            private readonly GameDataService _realService;

            public MockGameDataService()
            {
                // Create a mock web host environment that points to the test project's wwwroot
                var mockEnv = new Mock<IWebHostEnvironment>();
                // Get the test project directory (bin/Debug/net10.0) and navigate to project root
                var testDirectory = Directory.GetCurrentDirectory();
                // Navigate up 3 levels from bin/Debug/net10.0 to test project root
                var testProjectRoot = Path.GetFullPath(Path.Combine(testDirectory, "..", "..", ".."));
                var wwwrootPath = Path.Combine(testProjectRoot, "wwwroot");

                mockEnv.Setup(m => m.WebRootPath).Returns(wwwrootPath);

                _realService = new GameDataService(mockEnv.Object, Mock.Of<ILogger<GameDataService>>());
            }

            public List<ICard> LoadLevel1Cards() => _realService.LoadLevel1Cards();
            public List<ICard> LoadLevel2Cards() => _realService.LoadLevel2Cards();
            public List<ICard> LoadLevel3Cards() => _realService.LoadLevel3Cards();
            public List<INoble> LoadNobles() => _realService.LoadNobles();
            public Dictionary<Token, int> GetTokenConfig(int playerCount) => _realService.GetTokenConfig(playerCount);
        }

        /// <summary>
        /// Simple in-memory implementation of IGameRepository for testing
        /// </summary>
        private class InMemoryGameRepository : IGameRepository
        {
            private readonly Dictionary<int, IGameBoard> _games = new();

            public Task<IGameBoard?> GetGameAsync(int gameId)
            {
                _games.TryGetValue(gameId, out var game);
                return Task.FromResult(game);
            }

            public Task<Dictionary<int, IGameBoard>> GetAllGamesAsync()
            {
                return Task.FromResult(new Dictionary<int, IGameBoard>(_games));
            }

            public Task AddGameAsync(int gameId, IGameBoard gameBoard)
            {
                _games[gameId] = gameBoard;
                return Task.CompletedTask;
            }

            public Task UpdateGameAsync(int gameId, IGameBoard gameBoard)
            {
                _games[gameId] = gameBoard;
                return Task.CompletedTask;
            }

            public Task RemoveGameAsync(int gameId)
            {
                _games.Remove(gameId);
                return Task.CompletedTask;
            }

            public Task RemoveStaleGamesAsync(TimeSpan maxAge)
            {
                var cutoffTime = DateTime.UtcNow.Subtract(maxAge);
                var staleGames = _games
                    .Where(kvp => !kvp.Value.IsPaused && kvp.Value.GameStartTimeStamp < cutoffTime)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var gameId in staleGames)
                {
                    _games.Remove(gameId);
                }

                return Task.CompletedTask;
            }

            public Task<bool> GameExistsAsync(int gameId)
            {
                return Task.FromResult(_games.ContainsKey(gameId));
            }
        }

        /// <summary>
        /// Simple in-memory implementation of IPendingGameRepository for testing
        /// </summary>
        private class InMemoryPendingGameRepository : IPendingGameRepository
        {
            private readonly Dictionary<int, IPotentialGame> _pendingGames = new();

            public Task<IPotentialGame?> GetPendingGameAsync(int gameId)
            {
                _pendingGames.TryGetValue(gameId, out var game);
                return Task.FromResult(game);
            }

            public Task<Dictionary<int, IPotentialGame>> GetAllPendingGamesAsync()
            {
                return Task.FromResult(new Dictionary<int, IPotentialGame>(_pendingGames));
            }

            public Task AddPendingGameAsync(int gameId, IPotentialGame potentialGame)
            {
                _pendingGames[gameId] = potentialGame;
                return Task.CompletedTask;
            }

            public Task UpdatePendingGameAsync(int gameId, IPotentialGame potentialGame)
            {
                _pendingGames[gameId] = potentialGame;
                return Task.CompletedTask;
            }

            public Task RemovePendingGameAsync(int gameId)
            {
                _pendingGames.Remove(gameId);
                return Task.CompletedTask;
            }

            public Task RemoveStalePendingGamesAsync(TimeSpan maxAge)
            {
                var cutoffTime = DateTime.UtcNow.Subtract(maxAge);
                var staleGames = _pendingGames
                    .Where(kvp => kvp.Value.TimeCreated < cutoffTime)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var gameId in staleGames)
                {
                    _pendingGames.Remove(gameId);
                }

                return Task.CompletedTask;
            }

            public Task<bool> PendingGameExistsAsync(int gameId)
            {
                return Task.FromResult(_pendingGames.ContainsKey(gameId));
            }
        }

        /// <summary>
        /// Mock game ID generator for testing
        /// </summary>
        private class MockGameIdGenerator : IGameIdGenerator
        {
            private readonly IPendingGameRepository _pendingGameRepo;
            private static int _nextId = 10000;

            public MockGameIdGenerator(IPendingGameRepository pendingGameRepo)
            {
                _pendingGameRepo = pendingGameRepo;
            }

            public async Task<int> GenerateUniqueIdAsync()
            {
                int id;
                do
                {
                    id = _nextId++;
                } while (await _pendingGameRepo.PendingGameExistsAsync(id));

                return id;
            }
        }

        #endregion

        #region Reflection Helpers for Testing

        /// <summary>
        /// Sets player tokens using reflection to bypass readonly restrictions
        /// </summary>
        public static void SetPlayerTokens(IPlayer player, Token token, int amount)
        {
            var playerImpl = player as Player;
            if (playerImpl == null) return;

            var tokensField = typeof(Player).GetField("_tokens", BindingFlags.NonPublic | BindingFlags.Instance);
            var tokensDict = tokensField?.GetValue(playerImpl) as Dictionary<Token, int>;
            if (tokensDict != null)
                tokensDict[token] = amount;
        }

        /// <summary>
        /// Adds a card to player's collection using reflection
        /// </summary>
        public static void AddPlayerCard(IPlayer player, ICard card)
        {
            var playerImpl = player as Player;
            if (playerImpl == null) return;

            var cardsField = typeof(Player).GetField("_cards", BindingFlags.NonPublic | BindingFlags.Instance);
            var cardsList = cardsField?.GetValue(playerImpl) as List<ICard>;
            cardsList?.Add(card);

            // Also update prestige points and card tokens
            var prestigeProperty = typeof(Player).GetProperty("PrestigePoints");
            var currentPrestige = (uint)(prestigeProperty?.GetValue(playerImpl) ?? 0u);
            prestigeProperty?.SetValue(playerImpl, currentPrestige + card.PrestigePoints);

            // Update card tokens (bonuses)
            var cardTokensField = typeof(Player).GetField("_cardTokens", BindingFlags.NonPublic | BindingFlags.Instance);
            var cardTokensDict = cardTokensField?.GetValue(playerImpl) as Dictionary<Token, int>;
            if (cardTokensDict != null && cardTokensDict.ContainsKey(card.Type))
            {
                cardTokensDict[card.Type]++;
            }
        }

        /// <summary>
        /// Adds a noble to player's collection using reflection
        /// </summary>
        public static void AddPlayerNoble(IPlayer player, INoble noble)
        {
            var playerImpl = player as Player;
            if (playerImpl == null) return;

            var noblesField = typeof(Player).GetField("_nobles", BindingFlags.NonPublic | BindingFlags.Instance);
            var noblesList = noblesField?.GetValue(playerImpl) as List<INoble>;
            noblesList?.Add(noble);

            // Also update prestige points
            var prestigeProperty = typeof(Player).GetProperty("PrestigePoints");
            var currentPrestige = (uint)(prestigeProperty?.GetValue(playerImpl) ?? 0u);
            prestigeProperty?.SetValue(playerImpl, currentPrestige + noble.PrestigePoints);
        }

        /// <summary>
        /// Sets a card on the game board at a specific level and index using reflection
        /// </summary>
        public static void SetBoardCard(IGameBoard board, int level, int index, ICard? card)
        {
            var boardImpl = board as GameBoard;
            if (boardImpl == null) return;

            string fieldName = level switch
            {
                1 => "_level1Cards",
                2 => "_level2Cards",
                3 => "_level3Cards",
                _ => null
            };

            if (fieldName == null) return;

            var cardsField = typeof(GameBoard).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            var cardsArray = cardsField?.GetValue(boardImpl) as ICard?[];
            if (cardsArray != null && index >= 0 && index < cardsArray.Length)
                cardsArray[index] = card;
        }

        /// <summary>
        /// Modifies the nobles list on the game board using reflection
        /// </summary>
        public static void ClearBoardNobles(IGameBoard board)
        {
            var boardImpl = board as GameBoard;
            if (boardImpl == null) return;

            var noblesField = typeof(GameBoard).GetField("_nobles", BindingFlags.NonPublic | BindingFlags.Instance);
            var noblesList = noblesField?.GetValue(boardImpl) as List<INoble>;
            noblesList?.Clear();
        }

        /// <summary>
        /// Adds a noble to the game board using reflection
        /// </summary>
        public static void AddBoardNoble(IGameBoard board, INoble noble)
        {
            var boardImpl = board as GameBoard;
            if (boardImpl == null) return;

            var noblesField = typeof(GameBoard).GetField("_nobles", BindingFlags.NonPublic | BindingFlags.Instance);
            var noblesList = noblesField?.GetValue(boardImpl) as List<INoble>;
            noblesList?.Add(noble);
        }

        /// <summary>
        /// Sets token stack amounts on the game board using reflection
        /// </summary>
        public static void SetBoardTokenStack(IGameBoard board, Token token, int amount)
        {
            var boardImpl = board as GameBoard;
            if (boardImpl == null) return;

            var tokenStacksField = typeof(GameBoard).GetField("_tokenStacks", BindingFlags.NonPublic | BindingFlags.Instance);
            var tokenStacksDict = tokenStacksField?.GetValue(boardImpl) as Dictionary<Token, int>;
            if (tokenStacksDict != null)
                tokenStacksDict[token] = amount;
        }

        #endregion
    }
}
