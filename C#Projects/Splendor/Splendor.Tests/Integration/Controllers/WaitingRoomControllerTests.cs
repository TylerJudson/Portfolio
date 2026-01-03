using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Splendor.Controllers;
using Splendor.Models;
using Splendor.Models.Implementation;
using Xunit;

namespace Splendor.Tests.Integration.Controllers
{
    // Disable parallel execution to avoid race conditions on static dictionaries
    [Collection("Sequential")]
    public class WaitingRoomControllerTests : IDisposable
    {
        // Constructor to ensure clean state before each test
        public WaitingRoomControllerTests()
        {
            WaitingRoomController.PendingGames.Clear();
            GameController.ActiveGames.Clear();
        }

        // IDisposable implementation to clean up after each test
        public void Dispose()
        {
            WaitingRoomController.PendingGames.Clear();
            GameController.ActiveGames.Clear();
        }

        // Helper method to create a controller with TempData configured
        private WaitingRoomController CreateControllerWithTempData()
        {
            var controller = new WaitingRoomController();
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
            return controller;
        }

        #region NewGameInfo Action Tests

        [Fact]
        public void NewGameInfo_CreatesNewPendingGameWithUniqueId()
        {
            // Arrange
            var controller = new WaitingRoomController();
            string playerName = "TestPlayer";

            // Act
            var result = controller.NewGameInfo(playerName);

            // Assert
            WaitingRoomController.PendingGames.Should().HaveCount(1);
            var game = WaitingRoomController.PendingGames.Values.First();
            game.Should().NotBeNull();
            game.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public void NewGameInfo_AddsCreatorToPlayersDictionaryAtIdZero()
        {
            // Arrange
            var controller = new WaitingRoomController();
            string playerName = "TestCreator";

            // Act
            var result = controller.NewGameInfo(playerName);

            // Assert
            var game = WaitingRoomController.PendingGames.Values.First();
            game.Players.Should().ContainKey(0);
            game.Players[0].Should().Be(playerName);
        }

        [Fact]
        public void NewGameInfo_SetsCreatingPlayerNameCorrectly()
        {
            // Arrange
            var controller = new WaitingRoomController();
            string playerName = "GameCreator";

            // Act
            var result = controller.NewGameInfo(playerName);

            // Assert
            var game = WaitingRoomController.PendingGames.Values.First();
            game.CreatingPlayerName.Should().Be(playerName);
        }

        [Fact]
        public void NewGameInfo_RemovesStaleGamesOlderThan30Minutes()
        {
            // Arrange
            var controller = new WaitingRoomController();

            // Create a mock stale game (we can't set TimeCreated directly, so we'll use reflection or test differently)
            // Since TimeCreated is readonly and set in constructor, we need to create a custom implementation
            var staleGame = new TestPotentialGame(123, "OldPlayer", DateTime.UtcNow.Subtract(new TimeSpan(0, 31, 0)));
            var recentGame = new TestPotentialGame(456, "RecentPlayer", DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)));

            WaitingRoomController.PendingGames.Add(123, staleGame);
            WaitingRoomController.PendingGames.Add(456, recentGame);

            // Act
            var result = controller.NewGameInfo("NewPlayer");

            // Assert
            WaitingRoomController.PendingGames.Should().NotContainKey(123);
            WaitingRoomController.PendingGames.Should().ContainKey(456);
            WaitingRoomController.PendingGames.Should().HaveCount(2); // Recent game + new game

        }

        [Fact]
        public void NewGameInfo_RedirectsToIndexWithGameIdAndPlayerIdZero()
        {
            // Arrange
            var controller = new WaitingRoomController();
            string playerName = "TestPlayer";

            // Act
            var result = controller.NewGameInfo(playerName) as RedirectResult;

            // Assert
            result.Should().NotBeNull();
            var gameId = WaitingRoomController.PendingGames.Keys.First();
            result!.Url.Should().Be($"/WaitingRoom/Index?gameId={gameId}&playerId=0");

        }

        #endregion

        #region EnterGame Action Tests

        [Fact]
        public void EnterGame_PlayerSuccessfullyJoinsGameWithAvailableSlots()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game = new PotentialGame(100, "Creator");
            WaitingRoomController.PendingGames.Add(100, game);

            // Act
            var result = controller.EnterGame(100, "JoiningPlayer") as RedirectResult;

            // Assert
            result.Should().NotBeNull();
            game.Players.Should().HaveCount(2);
            game.Players.Should().ContainValue("JoiningPlayer");

        }

        [Fact]
        public void EnterGame_NewPlayerGetsIncrementedId()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game = new PotentialGame(100, "Creator");
            WaitingRoomController.PendingGames.Add(100, game);

            // Act
            var result = controller.EnterGame(100, "Player1") as RedirectResult;

            // Assert
            game.Players.Should().ContainKey(1);
            game.Players[1].Should().Be("Player1");
            result!.Url.Should().Contain("playerId=1");

        }

        [Fact]
        public void EnterGame_JoiningFullGameRedirectsToListGames()
        {
            // Arrange
            var controller = CreateControllerWithTempData();
            var game = new PotentialGame(100, "Creator");

            // Fill the game to max capacity (4 players)
            game.Players.Add(1, "Player1");
            game.Players.Add(2, "Player2");
            game.Players.Add(3, "Player3");

            WaitingRoomController.PendingGames.Add(100, game);

            // Act
            var result = controller.EnterGame(100, "Player4") as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("ListGames");
            game.Players.Should().HaveCount(4); // Should not have added Player4

        }

        [Fact]
        public void EnterGame_JoiningNonExistentGameRedirectsToListGames()
        {
            // Arrange
            var controller = CreateControllerWithTempData();

            // Act
            var result = controller.EnterGame(999, "Player") as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("ListGames");

        }

        [Fact]
        public void EnterGame_MultipleSequentialJoinsHandledCorrectly()
        {
            // Arrange
            var game = new PotentialGame(100, "Creator");
            WaitingRoomController.PendingGames.Add(100, game);
            var controller = CreateControllerWithTempData();

            // Act - 3 players join sequentially
            var result1 = controller.EnterGame(100, "Player1");
            var result2 = controller.EnterGame(100, "Player2");
            var result3 = controller.EnterGame(100, "Player3");

            // Assert
            // Should have creator (0) + 3 more players = 4 total (max capacity)
            game.Players.Should().HaveCount(4);

            // All joins should be successful since we're under the limit
            result1.Should().BeOfType<RedirectResult>();
            result2.Should().BeOfType<RedirectResult>();
            result3.Should().BeOfType<RedirectResult>();

            // Verify player IDs are sequential
            game.Players.Keys.Should().Contain(new[] { 0, 1, 2, 3 });
        }

        #endregion

        #region State Action Tests

        [Fact]
        public void State_ReturnsIPotentialGameJsonWhenGamePending()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game = new PotentialGame(100, "TestPlayer");
            WaitingRoomController.PendingGames.Add(100, game);

            // Act
            var result = controller.State(100, 0);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().BeOfType<PotentialGame>();
            var returnedGame = result.Value as IPotentialGame;
            returnedGame!.Id.Should().Be(100);
            returnedGame.CreatingPlayerName.Should().Be("TestPlayer");

        }

        [Fact]
        public void State_ReturnsStartedWhenGameMovedToActiveGames()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var players = new List<IPlayer>
            {
                new Player("Player1", 0),
                new Player("Player2", 1)
            };
            var activeGame = new GameBoard(players);

            GameController.ActiveGames.Add(100, activeGame);

            // Act
            var result = controller.State(100, 0);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be("started");

        }

        [Fact]
        public void State_ReturnsRemovedFromGameWhenPlayerRemoved()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game = new PotentialGame(100, "Creator");
            game.Players.Add(1, "Player1");
            WaitingRoomController.PendingGames.Add(100, game);

            // Remove player 1
            game.Players.Remove(1);

            // Act
            var result = controller.State(100, 1);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be("removed from game");

        }

        [Fact]
        public void State_ReturnsEmptyWhenGameDoesNotExist()
        {
            // Arrange
            var controller = new WaitingRoomController();

            // Act
            var result = controller.State(999, 0);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be("");

        }

        #endregion

        #region ListGames Tests

        [Fact]
        public void ListGames_ReturnsViewWithAllPendingGames()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game1 = new PotentialGame(100, "Player1");
            var game2 = new PotentialGame(200, "Player2");
            var game3 = new PotentialGame(300, "Player3");

            WaitingRoomController.PendingGames.Add(100, game1);
            WaitingRoomController.PendingGames.Add(200, game2);
            WaitingRoomController.PendingGames.Add(300, game3);

            // Act
            var result = controller.ListGames() as ViewResult;

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
        public void ListGamesState_ReturnsOnlyGamesWithAvailableSlots()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var gameWithSlots = new PotentialGame(100, "Player1");
            var fullGame = new PotentialGame(200, "Player2");

            // Fill the second game
            fullGame.Players.Add(1, "Player3");
            fullGame.Players.Add(2, "Player4");
            fullGame.Players.Add(3, "Player5");

            WaitingRoomController.PendingGames.Add(100, gameWithSlots);
            WaitingRoomController.PendingGames.Add(200, fullGame);

            // Act
            var result = controller.ListGamesState() as JsonResult;

            // Assert
            result.Should().NotBeNull();
            var games = result!.Value as List<IPotentialGame>;
            games.Should().NotBeNull();
            games!.Should().HaveCount(1);
            games.First().Id.Should().Be(100);

        }

        #endregion

        #region RemovePlayer Tests

        [Fact]
        public void RemovePlayer_RemovesPlayerFromGame()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game = new PotentialGame(100, "Creator");
            game.Players.Add(1, "Player1");
            game.Players.Add(2, "Player2");

            WaitingRoomController.PendingGames.Add(100, game);

            // Act
            var result = controller.RemovePlayer(100, 1) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            game.Players.Should().NotContainKey(1);
            game.Players.Should().HaveCount(2); // Creator + Player2

            var returnedGame = result!.Value as IPotentialGame;
            returnedGame.Should().NotBeNull();

        }

        [Fact]
        public void RemovePlayer_OnNonExistentGameReturnsEmptyJson()
        {
            // Arrange
            var controller = new WaitingRoomController();

            // Act
            var result = controller.RemovePlayer(999, 0);

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be("");

        }

        #endregion

        #region Edge Cases

        [Fact]
        public void MultipleGamesCanExistSimultaneously()
        {
            // Arrange
            var controller = new WaitingRoomController();

            // Act - Create games
            controller.NewGameInfo("Player1");
            var countAfterFirst = WaitingRoomController.PendingGames.Count;

            controller.NewGameInfo("Player2");
            var countAfterSecond = WaitingRoomController.PendingGames.Count;

            controller.NewGameInfo("Player3");

            // Assert
            countAfterFirst.Should().Be(1, "first game should be added");
            countAfterSecond.Should().Be(2, "second game should be added");
            WaitingRoomController.PendingGames.Should().HaveCount(3, "all three games should exist");

            var gameIds = WaitingRoomController.PendingGames.Keys.ToList();
            gameIds.Should().OnlyHaveUniqueItems("all game IDs should be unique");
        }

        [Fact]
        public void GameIds_AreUnique_WhenCreatingMultipleGames()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var gameIds = new List<int>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                controller.NewGameInfo($"Player{i}");
            }
            gameIds = WaitingRoomController.PendingGames.Keys.ToList();

            // Assert
            gameIds.Should().HaveCount(10);
            gameIds.Should().OnlyHaveUniqueItems();

        }

        [Fact]
        public void Cleanup_DoesNotAffectRecentlyCreatedGames()
        {
            // Arrange
            var controller = new WaitingRoomController();

            // Create a recent game
            var recentGame = new TestPotentialGame(100, "RecentPlayer", DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)));
            WaitingRoomController.PendingGames.Add(100, recentGame);

            // Act - Creating a new game triggers cleanup
            controller.NewGameInfo("NewPlayer");

            // Assert
            WaitingRoomController.PendingGames.Should().ContainKey(100);
            WaitingRoomController.PendingGames.Should().HaveCount(2); // Recent game + new game

        }

        #endregion

        #region Additional Coverage Tests

        [Fact]
        public void Index_ReturnsViewWhenGameExists()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game = new PotentialGame(100, "TestPlayer");
            WaitingRoomController.PendingGames.Add(100, game);

            // Act
            var result = controller.Index(100, 0) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewName.Should().Be("Index");
            result.Model.Should().Be(game);
            result.ViewData["GameId"].Should().Be(100);
            result.ViewData["PlayerId"].Should().Be(0);
            result.ViewData["IsCreator"].Should().Be("true");

        }

        [Fact]
        public void Index_ReturnsErrorViewWhenGameDoesNotExist()
        {
            // Arrange
            var controller = new WaitingRoomController();

            // Act
            var result = controller.Index(999, 0) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewName.Should().Be("Error");

        }

        [Fact]
        public void NewGame_ReturnsView()
        {
            // Arrange
            var controller = new WaitingRoomController();

            // Act
            var result = controller.NewGame() as ViewResult;

            // Assert
            result.Should().NotBeNull();

        }

        [Fact]
        public void EnterGame_SetsCorrectPlayerIdForMultipleJoins()
        {
            // Arrange
            var controller = new WaitingRoomController();
            var game = new PotentialGame(100, "Creator");
            WaitingRoomController.PendingGames.Add(100, game);

            // Act
            var result1 = controller.EnterGame(100, "Player1") as RedirectResult;
            var result2 = controller.EnterGame(100, "Player2") as RedirectResult;
            var result3 = controller.EnterGame(100, "Player3") as RedirectResult;

            // Assert
            game.Players.Should().HaveCount(4); // Creator + 3 players
            game.Players.Should().ContainKey(0);
            game.Players.Should().ContainKey(1);
            game.Players.Should().ContainKey(2);
            game.Players.Should().ContainKey(3);

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
            public Dictionary<int, string> Players { get; }
            public int MaxPlayers { get; } = 4;
            public DateTime TimeCreated { get; }

            public TestPotentialGame(int id, string creator, DateTime timeCreated)
            {
                Id = id;
                CreatingPlayerName = creator;
                TimeCreated = timeCreated;
                Players = new Dictionary<int, string>();
                Players.Add(0, creator);
            }
        }

        #endregion
    }
}
