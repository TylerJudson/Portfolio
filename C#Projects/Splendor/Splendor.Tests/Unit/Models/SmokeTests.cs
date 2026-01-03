using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Splendor.Tests.TestUtilities.Fakes;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Smoke tests to verify the test infrastructure is working correctly.
/// </summary>
public class SmokeTests
{
    [Fact]
    public void TokenHelper_CreatesCorrectDictionary()
    {
        // Arrange & Act
        var tokens = TokenHelper.Create(diamond: 1, sapphire: 2, emerald: 3);

        // Assert
        tokens[Token.Diamond].Should().Be(1);
        tokens[Token.Sapphire].Should().Be(2);
        tokens[Token.Emerald].Should().Be(3);
        tokens[Token.Ruby].Should().Be(0);
    }

    [Fact]
    public void CardBuilder_CreatesCard()
    {
        // Arrange & Act
        var card = new CardBuilder()
            .WithLevel(2)
            .WithType(Token.Sapphire)
            .WithPrestigePoints(3)
            .WithPrice(diamond: 2, ruby: 3)
            .Build();

        // Assert
        card.Level.Should().Be(2);
        card.Type.Should().Be(Token.Sapphire);
        card.PrestigePoints.Should().Be(3);
        card.Price[Token.Diamond].Should().Be(2);
        card.Price[Token.Ruby].Should().Be(3);
    }

    [Fact]
    public void PlayerBuilder_CreatesPlayerWithTokens()
    {
        // Arrange & Act
        var player = new PlayerBuilder()
            .WithName("TestPlayer")
            .WithId(1)
            .WithTokens(diamond: 3, sapphire: 2)
            .Build();

        // Assert
        player.Name.Should().Be("TestPlayer");
        player.Id.Should().Be(1);
        player.Tokens[Token.Diamond].Should().Be(3);
        player.Tokens[Token.Sapphire].Should().Be(2);
    }

    [Fact]
    public void NobleBuilder_CreatesNobleWithCriteria()
    {
        // Arrange & Act
        var noble = new NobleBuilder()
            .WithCriteria(diamond: 3, sapphire: 3, emerald: 3)
            .Build();

        // Assert
        noble.Criteria[Token.Diamond].Should().Be(3);
        noble.Criteria[Token.Sapphire].Should().Be(3);
        noble.Criteria[Token.Emerald].Should().Be(3);
        noble.PrestigePoints.Should().Be(3);
    }

    [Fact]
    public void FakeCardStack_DrawsCardsInOrder()
    {
        // Arrange
        var card1 = CardBuilder.CheapLevel1Diamond();
        var card2 = CardBuilder.ExpensiveLevel3();
        var stack = new FakeCardStack(1, card1, card2);

        // Act
        var drawn1 = stack.Draw();
        var drawn2 = stack.Draw();
        var drawn3 = stack.Draw();

        // Assert
        drawn1.Should().BeSameAs(card1);
        drawn2.Should().BeSameAs(card2);
        drawn3.Should().BeNull();
    }

    [Fact]
    public void GameBoard_CanBeCreated()
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
        board.Should().NotBeNull();
        board.Players.Should().HaveCount(2);
        board.TokenStacks[Token.Diamond].Should().Be(4); // 2 players = 4 gems each
        board.Nobles.Should().HaveCount(3); // 2 players + 1 = 3 nobles
    }

    [Fact]
    public void TurnBuilder_CreatesTakeTurn()
    {
        // Arrange & Act
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Assert
        turn.TakenTokens.Should().NotBeNull();
        turn.TakenTokens![Token.Diamond].Should().Be(1);
        turn.TakenTokens[Token.Sapphire].Should().Be(1);
        turn.TakenTokens[Token.Emerald].Should().Be(1);
    }

    [Fact]
    public void AssertionHelpers_WorkCorrectly()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 3, emerald: 1, ruby: 0, onyx: 0, gold: 1)
            .Build();

        // Act & Assert (should not throw)
        AssertionHelpers.AssertPlayerTokens(player, 2, 3, 1, 0, 0, 1);
    }
}
