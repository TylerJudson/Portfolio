using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Splendor.Controllers;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Repositories;
using Splendor.Services.Data;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Integration.Controllers
{
    // Disable parallel execution to avoid race conditions on repositories
    [Collection("Sequential")]
    public class WaitingRoomControllerTests : IDisposable
    {
        private readonly WaitingRoomController _controller;
        private readonly IPendingGameRepository _pendingGameRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameDataService _gameDataService;

        // Constructor to ensure clean state before each test
        public WaitingRoomControllerTests()
        {
            _pendingGameRepository = TestHelpers.CreateInMemoryPendingGameRepository();
            _gameRepository = TestHelpers.CreateInMemoryGameRepository();
            _gameDataService = TestHelpers.CreateMockGameDataService();

            var gameIdGenerator = TestHelpers.CreateMockGameIdGenerator(_pendingGameRepository);
            var playerIdAssignment = TestHelpers.CreateMockPlayerIdAssignmentService();
            var gameCleanup = TestHelpers.CreateMockGameCleanupService(_gameRepository, _pendingGameRepository);
            var logger = TestHelpers.CreateMockLogger<WaitingRoomController>();

            _controller = new WaitingRoomController(
                _pendingGameRepository,
                _gameRepository,
                gameIdGenerator,
                playerIdAssignment,
                gameCleanup,
                logger
            );

            // Set up TempData
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
        }

        // IDisposable implementation to clean up after each test
        public void Dispose()
        {
            // Cleanup is handled by in-memory repositories being recreated each test
        }

        #region NewGameInfo Action Tests

        [Fact]
        public async Task NewGameInfo_CreatesNewPendingGameWithUniqueId()
        {
            // Arrange
            string playerName = "TestPlayer";

            // Act
            var result = await _controller.NewGameInfo(playerName);

            // Assert
            var allGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            allGames.Should().HaveCount(1);
            var game = allGames.Values.First();
            game.Should().NotBeNull();
            game.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task NewGameInfo_AddsCreatorToPlayersDictionaryAtIdZero()
        {
            // Arrange
            string playerName = "TestCreator";

            // Act
            var result = await _controller.NewGameInfo(playerName);

            // Assert
            var allGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            var game = allGames.Values.First();
            game.Players.Should().ContainKey(0);
            game.Players[0].Should().Be(playerName);
        }

        [Fact]
        public async Task NewGameInfo_SetsCreatingPlayerNameCorrectly()
        {
            // Arrange
            string playerName = "GameCreator";

            // Act
            var result = await _controller.NewGameInfo(playerName);

            // Assert
            var allGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            var game = allGames.Values.First();
            game.CreatingPlayerName.Should().Be(playerName);
        }

        [Fact]
        public async Task NewGameInfo_RemovesStaleGamesOlderThan30Minutes()
        {
            // Arrange
            // Create a mock stale game (we can't set TimeCreated directly, so we'll use reflection or test differently)
            // Since TimeCreated is readonly and set in constructor, we need to create a custom implementation
            var staleGame = new TestPotentialGame(123, "OldPlayer", DateTime.UtcNow.Subtract(new TimeSpan(0, 31, 0)));
            var recentGame = new TestPotentialGame(456, "RecentPlayer", DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)));

            await _pendingGameRepository.AddPendingGameAsync(123, staleGame);
            await _pendingGameRepository.AddPendingGameAsync(456, recentGame);

            // Act
            var result = await _controller.NewGameInfo("NewPlayer");

            // Assert
            (await _pendingGameRepository.PendingGameExistsAsync(123)).Should().BeFalse();
            (await _pendingGameRepository.PendingGameExistsAsync(456)).Should().BeTrue();
            var allGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            allGames.Should().HaveCount(2); // Recent game + new game

        }

        [Fact]
        public async Task NewGameInfo_RedirectsToIndexWithGameIdAndPlayerIdZero()
        {
            // Arrange
            string playerName = "TestPlayer";

            // Act
            var result = await _controller.NewGameInfo(playerName) as RedirectResult;

            // Assert
            result.Should().NotBeNull();
            var allGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            var gameId = allGames.Keys.First();
            result!.Url.Should().Be($"/WaitingRoom/Index?gameId={gameId}&playerId=0");

        }

        #endregion

        #region EnterGame Action Tests

        [Fact]
        public async Task EnterGame_PlayerSuccessfullyJoinsGameWithAvailableSlots()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");
            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act
            var result = await _controller.EnterGame(100, "JoiningPlayer") as RedirectResult;

            // Assert
            result.Should().NotBeNull();
            var updatedGame = await _pendingGameRepository.GetPendingGameAsync(100);
            updatedGame!.Players.Should().HaveCount(2);
            updatedGame.Players.Should().ContainValue("JoiningPlayer");

        }

        [Fact]
        public async Task EnterGame_NewPlayerGetsIncrementedId()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");
            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act
            var result = await _controller.EnterGame(100, "Player1") as RedirectResult;

            // Assert
            var updatedGame = await _pendingGameRepository.GetPendingGameAsync(100);
            updatedGame!.Players.Should().ContainKey(1);
            updatedGame.Players[1].Should().Be("Player1");
            result!.Url.Should().Contain("playerId=1");

        }

        [Fact]
        public async Task EnterGame_JoiningFullGameRedirectsToListGames()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");

            // Fill the game to max capacity (4 players)
            game.AddPlayer(1, "Player1");
            game.AddPlayer(2, "Player2");
            game.AddPlayer(3, "Player3");

            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act
            var result = await _controller.EnterGame(100, "Player4") as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("ListGames");
            var updatedGame = await _pendingGameRepository.GetPendingGameAsync(100);
            updatedGame!.Players.Should().HaveCount(4); // Should not have added Player4

        }

        [Fact]
        public async Task EnterGame_JoiningNonExistentGameRedirectsToListGames()
        {
            // Arrange
            // No game setup

            // Act
            var result = await _controller.EnterGame(999, "Player") as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("ListGames");

        }

        [Fact]
        public async Task EnterGame_MultipleSequentialJoinsHandledCorrectly()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");
            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act - 3 players join sequentially
            var result1 = await _controller.EnterGame(100, "Player1");
            var result2 = await _controller.EnterGame(100, "Player2");
            var result3 = await _controller.EnterGame(100, "Player3");

            // Assert
            var updatedGame = await _pendingGameRepository.GetPendingGameAsync(100);
            // Should have creator (0) + 3 more players = 4 total (max capacity)
            updatedGame!.Players.Should().HaveCount(4);

            // All joins should be successful since we're under the limit
            result1.Should().BeOfType<RedirectResult>();
            result2.Should().BeOfType<RedirectResult>();
            result3.Should().BeOfType<RedirectResult>();

            // Verify player IDs are sequential
            updatedGame.Players.Keys.Should().Contain(new[] { 0, 1, 2, 3 });
        }

        #endregion

        #region State Action Tests

        [Fact]
        public async Task State_ReturnsIPotentialGameJsonWhenGamePending()
        {
            // Arrange
            var game = new PotentialGame(100, "TestPlayer");
            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act
            var result = await _controller.State(100, 0);

            // Assert
            result.Should().NotBeNull();
            var response = result.Value as ApiResponse<IPotentialGame>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().Be(100);
            response.Data.CreatingPlayerName.Should().Be("TestPlayer");

        }

        [Fact]
        public async Task State_ReturnsStartedWhenGameMovedToActiveGames()
        {
            // Arrange
            var players = new List<IPlayer>
            {
                new Player("Player1", 0),
                new Player("Player2", 1)
            };
            var activeGame = new GameBoard(players, _gameDataService);

            await _gameRepository.AddGameAsync(100, activeGame);

            // Act
            var result = await _controller.State(100, 0);

            // Assert
            result.Should().NotBeNull();
            var response = result.Value as ApiResponse<string>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().Be("started");

        }

        [Fact]
        public async Task State_ReturnsRemovedFromGameWhenPlayerRemoved()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");
            game.AddPlayer(1, "Player1");
            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Remove player 1
            game.RemovePlayer(1);
            await _pendingGameRepository.UpdatePendingGameAsync(100, game);

            // Act
            var result = await _controller.State(100, 1);

            // Assert
            result.Should().NotBeNull();
            var response = result.Value as ApiResponse<string>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().Be("removed from game");

        }

        [Fact]
        public async Task State_ReturnsEmptyWhenGameDoesNotExist()
        {
            // Arrange
            // No game setup

            // Act
            var result = await _controller.State(999, 0);

            // Assert
            result.Should().NotBeNull();
            var response = result.Value as ApiResponse<string>;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.ErrorMessage.Should().NotBeNullOrEmpty();

        }

        #endregion

        #region ListGames Tests

        [Fact]
        public async Task ListGames_ReturnsViewWithAllPendingGames()
        {
            // Arrange
            var game1 = new PotentialGame(100, "Player1");
            var game2 = new PotentialGame(200, "Player2");
            var game3 = new PotentialGame(300, "Player3");

            await _pendingGameRepository.AddPendingGameAsync(100, game1);
            await _pendingGameRepository.AddPendingGameAsync(200, game2);
            await _pendingGameRepository.AddPendingGameAsync(300, game3);

            // Act
            var result = await _controller.ListGames() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as List<IPotentialGame>;
            model.Should().NotBeNull();
            model!.Should().HaveCount(3);
            model.Should().Contain(g => g.Id == 100);
            model.Should().Contain(g => g.Id == 200);
            model.Should().Contain(g => g.Id == 300);

        }

        [Fact]
        public async Task ListGamesState_ReturnsOnlyGamesWithAvailableSlots()
        {
            // Arrange
            var gameWithSlots = new PotentialGame(100, "Player1");
            var fullGame = new PotentialGame(200, "Player2");

            // Fill the second game
            fullGame.AddPlayer(1, "Player3");
            fullGame.AddPlayer(2, "Player4");
            fullGame.AddPlayer(3, "Player5");

            await _pendingGameRepository.AddPendingGameAsync(100, gameWithSlots);
            await _pendingGameRepository.AddPendingGameAsync(200, fullGame);

            // Act
            var result = await _controller.ListGamesState() as JsonResult;

            // Assert
            result.Should().NotBeNull();
            var response = result!.Value as ApiResponse<List<IPotentialGame>>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Should().HaveCount(1);
            response.Data.First().Id.Should().Be(100);

        }

        #endregion

        #region RemovePlayer Tests

        [Fact]
        public async Task RemovePlayer_RemovesPlayerFromGame()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");
            game.AddPlayer(1, "Player1");
            game.AddPlayer(2, "Player2");

            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act
            var result = await _controller.RemovePlayer(100, 1) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            var updatedGame = await _pendingGameRepository.GetPendingGameAsync(100);
            updatedGame!.Players.Should().NotContainKey(1);
            updatedGame.Players.Should().HaveCount(2); // Creator + Player2

            var response = result!.Value as ApiResponse<IPotentialGame>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();

        }

        [Fact]
        public async Task RemovePlayer_OnNonExistentGameReturnsEmptyJson()
        {
            // Arrange
            // No game setup

            // Act
            var result = await _controller.RemovePlayer(999, 0);

            // Assert
            result.Should().NotBeNull();
            var response = result.Value as ApiResponse<string>;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
            response.ErrorMessage.Should().NotBeNullOrEmpty();

        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task MultipleGamesCanExistSimultaneously()
        {
            // Arrange & Act - Create games
            await _controller.NewGameInfo("Player1");
            var gamesAfterFirst = await _pendingGameRepository.GetAllPendingGamesAsync();
            var countAfterFirst = gamesAfterFirst.Count;

            await _controller.NewGameInfo("Player2");
            var gamesAfterSecond = await _pendingGameRepository.GetAllPendingGamesAsync();
            var countAfterSecond = gamesAfterSecond.Count;

            await _controller.NewGameInfo("Player3");
            var gamesAfterThird = await _pendingGameRepository.GetAllPendingGamesAsync();

            // Assert
            countAfterFirst.Should().Be(1, "first game should be added");
            countAfterSecond.Should().Be(2, "second game should be added");
            gamesAfterThird.Should().HaveCount(3, "all three games should exist");

            var gameIds = gamesAfterThird.Keys.ToList();
            gameIds.Should().OnlyHaveUniqueItems("all game IDs should be unique");
        }

        [Fact]
        public async Task GameIds_AreUnique_WhenCreatingMultipleGames()
        {
            // Arrange
            var gameIds = new List<int>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                await _controller.NewGameInfo($"Player{i}");
            }
            var allGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            gameIds = allGames.Keys.ToList();

            // Assert
            gameIds.Should().HaveCount(10);
            gameIds.Should().OnlyHaveUniqueItems();

        }

        [Fact]
        public async Task Cleanup_DoesNotAffectRecentlyCreatedGames()
        {
            // Arrange
            // Create a recent game
            var recentGame = new TestPotentialGame(100, "RecentPlayer", DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)));
            await _pendingGameRepository.AddPendingGameAsync(100, recentGame);

            // Act - Creating a new game triggers cleanup
            await _controller.NewGameInfo("NewPlayer");

            // Assert
            (await _pendingGameRepository.PendingGameExistsAsync(100)).Should().BeTrue();
            var allGames = await _pendingGameRepository.GetAllPendingGamesAsync();
            allGames.Should().HaveCount(2); // Recent game + new game

        }

        #endregion

        #region Additional Coverage Tests

        [Fact]
        public async Task Index_ReturnsViewWhenGameExists()
        {
            // Arrange
            var game = new PotentialGame(100, "TestPlayer");
            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act
            var result = await _controller.Index(100, 0) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewName.Should().Be("Index");
            result.Model.Should().Be(game);
            result.ViewData["GameId"].Should().Be(100);
            result.ViewData["PlayerId"].Should().Be(0);
            result.ViewData["IsCreator"].Should().Be("true");

        }

        [Fact]
        public async Task Index_ReturnsErrorViewWhenGameDoesNotExist()
        {
            // Arrange
            // No game setup

            // Act
            var result = await _controller.Index(999, 0) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewName.Should().Be("Error");

        }

        [Fact]
        public void NewGame_ReturnsView()
        {
            // Arrange & Act
            var result = _controller.NewGame() as ViewResult;

            // Assert
            result.Should().NotBeNull();

        }

        [Fact]
        public async Task EnterGame_SetsCorrectPlayerIdForMultipleJoins()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");
            await _pendingGameRepository.AddPendingGameAsync(100, game);

            // Act
            var result1 = await _controller.EnterGame(100, "Player1") as RedirectResult;
            var result2 = await _controller.EnterGame(100, "Player2") as RedirectResult;
            var result3 = await _controller.EnterGame(100, "Player3") as RedirectResult;

            // Assert
            var updatedGame = await _pendingGameRepository.GetPendingGameAsync(100);
            updatedGame!.Players.Should().HaveCount(4); // Creator + 3 players
            updatedGame.Players.Should().ContainKey(0);
            updatedGame.Players.Should().ContainKey(1);
            updatedGame.Players.Should().ContainKey(2);
            updatedGame.Players.Should().ContainKey(3);

            result1!.Url.Should().Contain("playerId=1");
            result2!.Url.Should().Contain("playerId=2");
            result3!.Url.Should().Contain("playerId=3");

        }

        #endregion

        #region Test Helper Class

        // Helper class to create test games with specific TimeCreated values
        private class TestPotentialGame : IPotentialGame
        {
            public int Id { get; }
            public string CreatingPlayerName { get; }

            private Dictionary<int, string> _players;
            public IReadOnlyDictionary<int, string> Players => _players;

            public int MaxPlayers { get; } = 4;
            public DateTime TimeCreated { get; }

            public TestPotentialGame(int id, string creator, DateTime timeCreated)
            {
                Id = id;
                CreatingPlayerName = creator;
                TimeCreated = timeCreated;
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

        #endregion
    }
}
