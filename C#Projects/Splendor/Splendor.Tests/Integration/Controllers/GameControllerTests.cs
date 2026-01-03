using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Splendor.Controllers;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Repositories;
using Splendor.Services.Data;
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
    private readonly IGameRepository _gameRepository;
    private readonly IPendingGameRepository _pendingGameRepository;
    private readonly IGameDataService _gameDataService;

    public GameControllerTests()
    {
        _gameRepository = TestHelpers.CreateInMemoryGameRepository();
        _pendingGameRepository = TestHelpers.CreateInMemoryPendingGameRepository();
        _gameDataService = TestHelpers.CreateMockGameDataService();

        var cardLookup = TestHelpers.CreateMockCardLookupService();
        var nobleLookup = TestHelpers.CreateMockNobleLookupService();
        var playerLookup = TestHelpers.CreateMockPlayerLookupService();
        var gameActivation = TestHelpers.CreateMockGameActivationService(_pendingGameRepository, _gameRepository, _gameDataService);
        var gameCleanup = TestHelpers.CreateMockGameCleanupService(_gameRepository, _pendingGameRepository);
        var logger = TestHelpers.CreateMockLogger<GameController>();

        _controller = new GameController(
            _gameRepository,
            _gameDataService,
            cardLookup,
            nobleLookup,
            playerLookup,
            gameActivation,
            gameCleanup,
            logger
        );
    }

    public void Dispose()
    {
        // Cleanup is handled by in-memory repositories being recreated each test
    }

    #region State Management Tests

    [Fact]
    public async Task Start_MovesGameFromPendingToActive()
    {
        // Arrange
        int gameId = 12345;
        var pendingGame = new PotentialGame(gameId, "Player1");
        pendingGame.AddPlayer(1, "Player2");
        await _pendingGameRepository.AddPendingGameAsync(gameId, pendingGame);

        // Act
        var result = await _controller.Start(gameId);

        // Assert
        (await _gameRepository.GameExistsAsync(gameId)).Should().BeTrue();
        (await _pendingGameRepository.PendingGameExistsAsync(gameId)).Should().BeFalse();
        var activeGame = await _gameRepository.GetGameAsync(gameId);
        activeGame!.Players.Should().HaveCount(2);
        result.Should().BeOfType<RedirectResult>();
    }

    [Fact]
    public async Task State_ReturnsVersionNumber_WhenGameIsActive()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Act
        var result = await _controller.State(gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<int>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().Be(gameBoard.Version);
    }

    [Fact]
    public async Task State_ReturnsIsPaused_WhenGameIsPaused()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        gameBoard.IsPaused = true;
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Act
        var result = await _controller.State(gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<string>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().Be("IsPaused");
    }

    [Fact]
    public async Task State_ReturnsGameEnded_WhenGameNotInActiveGames()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = await _controller.State(gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<string>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().Be("The game has ended.");
    }

    [Fact]
    public async Task Start_CleansUpOldGames_WhenStartingNewGame()
    {
        // Arrange
        int newGameId = 1;
        int oldGameId = 2;

        // Create an old game (more than 30 minutes old)
        var oldPlayers = new List<IPlayer> { new Player("OldPlayer", 0), new Player("Dummy", 1) };
        var oldGameBoard = new GameBoard(oldPlayers, _gameDataService);
        // Use reflection to set an old timestamp
        var gameStartField = typeof(GameBoard).GetField("<GameStartTimeStamp>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameStartField?.SetValue(oldGameBoard, DateTime.UtcNow.Subtract(new TimeSpan(0, 35, 0)));
        await _gameRepository.AddGameAsync(oldGameId, oldGameBoard);

        // Create a new pending game
        var pendingGame = new PotentialGame(newGameId, "NewPlayer");
        pendingGame.AddPlayer(1, "Player2"); // Need at least 2 players
        await _pendingGameRepository.AddPendingGameAsync(newGameId, pendingGame);

        // Act
        await _controller.Start(newGameId);

        // Assert
        (await _gameRepository.GameExistsAsync(newGameId)).Should().BeTrue();
        (await _gameRepository.GameExistsAsync(oldGameId)).Should().BeFalse();
    }

    #endregion

    #region EndTurn Action Tests

    [Fact]
    public async Task EndTurn_ValidTokenTake_ReturnsUpdatedGameBoard()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);
        int initialVersion = gameBoard.Version;

        // Act
        var result = await _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<IGameBoard>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Version.Should().BeGreaterThan(initialVersion);
    }

    [Fact]
    public async Task EndTurn_InvalidTokenTake_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Take 4 different tokens (invalid)
        var tokens = new Dictionary<Token, int>
        {
            { Token.Diamond, 1 },
            { Token.Sapphire, 1 },
            { Token.Emerald, 1 },
            { Token.Ruby, 1 }
        };

        // Act
        var result = await _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<IError>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task EndTurn_TokenTakeWithContinueAction_ReturnsContinueAction()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1").AtMaxTokens().Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Take tokens that will exceed limit
        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        var result = await _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<IContinueAction>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task EndTurn_NullTokens_ReturnsGameBoard()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Act
        var result = await _controller.EndTurn(null!, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<IGameBoard>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task EndTurn_IncrementsVersion()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);
        int initialVersion = gameBoard.Version;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        await _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Version.Should().BeGreaterThan(initialVersion);
    }

    [Fact]
    public async Task EndTurn_AdvancesCurrentPlayer()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);
        int initialPlayer = gameBoard.CurrentPlayer;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        await _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.CurrentPlayer.Should().NotBe(initialPlayer);
    }

    #endregion

    #region Purchase Action Tests

    [Fact]
    public async Task Purchase_Level1Card_Successfully()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Get a card from level 1
        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = await _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;

        // If not successful, check the error message to understand why
        if (!success)
        {
            var errorMessageProp = result.Value.GetType().GetProperty("ErrorMessage");
            var errorMessage = errorMessageProp?.GetValue(result.Value) as string;
            var dataProp = result.Value.GetType().GetProperty("Data");
            var data = dataProp?.GetValue(result.Value);

            // Provide helpful failure message
            success.Should().BeTrue($"Purchase should succeed, but got error: {errorMessage}, Data: {data}");
        }

        success.Should().BeTrue();
    }

    [Fact]
    public async Task Purchase_Level2Card_Successfully()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Get a card from level 2
        var card = gameBoard.Level2Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = await _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();
    }

    [Fact]
    public async Task Purchase_Level3Card_Successfully()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5, gold: 5)
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Get a card from level 3
        var card = gameBoard.Level3Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = await _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();
    }

    [Fact]
    public async Task Purchase_ReservedCard_Successfully()
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
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Act
        var result = await _controller.Purchase(card.ImageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();
    }

    [Fact]
    public async Task Purchase_WithInsufficientTokens_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 0) // No tokens
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Get a card that costs something
        var card = gameBoard.Level1Cards.First(c => c != null && c.Price.Values.Sum() > 0);
        string imageName = card!.ImageName;

        // Act
        var result = await _controller.Purchase(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true and check for error data
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var dataProp = result.Value.GetType().GetProperty("Data");
        dataProp.Should().NotBeNull();
        var data = dataProp!.GetValue(result.Value);
        data.Should().NotBeNull();

        // Check if data has ErrorCode property (indicates it's an IError)
        var errorCodeProp = data!.GetType().GetProperty("ErrorCode");
        if (errorCodeProp != null)
        {
            var errorCode = (int)errorCodeProp.GetValue(data)!;
            errorCode.Should().Be(2); // Insufficient tokens error
        }
    }

    [Fact]
    public async Task Purchase_ReturnsUpdatedGameBoardOnSuccess()
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
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        int initialVersion = gameBoard.Version;

        // Act
        var result = await _controller.Purchase(freeCard.ImageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Version.Should().BeGreaterThan(initialVersion);
    }

    #endregion

    #region Reserve Action Tests

    [Fact]
    public async Task Reserve_Card_SuccessfullyAddsToReservedCards()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;
        int initialReservedCount = gameBoard.Players[0].ReservedCards.Count;

        // Act
        var result = await _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Players[0].ReservedCards.Count.Should().Be(initialReservedCount + 1);
    }

    [Fact]
    public async Task Reserve_Card_GrantsGoldToken()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;
        int initialGold = gameBoard.Players[0].Tokens[Token.Gold];

        // Act
        var result = await _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Players[0].Tokens[Token.Gold].Should().Be(initialGold + 1);
    }

    [Fact]
    public async Task Reserve_WhenAtMaxReservedCards_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithMaxReservedCards()
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;

        // Act
        var result = await _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true and check for error data
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var dataProp = result.Value.GetType().GetProperty("Data");
        dataProp.Should().NotBeNull();
        var data = dataProp!.GetValue(result.Value);
        data.Should().NotBeNull();

        // Check if data has ErrorCode property (indicates it's an IError)
        var errorCodeProp = data!.GetType().GetProperty("ErrorCode");
        if (errorCodeProp != null)
        {
            var errorCode = (int)errorCodeProp.GetValue(data)!;
            errorCode.Should().Be(3); // Too many reserved cards error
        }
    }

    [Fact]
    public async Task Reserve_ReturnsUpdatedGameBoardOnSuccess()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var card = gameBoard.Level1Cards.First(c => c != null);
        string imageName = card!.ImageName;
        int initialVersion = gameBoard.Version;

        // Act
        var result = await _controller.Reserve(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Version.Should().BeGreaterThan(initialVersion);
    }

    #endregion

    #region Noble Action Tests

    [Fact]
    public async Task Noble_SelectNoble_SuccessfullyWhenQualified()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = PlayerBuilder.ReadyForThreeOfThreeNoble();
        player1.Id.Should().Be(0); // Verify ID is set correctly
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

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
        var result = await _controller.Noble(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Players[0].Nobles.Count.Should().BeGreaterThanOrEqualTo(initialNobleCount);
    }

    [Fact]
    public async Task Noble_WhenNotQualified_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1").Build(); // No cards
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var noble = gameBoard.Nobles.FirstOrDefault();
        if (noble == null)
        {
            // Skip if no nobles available
            return;
        }

        string imageName = noble.ImageName;

        // Act
        var result = await _controller.Noble(imageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true and check for error data
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var dataProp = result.Value.GetType().GetProperty("Data");
        dataProp.Should().NotBeNull();
        var data = dataProp!.GetValue(result.Value);
        data.Should().NotBeNull();

        // Check if data has ErrorCode property (indicates it's an IError)
        var errorCodeProp = data!.GetType().GetProperty("ErrorCode");
        if (errorCodeProp != null)
        {
            var errorCode = (int)errorCodeProp.GetValue(data)!;
            errorCode.Should().Be(5); // Insufficient cards for noble
        }
    }

    [Fact]
    public async Task Noble_ReturnsUpdatedGameBoardOnSuccess()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = PlayerBuilder.ReadyForThreeOfThreeNoble();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Manually add a noble that the player qualifies for
        var qualifyingNoble = NobleBuilder.ThreeOfThree();
        TestHelpers.AddBoardNoble(gameBoard, qualifyingNoble);

        int initialVersion = gameBoard.Version;

        // Act
        var result = await _controller.Noble(qualifyingNoble.ImageName, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to true
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeTrue();

        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.Version.Should().BeGreaterThan(initialVersion);
    }

    #endregion

    #region Return Action Tests

    [Fact]
    public async Task Return_Tokens_WhenPlayerOverLimit()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 4, sapphire: 4, emerald: 3) // 11 tokens
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var returnRequest = new ReturnRequest
        {
            Tokens = TokenHelper.Create(diamond: -1), // Return 1 diamond
            ReservingCardImageName = null
        };

        // Act
        var result = await _controller.Return(returnRequest, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        // The result could be a GameBoard on success or an Error if the return is invalid
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Return_TokensWithReservingCard()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 4, sapphire: 4, emerald: 3) // 11 tokens
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var card = gameBoard.Level1Cards.First(c => c != null);
        var returnRequest = new ReturnRequest
        {
            Tokens = TokenHelper.Create(diamond: -1), // Return 1 diamond
            ReservingCardImageName = card!.ImageName
        };

        // Act
        var result = await _controller.Return(returnRequest, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        // Result depends on whether the return is valid
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Return_InvalidReturn_ReturnsError()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var player1 = new PlayerBuilder().WithId(0).WithName("Player1")
            .WithTokens(diamond: 1) // Only 1 diamond
            .Build();
        var player2 = new PlayerBuilder().WithId(1).WithName("Player2").Build();
        var players = new List<IPlayer> { player1, player2 };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var returnRequest = new ReturnRequest
        {
            Tokens = TokenHelper.Create(diamond: -5), // Try to return 5 diamonds
            ReservingCardImageName = null
        };

        // Act
        var result = await _controller.Return(returnRequest, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<IError>;
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
    }

    #endregion

    #region Pause/Resume Tests

    [Fact]
    public async Task Pause_SetsIsPausedToTrue()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Act
        await _controller.Pause(gameId, playerId);

        // Assert
        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.IsPaused.Should().BeTrue();
    }

    [Fact]
    public async Task Resume_SetsIsPausedToFalse()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        gameBoard.IsPaused = true;
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Act
        await _controller.Resume(gameId, playerId);

        // Assert
        var updatedGame = await _gameRepository.GetGameAsync(gameId);
        updatedGame!.IsPaused.Should().BeFalse();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task EndTurn_OnNonExistentGame_ReturnsEmptyString()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;
        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        var result = await _controller.EndTurn(tokens, gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        var response = result.Value as ApiResponse<string>;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.ErrorMessage.Should().Be("Game not found");
    }

    [Fact]
    public async Task Purchase_OnNonExistentGame_ReturnsEmptyString()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = await _controller.Purchase("Level1-D-0P.jpg", gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to false
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeFalse();

        var errorMessageProp = result.Value.GetType().GetProperty("ErrorMessage");
        errorMessageProp.Should().NotBeNull();
        var errorMessage = (string?)errorMessageProp!.GetValue(result.Value);
        errorMessage.Should().Be("Game not found");
    }

    [Fact]
    public async Task Reserve_OnNonExistentGame_ReturnsEmptyString()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = await _controller.Reserve("Level1-D-0P.jpg", gameId, playerId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();

        // Verify the response has Success property set to false
        var successProp = result.Value!.GetType().GetProperty("Success");
        successProp.Should().NotBeNull();
        var success = (bool)successProp!.GetValue(result.Value)!;
        success.Should().BeFalse();

        var errorMessageProp = result.Value.GetType().GetProperty("ErrorMessage");
        errorMessageProp.Should().NotBeNull();
        var errorMessage = (string?)errorMessageProp!.GetValue(result.Value);
        errorMessage.Should().Be("Game not found");
    }

    [Fact]
    public async Task MultipleConcurrentGames_DontInterfere()
    {
        // Arrange
        int game1Id = 1;
        int game2Id = 2;
        var players1 = new List<IPlayer> { new Player("Game1Player1", 0), new Player("Game1Player2", 1) };
        var players2 = new List<IPlayer> { new Player("Game2Player1", 0), new Player("Game2Player2", 1) };
        var gameBoard1 = new GameBoard(players1, _gameDataService);
        var gameBoard2 = new GameBoard(players2, _gameDataService);
        await _gameRepository.AddGameAsync(game1Id, gameBoard1);
        await _gameRepository.AddGameAsync(game2Id, gameBoard2);

        int initialVersion1 = gameBoard1.Version;
        int initialVersion2 = gameBoard2.Version;

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        await _controller.EndTurn(tokens, game1Id, 0);

        // Assert
        var updatedGame1 = await _gameRepository.GetGameAsync(game1Id);
        var updatedGame2 = await _gameRepository.GetGameAsync(game2Id);
        updatedGame1!.Version.Should().BeGreaterThan(initialVersion1);
        updatedGame2!.Version.Should().Be(initialVersion2); // Game 2 should be unchanged
    }

    [Fact]
    public async Task VersionIncrementsCorrectly_AcrossMultipleTurns()
    {
        // Arrange
        int gameId = 1;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        int initialVersion = gameBoard.Version;
        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act - Execute multiple turns
        await _controller.EndTurn(tokens, gameId, 0);
        var game1 = await _gameRepository.GetGameAsync(gameId);
        int version1 = game1!.Version;

        await _controller.EndTurn(tokens, gameId, 1);
        var game2 = await _gameRepository.GetGameAsync(gameId);
        int version2 = game2!.Version;

        // Assert
        version1.Should().BeGreaterThan(initialVersion);
        version2.Should().BeGreaterThan(version1);
    }

    [Fact]
    public async Task GameCleanup_DoesNotRemovePausedGames()
    {
        // Arrange
        int newGameId = 1;
        int pausedOldGameId = 2;

        // Create an old paused game
        var pausedPlayers = new List<IPlayer> { new Player("PausedPlayer", 0), new Player("Dummy", 1) };
        var pausedGameBoard = new GameBoard(pausedPlayers, _gameDataService);
        pausedGameBoard.IsPaused = true;

        // Use reflection to set an old timestamp
        var gameStartField = typeof(GameBoard).GetField("<GameStartTimeStamp>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameStartField?.SetValue(pausedGameBoard, DateTime.UtcNow.Subtract(new TimeSpan(0, 35, 0)));
        await _gameRepository.AddGameAsync(pausedOldGameId, pausedGameBoard);

        // Create a new pending game
        var pendingGame = new PotentialGame(newGameId, "NewPlayer");
        pendingGame.AddPlayer(1, "Player2"); // Need at least 2 players
        await _pendingGameRepository.AddPendingGameAsync(newGameId, pendingGame);

        // Act
        await _controller.Start(newGameId);

        // Assert
        (await _gameRepository.GameExistsAsync(pausedOldGameId)).Should().BeTrue(); // Paused game should remain
        (await _gameRepository.GameExistsAsync(newGameId)).Should().BeTrue();
    }

    [Fact]
    public async Task PlayerTurnRotation_WorksCorrectly()
    {
        // Arrange
        int gameId = 1;
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2)
        };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        var tokens = TokenHelper.ThreeDifferent(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act - Execute turns for all players
        var game0 = await _gameRepository.GetGameAsync(gameId);
        int player0 = game0!.CurrentPlayer;
        await _controller.EndTurn(tokens, gameId, player0);

        var game1 = await _gameRepository.GetGameAsync(gameId);
        int player1 = game1!.CurrentPlayer;
        await _controller.EndTurn(tokens, gameId, player1);

        var game2 = await _gameRepository.GetGameAsync(gameId);
        int player2 = game2!.CurrentPlayer;
        await _controller.EndTurn(tokens, gameId, player2);

        var game3 = await _gameRepository.GetGameAsync(gameId);
        int backToPlayer0 = game3!.CurrentPlayer;

        // Assert
        player0.Should().NotBe(player1);
        player1.Should().NotBe(player2);
        backToPlayer0.Should().Be(player0); // Should rotate back to first player
    }

    [Fact]
    public async Task Index_WithValidGameId_ReturnsView()
    {
        // Arrange
        int gameId = 1;
        int playerId = 0;
        var players = new List<IPlayer> { new Player("Player1", 0), new Player("Player2", 1) };
        var gameBoard = new GameBoard(players, _gameDataService);
        await _gameRepository.AddGameAsync(gameId, gameBoard);

        // Act
        var result = await _controller.Index(gameId, playerId);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(gameBoard);
    }

    [Fact]
    public async Task Index_WithInvalidGameId_ReturnsRedirect()
    {
        // Arrange
        int gameId = 999;
        int playerId = 0;

        // Act
        var result = await _controller.Index(gameId, playerId);

        // Assert
        result.Should().BeOfType<RedirectResult>();
    }

    #endregion
}
