using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Tests for the CardStack class.
/// </summary>
public class CardStackTests
{
    [Fact]
    public void Constructor_SetsLevelCorrectly()
    {
        // Arrange
        var cards = new List<ICard>();
        uint level = 2;

        // Act
        var cardStack = new CardStack(level, 0, cards);

        // Assert
        cardStack.Level.Should().Be(2);
    }

    [Fact]
    public void Constructor_SetsCardsList()
    {
        // Arrange
        var card1 = CardBuilder.CheapLevel1Diamond();
        var card2 = CardBuilder.ExpensiveLevel3();
        var cards = new List<ICard> { card1, card2 };

        // Act
        var cardStack = new CardStack(1, 2, cards);

        // Assert
        cardStack.Cards.Should().NotBeNull();
        cardStack.Cards.Should().HaveCount(2);
        cardStack.Cards.Should().Contain(card1);
        cardStack.Cards.Should().Contain(card2);
    }

    [Fact]
    public void Draw_ReturnsACard()
    {
        // Arrange
        var card1 = CardBuilder.CheapLevel1Diamond();
        var card2 = CardBuilder.ExpensiveLevel3();
        var cards = new List<ICard> { card1, card2 };
        var cardStack = new CardStack(1, 2, cards);

        // Act
        var drawnCard = cardStack.Draw();

        // Assert
        drawnCard.Should().NotBeNull();
        drawnCard.Should().BeOneOf(card1, card2);
    }

    [Fact]
    public void Draw_RemovesCardFromStack()
    {
        // Arrange
        var card1 = CardBuilder.CheapLevel1Diamond();
        var card2 = CardBuilder.ExpensiveLevel3();
        var card3 = CardBuilder.FreeCard();
        var cards = new List<ICard> { card1, card2, card3 };
        var cardStack = new CardStack(1, 3, cards);

        // Act
        var drawnCard = cardStack.Draw();

        // Assert
        cardStack.Cards.Should().HaveCount(2);
        cardStack.Cards.Should().NotContain(drawnCard!);
    }

    [Fact]
    public void Draw_WhenEmpty_ReturnsNull()
    {
        // Arrange
        var cards = new List<ICard>();
        var cardStack = new CardStack(1, 0, cards);

        // Act
        var drawnCard = cardStack.Draw();

        // Assert
        drawnCard.Should().BeNull();
    }

    [Fact]
    public void Draw_DecreasesCardCount()
    {
        // Arrange
        var card1 = CardBuilder.CheapLevel1Diamond();
        var card2 = CardBuilder.ExpensiveLevel3();
        var card3 = CardBuilder.FreeCard();
        var cards = new List<ICard> { card1, card2, card3 };
        var cardStack = new CardStack(1, 3, cards);

        // Act
        cardStack.Draw();
        var countAfterFirstDraw = cardStack.Cards.Count;
        cardStack.Draw();
        var countAfterSecondDraw = cardStack.Cards.Count;

        // Assert
        countAfterFirstDraw.Should().Be(2);
        countAfterSecondDraw.Should().Be(1);
    }

    [Fact]
    public void Cards_ReturnsRemainingCards()
    {
        // Arrange
        var card1 = CardBuilder.CheapLevel1Diamond();
        var card2 = CardBuilder.ExpensiveLevel3();
        var card3 = CardBuilder.FreeCard();
        var cards = new List<ICard> { card1, card2, card3 };
        var cardStack = new CardStack(1, 3, cards);

        // Act
        var drawnCard = cardStack.Draw();
        var remainingCards = cardStack.Cards;

        // Assert
        remainingCards.Should().HaveCount(2);
        remainingCards.Should().NotContain(drawnCard!);
    }

    [Fact]
    public void Draw_Randomness_DrawsDifferentCardsOverMultipleDraws()
    {
        // Arrange - Create multiple card stacks and track which cards get drawn first
        var drawnCards = new Dictionary<string, int>();
        const int iterations = 100;

        for (int i = 0; i < iterations; i++)
        {
            var card1 = new CardBuilder()
                .WithLevel(1)
                .WithType(Token.Diamond)
                .WithImageName("card1.jpg")
                .Build();
            var card2 = new CardBuilder()
                .WithLevel(1)
                .WithType(Token.Sapphire)
                .WithImageName("card2.jpg")
                .Build();
            var card3 = new CardBuilder()
                .WithLevel(1)
                .WithType(Token.Emerald)
                .WithImageName("card3.jpg")
                .Build();

            var cards = new List<ICard> { card1, card2, card3 };
            var cardStack = new CardStack(1, 3, cards);

            // Act
            var drawnCard = cardStack.Draw();

            // Track which card was drawn
            var imageName = drawnCard!.ImageName;
            if (!drawnCards.ContainsKey(imageName))
            {
                drawnCards[imageName] = 0;
            }
            drawnCards[imageName]++;
        }

        // Assert - All three cards should have been drawn at least once
        // Statistical assertion: with 100 iterations and 3 cards, it's extremely unlikely
        // that any single card would never be drawn (probability < 0.000001%)
        drawnCards.Should().HaveCount(3);
        drawnCards.Should().ContainKey("card1.jpg");
        drawnCards.Should().ContainKey("card2.jpg");
        drawnCards.Should().ContainKey("card3.jpg");

        // Each card should be drawn at least a few times (allowing for randomness)
        // With 100 draws and 3 cards, we expect ~33 each, but allow for variation
        drawnCards["card1.jpg"].Should().BeGreaterThan(5);
        drawnCards["card2.jpg"].Should().BeGreaterThan(5);
        drawnCards["card3.jpg"].Should().BeGreaterThan(5);
    }

    [Fact]
    public void Draw_MultipleDraws_EventuallyEmptiesStack()
    {
        // Arrange
        var card1 = CardBuilder.CheapLevel1Diamond();
        var card2 = CardBuilder.ExpensiveLevel3();
        var cards = new List<ICard> { card1, card2 };
        var cardStack = new CardStack(1, 2, cards);

        // Act
        var draw1 = cardStack.Draw();
        var draw2 = cardStack.Draw();
        var draw3 = cardStack.Draw();

        // Assert
        draw1.Should().NotBeNull();
        draw2.Should().NotBeNull();
        draw3.Should().BeNull();
        cardStack.Cards.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithEmptyList_CreatesValidStack()
    {
        // Arrange & Act
        var cardStack = new CardStack(3, 0, new List<ICard>());

        // Assert
        cardStack.Level.Should().Be(3);
        cardStack.Cards.Should().BeEmpty();
    }
}
