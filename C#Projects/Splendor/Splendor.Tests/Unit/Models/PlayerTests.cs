using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Comprehensive tests for the Player class covering all methods and logic.
/// </summary>
public class PlayerTests
{
    #region Initialization Tests (6 tests)

    [Fact]
    public void Constructor_InitializesEmptyTokens()
    {
        // Arrange & Act
        var player = new Player("TestPlayer", 1);

        // Assert
        player.Tokens.Should().NotBeNull();
        player.Tokens[Token.Diamond].Should().Be(0);
        player.Tokens[Token.Sapphire].Should().Be(0);
        player.Tokens[Token.Emerald].Should().Be(0);
        player.Tokens[Token.Ruby].Should().Be(0);
        player.Tokens[Token.Onyx].Should().Be(0);
        player.Tokens[Token.Gold].Should().Be(0);
    }

    [Fact]
    public void Constructor_InitializesEmptyCardsList()
    {
        // Arrange & Act
        var player = new Player("TestPlayer", 1);

        // Assert
        player.Cards.Should().NotBeNull();
        player.Cards.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_InitializesEmptyReservedCardsList()
    {
        // Arrange & Act
        var player = new Player("TestPlayer", 1);

        // Assert
        player.ReservedCards.Should().NotBeNull();
        player.ReservedCards.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_InitializesZeroPrestigePoints()
    {
        // Arrange & Act
        var player = new Player("TestPlayer", 1);

        // Assert
        player.PrestigePoints.Should().Be(0);
    }

    [Fact]
    public void Constructor_SetsNameCorrectly()
    {
        // Arrange & Act
        var player = new Player("Alice", 1);

        // Assert
        player.Name.Should().Be("Alice");
    }

    [Fact]
    public void Constructor_SetsIdCorrectly()
    {
        // Arrange & Act
        var player = new Player("Bob", 42);

        // Assert
        player.Id.Should().Be(42);
    }

    #endregion

    #region Token Operations Tests (6 tests)

    [Fact]
    public void ExecuteTurn_TakeTokens_AddsTokensToPlayer()
    {
        // Arrange
        var player = new Player("TestPlayer", 1);
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Diamond, Token.Sapphire, Token.Emerald);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        AssertionHelpers.AssertPlayerTokens(player, 1, 1, 1, 0, 0, 0);
    }

    [Fact]
    public void ExecuteTurn_TakeTokensExceedingLimit_ReturnsContinueAction()
    {
        // Arrange - player has 9 tokens
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 1, gold: 0)
            .Build();
        var turn = TurnBuilder.TakeTwoSameTokens(Token.Diamond); // Would bring total to 11

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnRequiresContinueAction(result, 0);
        // Tokens should not be added yet when exceeding limit
        player.NumberOfTokens().Should().Be(9);
    }

    [Fact]
    public void ExecuteTurn_ReturnTokens_SubtractsFromPlayer()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 3, sapphire: 2, emerald: 1, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var tokensToReturn = TokenHelper.Create(diamond: 2, sapphire: 1);
        var turn = TurnBuilder.ReturnTokens(tokensToReturn);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        AssertionHelpers.AssertPlayerTokens(player, 1, 1, 1, 0, 0, 0);
    }

    [Fact]
    public void ExecuteTurn_ReturnMoreThanPlayerHas_ReturnsError()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var tokensToReturn = TokenHelper.Create(diamond: 3); // Player only has 1
        var turn = TurnBuilder.ReturnTokens(tokensToReturn);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 7);
    }

    [Fact]
    public void NumberOfTokens_ReturnsCorrectCount()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 3, emerald: 1, ruby: 0, onyx: 0, gold: 1)
            .Build();

        // Act
        var count = player.NumberOfTokens();

        // Assert
        count.Should().Be(7);
    }

    [Fact]
    public void NumberOfTokens_IncludesGoldTokens()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 0, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 3)
            .Build();

        // Act
        var count = player.NumberOfTokens();

        // Assert
        count.Should().Be(3);
    }

    #endregion

    #region Card Purchase Tests (7 tests)

    [Fact]
    public void ExecuteTurn_PurchaseCard_AddsCardToCardsList()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 1, emerald: 1, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithType(Token.Diamond)
            .WithPrice(sapphire: 1, emerald: 1)
            .WithPrestigePoints(1)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Cards.Should().Contain(card);
        player.Cards.Should().HaveCount(1);
    }

    [Fact]
    public void ExecuteTurn_PurchaseCard_IncrementsCardTokens()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 0, sapphire: 2, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithType(Token.Sapphire)
            .WithPrice(sapphire: 2)
            .WithPrestigePoints(0)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.CardTokens[Token.Sapphire].Should().Be(1);
        player.CardTokens[Token.Diamond].Should().Be(0);
    }

    [Fact]
    public void ExecuteTurn_PurchaseCard_AddsPrestigePoints()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 3, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithType(Token.Diamond)
            .WithPrice(diamond: 2, sapphire: 3)
            .WithPrestigePoints(2)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.PrestigePoints.Should().Be(2);
    }

    [Fact]
    public void ExecuteTurn_PurchaseCard_SubtractsTokens()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 3, sapphire: 2, emerald: 1, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithType(Token.Ruby)
            .WithPrice(diamond: 2, sapphire: 1)
            .WithPrestigePoints(1)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        AssertionHelpers.AssertPlayerTokens(player, 1, 1, 1, 0, 0, 0);
    }

    [Fact]
    public void ExecuteTurn_PurchaseCard_WithCardBonuses_ReducesCost()
    {
        // Arrange - player has 2 diamond cards (bonuses) and some tokens
        var diamondCard1 = new CardBuilder().WithType(Token.Diamond).WithPrestigePoints(0).Build();
        var diamondCard2 = new CardBuilder().WithType(Token.Diamond).WithPrestigePoints(0).Build();
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .WithCards(diamondCard1, diamondCard2)
            .Build();

        // Card costs 3 diamonds, but player has 2 diamond bonuses, so only needs 1 diamond token
        var card = new CardBuilder()
            .WithType(Token.Sapphire)
            .WithPrice(diamond: 3)
            .WithPrestigePoints(1)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Tokens[Token.Diamond].Should().Be(0); // Used 1 token
        player.Cards.Should().HaveCount(3); // 2 original + new card
    }

    [Fact]
    public void ExecuteTurn_PurchaseCard_WithGoldTokens_UsesGoldAsWildcard()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 2)
            .Build();
        var card = new CardBuilder()
            .WithType(Token.Diamond)
            .WithPrice(diamond: 3) // Need 3 diamonds, have 1 + 2 gold
            .WithPrestigePoints(1)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Tokens[Token.Diamond].Should().Be(0); // Used all diamond tokens
        player.Tokens[Token.Gold].Should().Be(0); // Used 2 gold as wildcard
        player.Cards.Should().Contain(card);
    }

    [Fact]
    public void ExecuteTurn_PurchaseCard_InsufficientTokens_ReturnsError()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithType(Token.Diamond)
            .WithPrice(diamond: 3, sapphire: 2) // Not enough tokens
            .WithPrestigePoints(1)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 2);
        player.Cards.Should().BeEmpty();
    }

    #endregion

    #region CanPurchaseCard Method Tests (6 tests)

    [Fact]
    public void CanPurchaseCard_WithExactTokens_ReturnsTrue()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 3, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithPrice(diamond: 2, sapphire: 3)
            .Build();

        // Act
        var result = player.CanPurchaseCard(card);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPurchaseCard_WithExtraTokens_ReturnsTrue()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 5, sapphire: 4, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithPrice(diamond: 2, sapphire: 3)
            .Build();

        // Act
        var result = player.CanPurchaseCard(card);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPurchaseCard_WithCardBonusesReducingCost_ReturnsTrue()
    {
        // Arrange - player has 2 diamond card bonuses
        var diamondCard1 = new CardBuilder().WithType(Token.Diamond).Build();
        var diamondCard2 = new CardBuilder().WithType(Token.Diamond).Build();
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .WithCards(diamondCard1, diamondCard2)
            .Build();

        // Card needs 3 diamonds, but player has 2 bonuses + 1 token = 3 total
        var card = new CardBuilder()
            .WithPrice(diamond: 3)
            .Build();

        // Act
        var result = player.CanPurchaseCard(card);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPurchaseCard_WithInsufficientTokens_ReturnsFalse()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 1, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithPrice(diamond: 2, sapphire: 3)
            .Build();

        // Act
        var result = player.CanPurchaseCard(card);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanPurchaseCard_NullCard_ReturnsFalse()
    {
        // Arrange
        var player = new PlayerBuilder().Build();

        // Act
        var result = player.CanPurchaseCard(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanPurchaseCard_FreeCard_ReturnsTrue()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 0, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = CardBuilder.FreeCard();

        // Act
        var result = player.CanPurchaseCard(card);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region CanPurchaseCardWithGold Method Tests (4 tests)

    [Fact]
    public void CanPurchaseCardWithGold_UsingGoldAsWildcard_ReturnsTrue()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 2)
            .Build();
        var card = new CardBuilder()
            .WithPrice(diamond: 3) // Need 3 diamonds, have 1 + 2 gold
            .Build();

        // Act
        var result = player.CanPurchaseCardWithGold(card);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPurchaseCardWithGold_NotEnoughEvenWithGold_ReturnsFalse()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 1, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 1)
            .Build();
        var card = new CardBuilder()
            .WithPrice(diamond: 5) // Need 5 diamonds, only have 1 + 1 gold = 2 total
            .Build();

        // Act
        var result = player.CanPurchaseCardWithGold(card);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanPurchaseCardWithGold_NoGoldNeeded_ReturnsTrue()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 3, sapphire: 2, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .Build();
        var card = new CardBuilder()
            .WithPrice(diamond: 2, sapphire: 1)
            .Build();

        // Act
        var result = player.CanPurchaseCardWithGold(card);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPurchaseCardWithGold_NullCard_ReturnsFalse()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 0, sapphire: 0, emerald: 0, ruby: 0, onyx: 0, gold: 5)
            .Build();

        // Act
        var result = player.CanPurchaseCardWithGold(null!);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Card Reservation Tests (4 tests)

    [Fact]
    public void ExecuteTurn_ReserveCard_AddsCardToReservedCards()
    {
        // Arrange
        var player = new Player("TestPlayer", 1);
        var card = CardBuilder.CheapLevel1Diamond();
        var turn = TurnBuilder.ReserveCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: true);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.ReservedCards.Should().Contain(card);
        player.ReservedCards.Should().HaveCount(1);
    }

    [Fact]
    public void ExecuteTurn_ReserveCard_AddsGoldToken_WhenCanGetGoldIsTrue()
    {
        // Arrange
        var player = new Player("TestPlayer", 1);
        var card = CardBuilder.CheapLevel1Diamond();
        var turn = TurnBuilder.ReserveCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: true);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Tokens[Token.Gold].Should().Be(1);
    }

    [Fact]
    public void ExecuteTurn_ReserveCard_ThreeAlreadyReserved_ReturnsError()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithMaxReservedCards() // 3 reserved cards
            .Build();
        var card = CardBuilder.CheapLevel1Diamond();
        var turn = TurnBuilder.ReserveCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: true);

        // Assert
        AssertionHelpers.AssertTurnFailedWithError(result, 3);
        player.ReservedCards.Should().HaveCount(3); // No new card added
    }

    [Fact]
    public void ExecuteTurn_ReserveCard_CausingTokenOverflow_ReturnsContinueAction()
    {
        // Arrange - player has 10 tokens (at max)
        var player = new PlayerBuilder()
            .AtMaxTokens() // 10 tokens
            .Build();
        var card = CardBuilder.CheapLevel1Diamond();
        var turn = TurnBuilder.ReserveCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: true);

        // Assert
        AssertionHelpers.AssertTurnRequiresContinueAction(result, 2);
        // Card should NOT be added when token limit would be exceeded
        player.ReservedCards.Should().BeEmpty();
    }

    #endregion

    #region Noble Acquisition Tests (4 tests)

    [Fact]
    public void CanAcquireNoble_WithExactCards_ReturnsTrue()
    {
        // Arrange - player has exactly 3 diamonds, 3 sapphires, 3 emeralds
        var player = new PlayerBuilder()
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
        var noble = NobleBuilder.ThreeOfThree();

        // Act
        var result = player.CanAcquireNoble(noble);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAcquireNoble_WithExtraCards_ReturnsTrue()
    {
        // Arrange - player has more than required
        var player = new PlayerBuilder()
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(), // Extra
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build() // Extra
            )
            .Build();
        var noble = NobleBuilder.FourOfTwo(); // Requires 4 diamond, 4 onyx

        // Act
        var result = player.CanAcquireNoble(noble);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAcquireNoble_WithInsufficientCards_ReturnsFalse()
    {
        // Arrange - player has only 2 of each when 3 are required
        var player = new PlayerBuilder()
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build()
            )
            .Build();
        var noble = NobleBuilder.ThreeOfThree(); // Requires 3 of each

        // Act
        var result = player.CanAcquireNoble(noble);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ExecuteTurn_AcquireNoble_AddsThreePrestigePoints()
    {
        // Arrange
        var player = new PlayerBuilder()
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
        var noble = NobleBuilder.ThreeOfThree();
        var turn = TurnBuilder.AcquireNoble(noble);

        // Act
        var initialPrestige = player.PrestigePoints;
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.PrestigePoints.Should().Be(initialPrestige + 3);
        player.Nobles.Should().Contain(noble);
    }

    #endregion

    #region Integration Tests - Complex Scenarios

    [Fact]
    public void ExecuteTurn_PurchaseMultipleCards_AccumulatesPrestigeAndBonuses()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5, gold: 0)
            .Build();

        var card1 = new CardBuilder()
            .WithType(Token.Diamond)
            .WithPrice(sapphire: 2)
            .WithPrestigePoints(1)
            .Build();
        var card2 = new CardBuilder()
            .WithType(Token.Sapphire)
            .WithPrice(diamond: 2)
            .WithPrestigePoints(2)
            .Build();

        // Act - purchase first card
        var turn1 = TurnBuilder.PurchaseCard(card1);
        var result1 = player.ExecuteTurn(turn1, canGetGold: false);

        // Purchase second card
        var turn2 = TurnBuilder.PurchaseCard(card2);
        var result2 = player.ExecuteTurn(turn2, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result1);
        AssertionHelpers.AssertTurnSucceeded(result2);
        player.PrestigePoints.Should().Be(3); // 1 + 2
        player.CardTokens[Token.Diamond].Should().Be(1);
        player.CardTokens[Token.Sapphire].Should().Be(1);
        player.Cards.Should().HaveCount(2);
    }

    [Fact]
    public void ExecuteTurn_ReserveCard_WithoutGoldAvailable_DoesNotAddGold()
    {
        // Arrange
        var player = new Player("TestPlayer", 1);
        var card = CardBuilder.CheapLevel1Diamond();
        var turn = TurnBuilder.ReserveCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.ReservedCards.Should().Contain(card);
        player.Tokens[Token.Gold].Should().Be(0); // No gold added
    }

    [Fact]
    public void ExecuteTurn_PurchaseCardWithOnlyCardBonuses_NoTokensSpent()
    {
        // Arrange - player has 3 diamond card bonuses
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 2, emerald: 0, ruby: 0, onyx: 0, gold: 0)
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build()
            )
            .Build();

        // Card costs exactly 3 diamonds - can pay with bonuses only
        var card = new CardBuilder()
            .WithType(Token.Sapphire)
            .WithPrice(diamond: 3)
            .WithPrestigePoints(2)
            .Build();
        var turn = TurnBuilder.PurchaseCard(card);

        // Act
        var result = player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Tokens[Token.Diamond].Should().Be(2); // No tokens spent
        player.Cards.Should().HaveCount(4);
    }

    [Fact]
    public void NumberOfTokens_AfterMultipleOperations_ReturnsCorrectTotal()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 0, onyx: 0, gold: 0)
            .Build();

        // Act - take more tokens
        var turn = TurnBuilder.TakeThreeDifferentTokens(Token.Ruby, Token.Onyx, Token.Emerald);
        player.ExecuteTurn(turn, canGetGold: false);

        // Assert
        player.NumberOfTokens().Should().Be(9); // 6 original + 3 new
    }

    #endregion
}
