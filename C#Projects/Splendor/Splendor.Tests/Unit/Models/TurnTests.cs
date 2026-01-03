using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Unit tests for the Turn class.
/// </summary>
public class TurnTests
{
    [Fact]
    public void Constructor_WithTakenTokens_SetsTakenTokens()
    {
        // Arrange
        var tokens = TokenHelper.Create(diamond: 1, sapphire: 1, emerald: 1);

        // Act
        var turn = new Turn(tokens);

        // Assert
        turn.TakenTokens.Should().BeSameAs(tokens);
        turn.TakenTokens![Token.Diamond].Should().Be(1);
        turn.TakenTokens[Token.Sapphire].Should().Be(1);
        turn.TakenTokens[Token.Emerald].Should().Be(1);
        turn.Card.Should().BeNull();
        turn.ReservedCard.Should().BeNull();
        turn.Noble.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithCard_SetsCardProperty()
    {
        // Arrange
        var card = CardBuilder.CheapLevel1Diamond();

        // Act
        var turn = new Turn(card, isReserve: false);

        // Assert
        turn.Card.Should().BeSameAs(card);
        turn.ReservedCard.Should().BeNull();
        turn.TakenTokens.Should().BeNull();
        turn.Noble.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithReservedCard_SetsReservedCard()
    {
        // Arrange
        var card = CardBuilder.CheapLevel1Diamond();

        // Act
        var turn = new Turn(card, isReserve: true);

        // Assert
        turn.ReservedCard.Should().BeSameAs(card);
        turn.Card.Should().BeNull();
        turn.TakenTokens.Should().BeNull();
        turn.Noble.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithNoble_SetsNobleProperty()
    {
        // Arrange
        var noble = NobleBuilder.ThreeOfThree();

        // Act
        var turn = new Turn(noble);

        // Assert
        turn.Noble.Should().BeSameAs(noble);
        turn.Card.Should().BeNull();
        turn.ReservedCard.Should().BeNull();
        turn.TakenTokens.Should().BeNull();
    }

    [Fact]
    public void Constructor_SetsTimeStampToUtcNow()
    {
        // Arrange
        var beforeCreate = DateTime.UtcNow;

        // Act
        var turn = new Turn(TokenHelper.Create(diamond: 1));
        var afterCreate = DateTime.UtcNow;

        // Assert
        turn.TimeStamp.Should().BeOnOrAfter(beforeCreate);
        turn.TimeStamp.Should().BeOnOrBefore(afterCreate);
        turn.TimeStamp.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void PlayerName_CanBeSet()
    {
        // Arrange
        var turn = new Turn(TokenHelper.Create(diamond: 1));
        var playerName = "TestPlayer";

        // Act
        turn.PlayerName = playerName;

        // Assert
        turn.PlayerName.Should().Be(playerName);
    }

    [Fact]
    public void Constructor_WithTakenTokensAndReservedCard_SetsBothProperties()
    {
        // Arrange
        var tokens = TokenHelper.Create(gold: 1);
        var card = CardBuilder.CheapLevel1Diamond();

        // Act
        var turn = new Turn(tokens, card);

        // Assert
        turn.TakenTokens.Should().BeSameAs(tokens);
        turn.ReservedCard.Should().BeSameAs(card);
        turn.Card.Should().BeNull();
        turn.Noble.Should().BeNull();
    }
}
