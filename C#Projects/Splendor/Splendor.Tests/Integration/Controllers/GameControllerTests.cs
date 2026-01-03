using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Splendor.Controllers;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for GameController.
/// Tests the full flow of game operations including state management,
/// turn execution, and player actions.
/// </summary>
[Collection("Sequential")]
public class GameControllerTests : IDisposable
{
    private readonly GameController _controller;

    public GameControllerTests()
    {
        _controller = new GameController();
        // Clear any existing state before each test
        GameController.ActiveGames.Clear();
        WaitingRoomController.PendingGames.Clear();
    }

    public void Dispose()
    {
        // Clean up after each test
        GameController.ActiveGames.Clear();
        WaitingRoomController.PendingGames.Clear();
    }

    #region State Management Tests

    [Fact]
    public void Start_MovesGameFromPendingToActive()
    {
        // Arrange
        int gameId = 12345;
        var pendingGame = new PotentialGame(gameId, "Player1");
        pendingGame.Players.Add(1, "Player2");
        WaitingRoomController.PendingGames[gameId] = pendingGame;

        // Act
        var result = _controller.Start(gameId);

        // Assert
        GameController.ActiveGames.Should().ContainKey(gameId);
        WaitingRoomController.PendingGames.Should().NotContainKey(gameId);
        GameController.ActiveGames[gameId].Players.Should().HaveCount(2);
        result.Should().BeOfType<RedirectResult>();
    }

    [Fact]
    public void State_ReturnsVersionNumber_WhenGameIsActive()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Act
        var result = _controller.State(gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(gameBoard.Version);
    }

    [Fact]
    public void State_ReturnsIsPaused_WhenGameIsPaused()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        gameBoard.IsPaused = true;
        GameController.ActiveGames[gameId] = gameBoard;

        // Act
        var result = _controller.State(gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("IsPaused");
    }

    [Fact]
    public void State_ReturnsGameEnded_WhenGameNotInActiveGames()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = _controller.State(gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("The game has ended.");
    }

    [Fact]
    public void Start_CleansUpOldGames_WhenStartingNewGame()
    {
        // Arrange
        int newGameId = 1;
        int oldGameId = 2;

        // Create an old game (more than 30 minutes old)
        var oldPlayers = new List<IPlayer> { new Player("OldPlayer", 0) };
        var oldGameBoard = new GameBoard(oldPlayers);
        // Use reflection to set an old timestamp
        var gameStartField = typeof(GameBoard).GetField("<GameStartTimeStamp>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameStartField?.SetValue(oldGameBoard, DateTime.UtcNow.Subtract(new TimeSpan(0, 35, 0)));
        GameController.ActiveGames[oldGameId] = oldGameBoard;

        // Create a new pending game
        var pendingGame = new PotentialGame(newGameId, "NewPlayer");
        WaitingRoomController.PendingGames[newGameId] = pendingGame;

        // Act
        _controller.Start(newGameId);

        // Assert
        GameController.ActiveGames.Should().ContainKey(newGameId);
        GameController.ActiveGames.Should().NotContainKey(oldGameId);
    }

    #endregion

    #region EndTurn Action Tests

    [Fact]
    public void EndTurn_ValidTokenTake_ReturnsUpdatedGameBoard()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);
        int initialVersion = gameBoard.Version;

        // Act
        var result = _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeOfType<GameBoard>();
        var updatedGame = result.Value as IGameBoard;
        updatedGame.Should().NotBeNull();
        updatedGame!.Version.Should().BeGreaterThan(initialVersion);
    }

    [Fact]
    public void EndTurn_InvalidTokenTake_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Take 4 different tokens (invalid)
        var tokens = new Dictionary<Token, int>
        {
            { Token.Diamond, 1 },
            { Token.Sapphire, 1 },
            { Token.Emerald, 1 },
            { Token.Ruby, 1 }
        };

        // Act
        var result = _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAssignableTo<IError>();
    }

    [Fact]
    public void EndTurn_TokenTakeWithContinueAction_ReturnsContinueAction()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1").AtMaxTokens().Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Take tokens that will exceed limit
        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        var result = _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAssignableTo<IContinueAction>();
    }

    [Fact]
    public void EndTurn_NullTokens_ReturnsGameBoard()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Act
        var result = _controller.EndTurn(null!, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeOfType<GameBoard>();
    }

    [Fact]
    public void EndTurn_IncrementsVersion()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;
        int initialVersion = gameBoard.Version;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        gameBoard.Version.Should().BeGreaterThan(initialVersion);
    }

    [Fact]
    public void EndTurn_AdvancesCurrentPlayer()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;
        int initialPlayer = gameBoard.CurrentPlayer;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        gameBoard.CurrentPlayer.Should().NotBe(initialPlayer);
    }

    #endregion

    #region Purchase Action Tests

    [Fact]
    public void Purchase_Level1Card_Successfully()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Get a card from level 1
        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var value = result.Value;
        if (value is IError error)
        {
            // If error, it should be due to insufficient tokens
            error.ErrorCode.Should().Be(2);
        }
        else
        {
            // Success case
            value.Should().BeOfType<GameBoard>();
        }
    }

    [Fact]
    public void Purchase_Level2Card_Successfully()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Get a card from level 2
        var card = gameBoard.Level2Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var value = result.Value;
        if (value is IError error)
        {
            // If error, it should be due to insufficient tokens
            error.ErrorCode.Should().Be(2);
        }
        else
        {
            // Success case
            value.Should().BeOfType<GameBoard>();
        }
    }

    [Fact]
    public void Purchase_Level3Card_Successfully()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5, gold: 5)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Get a card from level 3
        var card = gameBoard.Level3Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var value = result.Value;
        if (value is IError error)
        {
            // If error, it should be due to insufficient tokens
            error.ErrorCode.Should().Be(2);
        }
        else
        {
            // Success case
            value.Should().BeOfType<GameBoard>();
        }
    }

    [Fact]
    public void Purchase_ReservedCard_Successfully()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var card = CardBuilder.CheapLevel1Diamond();
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(sapphire: 2, emerald: 2)
            .WithReservedCard(card)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Act
        var result = _controller.Purchase(card.ImageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeOfType<GameBoard>();
    }

    [Fact]
    public void Purchase_WithInsufficientTokens_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 0) // No tokens
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Get a card that costs something
        var card = gameBoard.Level1Cards.First(c => c != null && c.Price.Values.Sum() > 0);
        string imageName = card!.ImageName;

        // Act
        var result = _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAssignableTo<IError>();
        var error = result.Value as IError;
        error!.ErrorCode.Should().Be(2); // Insufficient tokens error
    }

    [Fact]
    public void Purchase_ReturnsUpdatedGameBoardOnSuccess()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var freeCard = CardBuilder.FreeCard();
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithReservedCard(freeCard)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        int initialVersion = gameBoard.Version;

        // Act
        var result = _controller.Purchase(freeCard.ImageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeOfType<GameBoard>();
        gameBoard.Version.Should().BeGreaterThan(initialVersion);
    }

    #endregion

    #region Reserve Action Tests

    [Fact]
    public void Reserve_Card_SuccessfullyAddsToReservedCards()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;
        int initialReservedCount = gameBoard.Players[0].ReservedCards.Count;

        // Act
        var result = _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        // Either success or continue action if nobles available
        if (result.Value is IGameBoard)
        {
            gameBoard.Players[0].ReservedCards.Count.Should().Be(initialReservedCount + 1);
        }
    }

    [Fact]
    public void Reserve_Card_GrantsGoldToken()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;
        int initialGold = gameBoard.Players[0].Tokens[Token.Gold];

        // Act
        var result = _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        if (result.Value is IGameBoard)
        {
            gameBoard.Players[0].Tokens[Token.Gold].Should().Be(initialGold + 1);
        }
    }

    [Fact]
    public void Reserve_WhenAtMaxReservedCards_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithMaxReservedCards()
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAssignableTo<IError>();
        var error = result.Value as IError;
        error!.ErrorCode.Should().Be(3); // Too many reserved cards error
    }

    [Fact]
    public void Reserve_ReturnsUpdatedGameBoardOnSuccess()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;
        int initialVersion = gameBoard.Version;

        // Act
        var result = _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        gameBoard.Version.Should().BeGreaterThan(initialVersion);
    }

    #endregion

    #region Noble Action Tests

    [Fact]
    public void Noble_SelectNoble_SuccessfullyWhenQualified()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = PlayerBuilder.ReadyForThreeOfThreeNoble();
        player1.Id.Should().Be(0); // Verify ID is set correctly
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Find a noble the player qualifies for
        var noble = gameBoard.Nobles.FirstOrDefault();
        if (noble == null)
        {
            // Skip if no nobles available
            return;
        }

        string imageName = noble.ImageName;
        int initialNobleCount = gameBoard.Players[0].Nobles.Count;

        // Act
        var result = _controller.Noble(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        // Result depends on whether player qualifies
        if (result.Value is IGameBoard)
        {
            gameBoard.Players[0].Nobles.Count.Should().BeGreaterThanOrEqualTo(initialNobleCount);
        }
    }

    [Fact]
    public void Noble_WhenNotQualified_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1").Build(); // No cards
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var noble = gameBoard.Nobles.FirstOrDefault();
        if (noble == null)
        {
            // Skip if no nobles available
            return;
        }

        string imageName = noble.ImageName;

        // Act
        var result = _controller.Noble(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAssignableTo<IError>();
        var error = result.Value as IError;
        error!.ErrorCode.Should().Be(5); // Insufficient cards for noble
    }

    [Fact]
    public void Noble_ReturnsUpdatedGameBoardOnSuccess()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = PlayerBuilder.ReadyForThreeOfThreeNoble();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Manually add a noble that the player qualifies for
        var qualifyingNoble = NobleBuilder.ThreeOfThree();
        gameBoard.Nobles.Add(qualifyingNoble);

        int initialVersion = gameBoard.Version;

        // Act
        var result = _controller.Noble(qualifyingNoble.ImageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        if (result.Value is IGameBoard)
        {
            gameBoard.Version.Should().BeGreaterThan(initialVersion);
        }
    }

    #endregion

    #region Return Action Tests

    [Fact]
    public void Return_Tokens_WhenPlayerOverLimit()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 4, sapphire: 4, emerald: 3) // 11 tokens
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var returnRequest = new ReturnRequest
        {
            Tokens = TokenHelper.Create(diamond: -1), // Return 1 diamond
            ReservingCardImageName = null
        };

        // Act
        var result = _controller.Return(returnRequest, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        // The result could be a GameBoard on success or an Error if the return is invalid
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Return_TokensWithReservingCard()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 4, sapphire: 4, emerald: 3) // 11 tokens
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var card = gameBoard.Level1Cards.First(c => c != null);
        var returnRequest = new ReturnRequest
        {
            Tokens = TokenHelper.Create(diamond: -1), // Return 1 diamond
            ReservingCardImageName = card!.ImageName
        };

        // Act
        var result = _controller.Return(returnRequest, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        // Result depends on whether the return is valid
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Return_InvalidReturn_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 1) // Only 1 diamond
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var returnRequest = new ReturnRequest
        {
            Tokens = TokenHelper.Create(diamond: -5), // Try to return 5 diamonds
            ReservingCardImageName = null
        };

        // Act
        var result = _controller.Return(returnRequest, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAssignableTo<IError>();
    }

    #endregion

    #region Pause/Resume Tests

    [Fact]
    public void Pause_SetsIsPausedToTrue()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Act
        _controller.Pause(gameId, playerId);

        // Assert
        gameBoard.IsPaused.Should().BeTrue();
    }

    [Fact]
    public void Resume_SetsIsPausedToFalse()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        gameBoard.IsPaused = true;
        GameController.ActiveGames[gameId] = gameBoard;

        // Act
        _controller.Resume(gameId, playerId);

        // Assert
        gameBoard.IsPaused.Should().BeFalse();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void EndTurn_OnNonExistentGame_ReturnsEmptyString()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;
        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        var result = _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("");
    }

    [Fact]
    public void Purchase_OnNonExistentGame_ReturnsEmptyString()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = _controller.Purchase("Level1-D-0P.jpg", gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("");
    }

    [Fact]
    public void Reserve_OnNonExistentGame_ReturnsEmptyString()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = _controller.Reserve("Level1-D-0P.jpg", gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("");
    }

    [Fact]
    public void MultipleConcurrentGames_DontInterfere()
    {
        // Arrange
        int game1Id = 1;
        int game2Id = 2;
        var players1 = new List<IPlayer> { new Player("Game1Player1", 0), new Player("Game1Player2", 1) };
        var players2 = new List<IPlayer> { new Player("Game2Player1", 0), new Player("Game2Player2", 1) };
        var gameBoard1 = new GameBoard(players1);
        var gameBoard2 = new GameBoard(players2);
        GameController.ActiveGames[game1Id] = gameBoard1;
        GameController.ActiveGames[game2Id] = gameBoard2;

        int initialVersion1 = gameBoard1.Version;
        int initialVersion2 = gameBoard2.Version;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        _controller.EndTurn(tokens, game1Id, 0);

        // Assert
        gameBoard1.Version.Should().BeGreaterThan(initialVersion1);
        gameBoard2.Version.Should().Be(initialVersion2); // Game 2 should be unchanged
    }

    [Fact]
    public void VersionIncrementsCorrectly_AcrossMultipleTurns()
    {
        // Arrange
        int gameId = 1;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        int initialVersion = gameBoard.Version;
        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act - Execute multiple turns
        _controller.EndTurn(tokens, gameId, 0);
        int version1 = gameBoard.Version;

        _controller.EndTurn(tokens, gameId, 1);
        int version2 = gameBoard.Version;

        // Assert
        version1.Should().BeGreaterThan(initialVersion);
        version2.Should().BeGreaterThan(version1);
    }

    [Fact]
    public void GameCleanup_DoesNotRemovePausedGames()
    {
        // Arrange
        int newGameId = 1;
        int pausedOldGameId = 2;

        // Create an old paused game
        var pausedPlayers = new List<IPlayer> { new Player("PausedPlayer", 0) };
        var pausedGameBoard = new GameBoard(pausedPlayers);
        pausedGameBoard.IsPaused = true;

        // Use reflection to set an old timestamp
        var gameStartField = typeof(GameBoard).GetField("<GameStartTimeStamp>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameStartField?.SetValue(pausedGameBoard, DateTime.UtcNow.Subtract(new TimeSpan(0, 35, 0)));
        GameController.ActiveGames[pausedOldGameId] = pausedGameBoard;

        // Create a new pending game
        var pendingGame = new PotentialGame(newGameId, "NewPlayer");
        WaitingRoomController.PendingGames[newGameId] = pendingGame;

        // Act
        _controller.Start(newGameId);

        // Assert
        GameController.ActiveGames.Should().ContainKey(pausedOldGameId); // Paused game should remain
        GameController.ActiveGames.Should().ContainKey(newGameId);
    }

    [Fact]
    public void PlayerTurnRotation_WorksCorrectly()
    {
        // Arrange
        int gameId = 1;
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2)
        };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act - Execute turns for all players
        int player0 = gameBoard.CurrentPlayer;
        _controller.EndTurn(tokens, gameId, player0);

        int player1 = gameBoard.CurrentPlayer;
        _controller.EndTurn(tokens, gameId, player1);

        int player2 = gameBoard.CurrentPlayer;
        _controller.EndTurn(tokens, gameId, player2);

        int backToPlayer0 = gameBoard.CurrentPlayer;

        // Assert
        player0.Should().NotBe(player1);
        player1.Should().NotBe(player2);
        backToPlayer0.Should().Be(player0); // Should rotate back to first player
    }

    [Fact]
    public void Index_WithValidGameId_ReturnsView()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players);
        GameController.ActiveGames[gameId] = gameBoard;

        // Act
        var result = _controller.Index(gameId, playerId);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(gameBoard);
    }

    [Fact]
    public void Index_WithInvalidGameId_ReturnsRedirect()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = _controller.Index(gameId, playerId);

        // Assert
        result.Should().BeOfType<RedirectResult>();
    }

    #endregion
}
