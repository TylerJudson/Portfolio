using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Tests for GameBoard - Part 1: Initialization and Token Taking Logic
/// Covers game initialization across different player counts and token-taking scenarios.
/// </summary>
public class GameBoardTests_Part1
{
    #region Initialization Tests (16 tests)

    [Fact]
    public void Constructor_TwoPlayers_InitializesTokenStacksWith4GemsEach()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        AssertionHelpers.AssertTokenStacks(board,
            diamond: 4,
            sapphire: 4,
            emerald: 4,
            ruby: 4,
            onyx: 4,
            gold: 5);
    }

    [Fact]
    public void Constructor_TwoPlayers_InitializesWithAlways5Gold()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.TokenStacks[Token.Gold].Should().Be(5, "gold tokens should always be 5 regardless of player count");
    }

    [Fact]
    public void Constructor_ThreePlayers_InitializesTokenStacksWith5GemsEach()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        AssertionHelpers.AssertTokenStacks(board,
            diamond: 5,
            sapphire: 5,
            emerald: 5,
            ruby: 5,
            onyx: 5,
            gold: 5);
    }

    [Fact]
    public void Constructor_FourPlayers_InitializesTokenStacksWith7GemsEach()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2),
            new Player("Player4", 3)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        AssertionHelpers.AssertTokenStacks(board,
            diamond: 7,
            sapphire: 7,
            emerald: 7,
            ruby: 7,
            onyx: 7,
            gold: 5);
    }

    [Fact]
    public void Constructor_FivePlayers_InitializesTokenStacksWith7GemsEach()
    {
        // Arrange - Testing 4+ players should use same token counts
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2),
            new Player("Player4", 3),
            new Player("Player5", 4)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        AssertionHelpers.AssertTokenStacks(board,
            diamond: 7,
            sapphire: 7,
            emerald: 7,
            ruby: 7,
            onyx: 7,
            gold: 5);
    }

    [Fact]
    public void Constructor_TwoPlayers_Initializes3Nobles()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.Nobles.Should().HaveCount(3, "2 players should have 3 nobles (player count + 1)");
    }

    [Fact]
    public void Constructor_ThreePlayers_Initializes4Nobles()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.Nobles.Should().HaveCount(4, "3 players should have 4 nobles (player count + 1)");
    }

    [Fact]
    public void Constructor_FourPlayers_Initializes5Nobles()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2),
            new Player("Player4", 3)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.Nobles.Should().HaveCount(5, "4 players should have 5 nobles (player count + 1)");
    }

    [Fact]
    public void Constructor_InitializesLevel1CardsWith36CardsRemaining()
    {
        // Arrange - Level 1 has 40 cards, 4 are drawn for display
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.CardStackLevel1.Cards.Count.Should().Be(36, "40 level 1 cards - 4 displayed = 36 remaining");
    }

    [Fact]
    public void Constructor_InitializesLevel2CardsWith26CardsRemaining()
    {
        // Arrange - Level 2 has 30 cards, 4 are drawn for display
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.CardStackLevel2.Cards.Count.Should().Be(26, "30 level 2 cards - 4 displayed = 26 remaining");
    }

    [Fact]
    public void Constructor_InitializesLevel3CardsWith16CardsRemaining()
    {
        // Arrange - Level 3 has 20 cards, 4 are drawn for display
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.CardStackLevel3.Cards.Count.Should().Be(16, "20 level 3 cards - 4 displayed = 16 remaining");
    }

    [Fact]
    public void Constructor_Initializes4VisibleCardsPerLevel()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.Level1Cards.Should().HaveCount(4, "level 1 should have 4 visible cards");
        board.Level2Cards.Should().HaveCount(4, "level 2 should have 4 visible cards");
        board.Level3Cards.Should().HaveCount(4, "level 3 should have 4 visible cards");

        // All visible cards should be non-null initially
        board.Level1Cards.Should().AllSatisfy(card => card.Should().NotBeNull());
        board.Level2Cards.Should().AllSatisfy(card => card.Should().NotBeNull());
        board.Level3Cards.Should().AllSatisfy(card => card.Should().NotBeNull());
    }

    [Fact]
    public void Constructor_InitializesVersionToZero()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.Version.Should().Be(0, "game should start at version 0");
    }

    [Fact]
    public void Constructor_InitializesCurrentPlayerToZero()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.CurrentPlayer.Should().Be(0, "first player (index 0) should start");
    }

    [Fact]
    public void Constructor_InitializesGameOverToFalse()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.GameOver.Should().BeFalse("game should not be over at start");
    }

    [Fact]
    public void Constructor_InitializesLastRoundToFalse()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };

        // Act
        var board = new GameBoard(players);

        // Assert
        board.LastRound.Should().BeFalse("last round should not be triggered at start");
    }

    #endregion

    #region Token Taking - Valid Scenarios (8 tests)

    [Fact]
    public void ExecuteTurn_TakeThreeDifferentTokens_SucceedsAndSubtractsFromStacks()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);

        // Tokens should be subtracted from stacks (started at 4 each for 2 players)
        board.TokenStacks[Token.Diamond].Should().Be(3, "one diamond should be taken");
        board.TokenStacks[Token.Sapphire].Should().Be(3, "one sapphire should be taken");
        board.TokenStacks[Token.Emerald].Should().Be(3, "one emerald should be taken");
        board.TokenStacks[Token.Ruby].Should().Be(4, "ruby stack should be unchanged");
        board.TokenStacks[Token.Onyx].Should().Be(4, "onyx stack should be unchanged");
    }

    [Fact]
    public void ExecuteTurn_TakeThreeDifferentTokens_AddsTokensToPlayer()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Ruby, Token.Onyx, Token.Emerald);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        AssertionHelpers.AssertPlayerTokens(board.Players[0],
            diamond: 0,
            sapphire: 0,
            emerald: 1,
            ruby: 1,
            onyx: 1,
            gold: 0);
    }

    [Fact]
    public void ExecuteTurn_TakeTwoSameTokens_SucceedsWhen4Available()
    {
        // Arrange - 2 players means 4 tokens of each gem type
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeTwoSameTokens(Token.Diamond);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        board.TokenStacks[Token.Diamond].Should().Be(2, "two diamonds should be taken, leaving 2");
        AssertionHelpers.AssertPlayerTokens(board.Players[0],
            diamond: 2,
            sapphire: 0,
            emerald: 0,
            ruby: 0,
            onyx: 0,
            gold: 0);
    }

    [Fact]
    public void ExecuteTurn_TakeOneToken_SucceedsAndUpdatesState()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeOneToken(Token.Sapphire);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        board.TokenStacks[Token.Sapphire].Should().Be(3, "one sapphire should be taken");
        board.Players[0].Tokens[Token.Sapphire].Should().Be(1, "player should have received 1 sapphire");
    }

    [Fact]
    public void ExecuteTurn_TakeTwoDifferentTokens_SucceedsAndUpdatesState()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        // Use a dictionary with only the tokens we want to take (excluding gold)
        var turn = new Turn(new Dictionary<Token, int>
        {
            { Token.Diamond, 1 },
            { Token.Ruby, 1 }
        });

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        board.TokenStacks[Token.Diamond].Should().Be(3);
        board.TokenStacks[Token.Ruby].Should().Be(3);
        AssertionHelpers.AssertPlayerTokens(board.Players[0],
            diamond: 1,
            sapphire: 0,
            emerald: 0,
            ruby: 1,
            onyx: 0,
            gold: 0);
    }

    [Fact]
    public void ExecuteTurn_ValidTokenTake_IncrementsVersion()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        var initialVersion = board.Version;
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        board.ExecuteTurn(turn);

        // Assert
        board.Version.Should().Be(initialVersion + 1, "version should increment after successful turn");
    }

    [Fact]
    public void ExecuteTurn_ValidTokenTake_AdvancesCurrentPlayer()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeOneToken(Token.Diamond);

        // Act
        board.ExecuteTurn(turn);

        // Assert
        board.CurrentPlayer.Should().Be(1, "current player should advance to next player");
    }

    [Fact]
    public void ExecuteTurn_ValidTokenTake_WrapsCurrentPlayerToZero()
    {
        // Arrange - Start with last player
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);

        // Execute turn for player 0 to advance to player 1
        board.ExecuteTurn(TurnBuilder.TakeOneToken(Token.Diamond));

        board.CurrentPlayer.Should().Be(1, "sanity check: current player should be 1");

        var turn = TurnBuilder.TakeOneToken(Token.Sapphire);

        // Act - Player 1 takes turn
        board.ExecuteTurn(turn);

        // Assert - Should wrap back to player 0
        board.CurrentPlayer.Should().Be(0, "current player should wrap back to 0 after last player's turn");
    }

    #endregion

    #region Token Taking - Invalid Scenarios (6 tests)

    [Fact]
    public void ExecuteTurn_TakeFourDifferentTokenTypes_FailsWithError4()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeFourDifferentTokens();

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 4);
    }

    [Fact]
    public void ExecuteTurn_TakeGoldDirectly_FailsWithError6()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeGold();

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 6);
    }

    [Fact]
    public void ExecuteTurn_TakeMoreThanTwoOfSameToken_FailsWithError0()
    {
        // Arrange
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1),
            new Player("Player3", 2),
            new Player("Player4", 3)
        };
        var board = new GameBoard(players);
        var turn = TurnBuilder.TakeThreeSameTokens(Token.Diamond);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 0);
        result.Error!.Message.Should().Contain("can't take more than 2 tokens of the same token type");
    }

    [Fact]
    public void ExecuteTurn_TakeTwoSameTokensWhenLessThan4Available_FailsWithError0()
    {
        // Arrange - 2 players means only 4 gems each, take 2 to leave only 2
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);

        // First player takes 1 diamond, leaving 3 in the stack
        board.ExecuteTurn(TurnBuilder.TakeOneToken(Token.Diamond));

        // Now player 2 tries to take 2 diamonds (but only 3 remain, need at least 4)
        var turn = TurnBuilder.TakeTwoSameTokens(Token.Diamond);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 0);
        result.Error!.Message.Should().Contain("must leave at least 2 tokens in the stack");
    }

    [Fact]
    public void ExecuteTurn_TakeMoreTokensThanAvailable_FailsWithError0()
    {
        // Arrange - Manually set up a scenario with only 1 token in a stack
        // Then try to take 2 different tokens where one doesn't exist
        var players = new List<IPlayer>
        {
            new Player("Player1", 0),
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);

        // With 2 players, we start with 4 of each gem
        // Take tokens to deplete the diamond stack
        board.ExecuteTurn(TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Emerald)); // 4->3 diamonds
        board.ExecuteTurn(TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Ruby, Token.Onyx)); // 3->2 diamonds
        board.ExecuteTurn(TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Ruby)); // 2->1 diamond
        board.ExecuteTurn(TurnBuilder.TakeOneToken(Token.Sapphire)); // Skip player 2, diamonds still 1
        board.ExecuteTurn(TurnBuilder.TakeOneToken(Token.Diamond)); // 1->0 diamonds

        // Verify diamond stack is empty
        board.TokenStacks[Token.Diamond].Should().Be(0, "sanity check: diamond stack should be depleted");

        // Now player 2 tries to take a diamond (but there are 0 available)
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Ruby);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 0);
        result.Error!.Message.Should().Contain("Not enough tokens to take");
    }

    [Fact]
    public void ExecuteTurn_TakeTokensExceedingPlayerLimit_ReturnsContinueAction0()
    {
        // Arrange - Player already has 10 tokens (the max)
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .AtMaxTokens() // Sets player to have exactly 10 tokens
            .Build();

        var players = new List<IPlayer>
        {
            player1,
            new Player("Player2", 1)
        };
        var board = new GameBoard(players);

        // Player with 10 tokens tries to take more
        var turn = TurnBuilder.TakeOneToken(Token.Diamond);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnRequiresContinueAction(result, 0);
        result.ContinueAction!.Message.Should().Contain("get rid of", "player should be prompted to return tokens");
    }

    #endregion
}
