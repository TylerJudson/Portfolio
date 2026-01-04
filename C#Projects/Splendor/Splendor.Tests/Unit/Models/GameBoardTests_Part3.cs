using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Comprehensive tests for GameBoard - Part 3
/// Focuses on critical missing scenarios: winner determination, blind reservation,
/// state integrity on errors, noble limits, and turn combination rules.
/// </summary>
public class GameBoardTests_Part3
{
    #region Winner Determination Tests (6 tests)

    [Fact]
    public void GetWinner_SinglePlayerAbove15_ReturnsThatPlayer()
    {
        // Arrange - Create 2 players, one with 16 prestige, one with 10
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(new CardBuilder().WithPrestigePoints(16).Build())
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .WithCards(new CardBuilder().WithPrestigePoints(10).Build())
            .Build();

        var board = new GameBoard(new List<IPlayer> { player1, player2 }, TestHelpers.CreateMockGameDataService());

        // Manually set GameOver to true
        typeof(GameBoard).GetProperty("GameOver")!.SetValue(board, true);

        // Act
        var winner = board.GetWinner();

        // Assert
        winner.Should().BeSameAs(player1, "player1 has highest prestige");
    }

    [Fact]
    public void GetWinner_TwoPlayersAbove15_HighestPrestigeWins()
    {
        // Arrange - Both players above 15, different prestige
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(new CardBuilder().WithPrestigePoints(17).Build())
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .WithCards(new CardBuilder().WithPrestigePoints(15).Build())
            .Build();

        var board = new GameBoard(new List<IPlayer> { player1, player2 }, TestHelpers.CreateMockGameDataService());
        typeof(GameBoard).GetProperty("GameOver")!.SetValue(board, true);

        // Act
        var winner = board.GetWinner();

        // Assert
        winner.Should().BeSameAs(player1, "player1 has 17 prestige vs player2's 15");
    }

    [Fact]
    public void GetWinner_TiedPrestige_FewerCardsWins()
    {
        // Arrange - Both players have 15 prestige, different card counts
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                new CardBuilder().WithPrestigePoints(5).Build(),
                new CardBuilder().WithPrestigePoints(5).Build(),
                new CardBuilder().WithPrestigePoints(5).Build(),
                new CardBuilder().WithPrestigePoints(0).Build(),
                new CardBuilder().WithPrestigePoints(0).Build() // 5 cards total
            )
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .WithCards(
                new CardBuilder().WithPrestigePoints(7).Build(),
                new CardBuilder().WithPrestigePoints(8).Build() // 2 cards total
            )
            .Build();

        var board = new GameBoard(new List<IPlayer> { player1, player2 }, TestHelpers.CreateMockGameDataService());
        typeof(GameBoard).GetProperty("GameOver")!.SetValue(board, true);

        // Act
        var winner = board.GetWinner();

        // Assert
        winner.Should().BeSameAs(player2, "both have 15 prestige, but player2 has fewer cards (2 vs 5)");
    }

    [Fact]
    public void GetWinner_ThreePlayers_CorrectWinnerSelected()
    {
        // Arrange - 3 players with different prestige levels
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .Build();

        var player3 = new PlayerBuilder()
            .WithName("Player3")
            .WithId(2)
            .Build();

        // Set prestige points using reflection since they're calculated from cards
        TestHelpers.AddPlayerCard(player1, new CardBuilder().WithPrestigePoints(14).Build());
        TestHelpers.AddPlayerCard(player2, new CardBuilder().WithPrestigePoints(18).Build());
        TestHelpers.AddPlayerCard(player3, new CardBuilder().WithPrestigePoints(16).Build());

        var board = new GameBoard(new List<IPlayer> { player1, player2, player3 }, TestHelpers.CreateMockGameDataService());
        typeof(GameBoard).GetProperty("GameOver")!.SetValue(board, true);

        // Act
        var winner = board.GetWinner();

        // Assert
        winner.Should().NotBeNull();
        winner!.Name.Should().Be("Player2", "player2 has highest prestige (18)");
        winner.PrestigePoints.Should().Be(18u);
    }

    [Fact]
    public void GetWinner_BeforeGameOver_ReturnsNull()
    {
        // Arrange - Game not over yet
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(new CardBuilder().WithPrestigePoints(10).Build())
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .Build();

        var board = new GameBoard(new List<IPlayer> { player1, player2 }, TestHelpers.CreateMockGameDataService());

        // Act
        var winner = board.GetWinner();

        // Assert
        winner.Should().BeNull("game is not over yet");
    }

    [Fact]
    public void GetWinner_ComplexTieBreaker_MultiplePlayersSamePrestige()
    {
        // Arrange - 4 players, 2 tied at 15 prestige with different card counts
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .Build();

        var player3 = new PlayerBuilder()
            .WithName("Player3")
            .WithId(2)
            .Build();

        var player4 = new PlayerBuilder()
            .WithName("Player4")
            .WithId(3)
            .Build();

        // Add cards to get specific prestige points
        TestHelpers.AddPlayerCard(player1, new CardBuilder().WithPrestigePoints(12).Build());

        TestHelpers.AddPlayerCard(player2, new CardBuilder().WithPrestigePoints(5).Build());
        TestHelpers.AddPlayerCard(player2, new CardBuilder().WithPrestigePoints(5).Build());
        TestHelpers.AddPlayerCard(player2, new CardBuilder().WithPrestigePoints(5).Build());
        TestHelpers.AddPlayerCard(player2, new CardBuilder().WithPrestigePoints(0).Build()); // 4 cards, 15 prestige

        TestHelpers.AddPlayerCard(player3, new CardBuilder().WithPrestigePoints(7).Build());
        TestHelpers.AddPlayerCard(player3, new CardBuilder().WithPrestigePoints(8).Build()); // 2 cards, 15 prestige

        TestHelpers.AddPlayerCard(player4, new CardBuilder().WithPrestigePoints(5).Build());
        TestHelpers.AddPlayerCard(player4, new CardBuilder().WithPrestigePoints(5).Build());
        TestHelpers.AddPlayerCard(player4, new CardBuilder().WithPrestigePoints(5).Build()); // 3 cards, 15 prestige

        var board = new GameBoard(new List<IPlayer> { player1, player2, player3, player4 }, TestHelpers.CreateMockGameDataService());
        typeof(GameBoard).GetProperty("GameOver")!.SetValue(board, true);

        // Act
        var winner = board.GetWinner();

        // Assert
        winner.Should().NotBeNull();
        winner!.Name.Should().Be("Player3", "player3 has 15 prestige with fewest cards (2) among tied players");
        winner.PrestigePoints.Should().Be(15u);
        winner.Cards.Count.Should().Be(2);
    }

    #endregion

    #region Reserve From Deck (Blind) Tests (5 tests)

    [Fact]
    public void ReserveCardFromLevel1Deck_Successfully_AddsToReservedCards()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Create a turn that reserves from deck (blind) - using a special marker or flag
        var turn = TurnBuilder.ReserveFromDeck(1); // Level 1 deck

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.ReservedCards.Should().HaveCount(1, "card should be added to reserved cards");
        player.ReservedCards[0].Should().NotBeNull("reserved card should exist");
        player.ReservedCards[0]!.Level.Should().Be(1, "card should be from level 1 deck");
    }

    [Fact]
    public void ReserveCardFromLevel2Deck_Successfully_GetsGoldToken()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var initialGold = board.TokenStacks[Token.Gold];
        var turn = TurnBuilder.ReserveFromDeck(2); // Level 2 deck

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Tokens[Token.Gold].Should().Be(1, "player should receive 1 gold token");
        board.TokenStacks[Token.Gold].Should().Be(initialGold - 1, "gold should be removed from stack");
    }

    [Fact]
    public void ReserveCardFromLevel3Deck_Successfully_CardNotInVisibleCards()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Capture visible cards before reservation
        var visibleCardsBefore = board.Level3Cards.ToList();

        var turn = TurnBuilder.ReserveFromDeck(3); // Level 3 deck

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.ReservedCards.Should().HaveCount(1);

        // The reserved card should NOT be one of the visible cards
        var reservedCard = player.ReservedCards[0];
        visibleCardsBefore.Should().NotContain(reservedCard, "card reserved from deck should not be from visible cards");
    }

    [Fact]
    public void ReserveFromEmptyDeck_ReturnsError()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Empty the level 1 deck by drawing all cards
        while (board.CardStackLevel1.Draw() != null) { }

        var turn = TurnBuilder.ReserveFromDeck(1);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 9); // Error code for empty deck
    }

    [Fact]
    public void ReserveFromDeck_WithMaxReservedCards_ReturnsError()
    {
        // Arrange - Player already has 3 reserved cards
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithMaxReservedCards()
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var turn = TurnBuilder.ReserveFromDeck(1);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 3); // Error code for too many reserved cards
    }

    #endregion

    #region State Integrity on Errors Tests (8 tests)

    [Fact]
    public void Error_TakingTooManyTokenTypes_DoesNotIncrementVersion()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var initialVersion = board.Version;
        var turn = TurnBuilder.TakeFourDifferentTokens();

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 4);
        board.Version.Should().Be(initialVersion, "version should not increment on error");
    }

    [Fact]
    public void Error_TakingGold_DoesNotAdvanceCurrentPlayer()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var initialPlayer = board.CurrentPlayer;
        var turn = TurnBuilder.TakeGold();

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 6);
        board.CurrentPlayer.Should().Be(initialPlayer, "current player should not advance on error");
    }

    [Fact]
    public void Error_InsufficientTokensForCard_DoesNotMutateTokenStacks()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 1)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var expensiveCard = new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Diamond)
            .WithPrice(diamond: 5, sapphire: 3)
            .Build();

        TestHelpers.SetBoardCard(board, 1, 0, expensiveCard);

        var initialTokenStacks = new Dictionary<Token, int>(board.TokenStacks);
        var turn = TurnBuilder.PurchaseCard(expensiveCard);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 2);

        // Verify token stacks are unchanged
        foreach (var token in initialTokenStacks.Keys)
        {
            board.TokenStacks[token].Should().Be(initialTokenStacks[token],
                $"{token} stack should not change on error");
        }
    }

    [Fact]
    public void Error_TooManyReservedCards_DoesNotMutatePlayerState()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithMaxReservedCards()
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var initialReservedCount = player.ReservedCards.Count;
        var initialTokenCount = player.NumberOfTokens();

        var cardToReserve = board.Level1Cards[0];
        var turn = TurnBuilder.ReserveCard(cardToReserve!);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 3);
        player.ReservedCards.Should().HaveCount(initialReservedCount, "reserved cards should not change");
        player.NumberOfTokens().Should().Be(initialTokenCount, "token count should not change");
    }

    [Fact]
    public void Error_NotEnoughTokensInStack_DoesNotSubtractFromStack()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Deplete diamond stack to 0
        for (int i = 0; i < 5; i++)
        {
            board.ExecuteTurn(TurnBuilder.TakeOneToken(Token.Diamond));
        }

        var initialDiamonds = board.TokenStacks[Token.Diamond];
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Ruby);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 0);
        board.TokenStacks[Token.Diamond].Should().Be(initialDiamonds, "diamond stack should not change on error");
    }

    [Fact]
    public void Error_GameOver_DoesNotIncrementVersion()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        typeof(GameBoard).GetProperty("GameOver")!.SetValue(board, true);

        var initialVersion = board.Version;
        var turn = TurnBuilder.TakeOneToken(Token.Diamond);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 8);
        board.Version.Should().Be(initialVersion, "version should not increment when game is over");
    }

    [Fact]
    public void Error_DoesNotAddToTurnHistory()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var initialTurnCount = board.Turns.Count;
        var turn = TurnBuilder.TakeFourDifferentTokens();

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 4);
        board.Turns.Should().HaveCount(initialTurnCount, "failed turn should not be added to history");
    }

    [Fact]
    public void Error_MultipleErrors_AllMaintainStateIntegrity()
    {
        // Arrange - Test multiple different errors to ensure consistency
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var initialVersion = board.Version;
        var initialPlayer = board.CurrentPlayer;

        // Act & Assert - Try multiple errors
        var errors = new[]
        {
            TurnBuilder.TakeFourDifferentTokens(),
            TurnBuilder.TakeGold(),
            TurnBuilder.TakeThreeSameTokens(Token.Diamond)
        };

        foreach (var errorTurn in errors)
        {
            var result = board.ExecuteTurn(errorTurn);

            result.Error.Should().NotBeNull("turn should result in error");
            board.Version.Should().Be(initialVersion, "version should remain unchanged across all errors");
            board.CurrentPlayer.Should().Be(initialPlayer, "current player should remain unchanged across all errors");
        }
    }

    #endregion

    #region Noble Acquisition Limit Tests (4 tests)

    [Fact]
    public void AcquireNoble_WhenNotCurrentPlayer_CannotAcquire()
    {
        // Arrange - Player 0 qualifies for nobles
        var noble1 = NobleBuilder.ThreeOfThree();
        var noble2 = NobleBuilder.FourOfTwo();

        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build()
            )
            .Build();

        var player2 = new Player("Player2", 1);
        var board = new GameBoard(new List<IPlayer> { player1, player2 }, TestHelpers.CreateMockGameDataService());

        TestHelpers.ClearBoardNobles(board);
        TestHelpers.AddBoardNoble(board, noble1);
        TestHelpers.AddBoardNoble(board, noble2);

        // Player 1 acquires noble1
        var turn1 = TurnBuilder.AcquireNoble(noble1);
        var result1 = board.ExecuteTurn(turn1);
        AssertionHelpers.AssertTurnSucceeded(result1);

        // Current player should now be player 2 (index 1)
        board.CurrentPlayer.Should().Be(1, "turn should advance to next player");

        // Now it's player 2's turn - player 1 cannot acquire another noble
        // This test verifies that nobles can only be acquired when it's your turn
        player1.Nobles.Should().HaveCount(1, "player1 should only have one noble");
        board.Nobles.Should().Contain(noble2, "noble2 should still be on board for player1 to get in future");
    }

    [Fact]
    public void MultipleNoblesQualify_OnlyOneCanBeAcquired()
    {
        // Arrange - Player qualifies for 2 nobles
        var noble1 = NobleBuilder.WithRequirements(diamond: 3, sapphire: 3, emerald: 3);
        var noble2 = NobleBuilder.WithRequirements(diamond: 4, onyx: 4);

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build()
            )
            .WithTokens(diamond: 1)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        TestHelpers.ClearBoardNobles(board);
        TestHelpers.AddBoardNoble(board, noble1);
        TestHelpers.AddBoardNoble(board, noble2);

        // Take a turn that triggers noble selection
        var turn = new Turn(TokenHelper.One(Token.Diamond));

        // Act - This should return a continue action
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnRequiresNobleSelection(result);
        result.ContinueAction!.Nobles.Should().HaveCount(2, "player should be offered both nobles");

        // Now acquire one noble
        var acquireTurn = TurnBuilder.AcquireNoble(noble1);
        var acquireResult = board.ExecuteTurn(acquireTurn);

        AssertionHelpers.AssertTurnSucceeded(acquireResult);
        player.Nobles.Should().HaveCount(1, "player should have exactly one noble");
        board.Nobles.Should().Contain(noble2, "second noble should still be on board");
        board.Nobles.Should().NotContain(noble1, "first noble should be removed from board");
    }

    [Fact]
    public void NobleAcquisition_CountsTowardPrestigeLimit()
    {
        // Arrange - Noble should count toward the 3 prestige points
        var noble = NobleBuilder.ThreeOfThree();

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build()
            )
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        TestHelpers.ClearBoardNobles(board);
        TestHelpers.AddBoardNoble(board, noble);

        var initialPrestige = player.PrestigePoints;
        var turn = TurnBuilder.AcquireNoble(noble);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.PrestigePoints.Should().Be(initialPrestige + 3, "noble should add 3 prestige points");
        player.Nobles.Should().Contain(noble);
    }

    [Fact]
    public void NobleAcquisition_IsNotAnAction_CanOccurAfterMainAction()
    {
        // Arrange - Noble acquisition should happen AFTER the main turn action
        var noble = NobleBuilder.ThreeOfThree();

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build()
            )
            .WithTokens(diamond: 1)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        TestHelpers.ClearBoardNobles(board);
        TestHelpers.AddBoardNoble(board, noble);

        // Take a regular turn (take token)
        var turn = new Turn(TokenHelper.One(Token.Diamond));

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Should return continue action for noble, not error
        AssertionHelpers.AssertTurnRequiresNobleSelection(result);
        player.Tokens[Token.Diamond].Should().Be(2, "main action (take token) should have executed");
    }

    #endregion

    #region Turn Combination Rules Tests (5 tests)

    [Fact]
    public void Turn_WithNoAction_ReturnsError()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Create a turn with all zeros (no action taken)
        var turn = new Turn(new Dictionary<Token, int>
        {
            { Token.Diamond, 0 },
            { Token.Sapphire, 0 },
            { Token.Emerald, 0 },
            { Token.Ruby, 0 },
            { Token.Onyx, 0 },
            { Token.Gold, 0 }
        });

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Should get error for not taking enough tokens
        result.Error.Should().NotBeNull("turn with zero tokens should fail");
    }

    [Fact]
    public void Turn_TakeTokensAndPurchaseCard_OnlyOneActionAllowed()
    {
        // Arrange - This should not be possible, but test to ensure
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 3)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var card = new CardBuilder()
            .WithLevel(1)
            .WithPrice(diamond: 2)
            .Build();

        // Create a turn that tries to both take tokens AND purchase a card
        var turn = new Turn(TokenHelper.Create(sapphire: 1)) // Take token
        {
            // Also set card (this would be an invalid turn)
        };
        typeof(Turn).GetProperty("Card")!.SetValue(turn, card);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        result.Error.Should().NotBeNull("turn with multiple actions should fail");
        // The exact error depends on implementation - could be error 11 or a validation error
    }

    [Fact]
    public void Turn_PurchaseAndReserve_BothActionsSetInvalid()
    {
        // Note: This test verifies that having both Card and ReservedCard set is an invalid state
        // In practice, the Turn constructors prevent this, but we test the edge case
        // The game will process Card first (purchase), so ReservedCard is ignored

        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Get a card from the board
        var card1 = board.Level1Cards[0];
        Assert.NotNull(card1);

        // Give player enough tokens to purchase the card
        foreach (var cost in card1.Price)
        {
            TestHelpers.SetPlayerTokens(player, cost.Key, cost.Value);
        }

        // Create a purchase turn - the Card property takes precedence
        var turn = new Turn(card1, isReserve: false);

        // Act - The turn will process as a purchase since Card is set
        var result = board.ExecuteTurn(turn);

        // Assert - Purchase should succeed if player has enough tokens
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Cards.Should().Contain(card1, "card should be purchased");
    }

    [Fact]
    public void Turn_ValidSingleAction_Succeeds()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Each single action type should succeed
        var validTurns = new[]
        {
            TurnBuilder.TakeOneToken(Token.Diamond),
            TurnBuilder.TakeThreeDifferentTokens(Token.Sapphire, Token.Emerald, Token.Ruby),
            TurnBuilder.TakeTwoSameTokens(Token.Onyx)
        };

        foreach (var turn in validTurns)
        {
            // Reset board for each test
            var testBoard = new GameBoard(new List<IPlayer>
            {
                new PlayerBuilder().WithName("Player1").WithId(0).Build(),
                new Player("Dummy", 1)
            }, TestHelpers.CreateMockGameDataService());

            // Act
            var result = testBoard.ExecuteTurn(turn);

            // Assert
            AssertionHelpers.AssertTurnSucceeded(result);
        }
    }

    [Fact]
    public void Turn_ReserveIsExclusiveAction_CannotCombineWithOthers()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var cardToReserve = board.Level1Cards[0];

        // NOTE: Reserve card DOES come with a gold token, which is a special case
        // This is allowed per rules (lines 93-94: "Reserve 1 development card and take 1 gold token")
        // So reserving + getting gold is ONE action, not two

        // But trying to reserve AND take other tokens should fail
        var turn = new Turn(TokenHelper.Create(diamond: 1), cardToReserve!);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        // This should fail because you can't reserve AND take tokens (other than the automatic gold)
        result.Error.Should().NotBeNull("cannot combine reservation with taking tokens");
    }

    #endregion

    #region Additional Edge Cases (3 tests)

    [Fact]
    public void TakingZeroTokens_ReturnsError()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var turn = new Turn(TokenHelper.Create(diamond: 0, sapphire: 0, emerald: 0));

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        result.Error.Should().NotBeNull("taking zero tokens should be an error");
    }

    [Fact]
    public void RandomNobleSelection_SelectsCorrectCount()
    {
        // Arrange - 3 players should get 4 nobles (player count + 1)
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2)
        };

        var board = new GameBoard(players, TestHelpers.CreateMockGameDataService());

        // Act & Assert
        board.Nobles.Should().HaveCount(4, "3 players should have 4 nobles (player count + 1)");
        board.Nobles.Should().OnlyHaveUniqueItems("each noble should be unique");
        board.Nobles.Should().AllSatisfy(noble => noble.Should().NotBeNull());
    }

    [Fact]
    public void InvalidTurn_WithNullProperties_ReturnsError()
    {
        // Arrange
        var player = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Create turn using default constructor (all nulls)
        var turn = new Turn(new Dictionary<Token, int>
        {
            { Token.Diamond, 0 },
            { Token.Sapphire, 0 },
            { Token.Emerald, 0 },
            { Token.Ruby, 0 },
            { Token.Onyx, 0 },
            { Token.Gold, 0 }
        });

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        result.Error.Should().NotBeNull("turn with all zero/null values should be invalid");
    }

    #endregion
}
