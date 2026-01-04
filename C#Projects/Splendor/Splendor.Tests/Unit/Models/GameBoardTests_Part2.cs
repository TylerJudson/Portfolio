using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Splendor.Tests.TestUtilities.Fakes;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Comprehensive tests for GameBoard - Part 2
/// Focuses on card operations, noble acquisition, and win conditions.
/// </summary>
public class GameBoardTests_Part2
{
    #region Card Purchase Tests (9 tests)

    [Fact]
    public void PurchaseLevel1Card_Successfully_RemovesCardAndDrawsReplacement()
    {
        // Arrange - Create a player with enough tokens to purchase a card
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 3, sapphire: 3, emerald: 3)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Get one of the level 1 cards from the board
        var cardToPurchase = board.Level1Cards[0];
        Assert.NotNull(cardToPurchase); // Ensure there is a card to purchase

        // Give player enough tokens to purchase this specific card
        // Reset tokens to ensure we have exactly what we need
        foreach (var token in player.Tokens.Keys.ToList())
        {
            TestHelpers.SetPlayerTokens(player, token, 0);
        }
        foreach (var cost in cardToPurchase.Price)
        {
            TestHelpers.SetPlayerTokens(player, cost.Key, cost.Value);
        }

        var turn = TurnBuilder.PurchaseCard(cardToPurchase);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Cards.Should().Contain(cardToPurchase, "player should have purchased card");
        board.Level1Cards[0].Should().NotBeSameAs(cardToPurchase, "card should be replaced with new card from stack");
    }

    [Fact]
    public void PurchaseLevel2Card_Successfully_RemovesCardAndDrawsReplacement()
    {
        // Arrange - Create a player with enough tokens
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5, ruby: 5, onyx: 5)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var cardToPurchase = board.Level2Cards[0];
        Assert.NotNull(cardToPurchase);

        // Give player enough tokens for this card
        foreach (var token in player.Tokens.Keys.ToList())
        {
            TestHelpers.SetPlayerTokens(player, token, 0);
        }
        foreach (var cost in cardToPurchase.Price)
        {
            TestHelpers.SetPlayerTokens(player, cost.Key, cost.Value);
        }

        var turn = TurnBuilder.PurchaseCard(cardToPurchase);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Cards.Should().Contain(cardToPurchase, "player should have purchased card");
        board.Level2Cards[0].Should().NotBeSameAs(cardToPurchase, "card should be replaced");
    }

    [Fact]
    public void PurchaseLevel3Card_Successfully_RemovesCardAndDrawsReplacement()
    {
        // Arrange - Create a player with enough tokens
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 7, sapphire: 7, emerald: 7, ruby: 7, onyx: 7)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var cardToPurchase = board.Level3Cards[0];
        Assert.NotNull(cardToPurchase);

        // Give player enough tokens for this card
        foreach (var token in player.Tokens.Keys.ToList())
        {
            TestHelpers.SetPlayerTokens(player, token, 0);
        }
        foreach (var cost in cardToPurchase.Price)
        {
            TestHelpers.SetPlayerTokens(player, cost.Key, cost.Value);
        }

        var turn = TurnBuilder.PurchaseCard(cardToPurchase);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Cards.Should().Contain(cardToPurchase, "player should have purchased card");
        board.Level3Cards[0].Should().NotBeSameAs(cardToPurchase, "card should be replaced");
    }

    [Fact]
    public void PurchaseCard_TokensReturnedToStacks_AfterPurchase()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 3, sapphire: 2, emerald: 1)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var cardToPurchase = board.Level1Cards[0];
        Assert.NotNull(cardToPurchase);

        // Set up player with specific tokens for this card
        foreach (var token in player.Tokens.Keys.ToList())
        {
            TestHelpers.SetPlayerTokens(player, token, 0);
        }
        TestHelpers.SetPlayerTokens(player, Token.Diamond, 2);
        TestHelpers.SetPlayerTokens(player, Token.Sapphire, 2);

        // Create a card that costs 2 diamond and 1 sapphire
        var cheapCard = new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Diamond)
            .WithPrice(diamond: 2, sapphire: 1)
            .Build();

        // Replace the first card with our test card
        TestHelpers.SetBoardCard(board, 1, 0, cheapCard);

        var initialDiamondStack = board.TokenStacks[Token.Diamond];
        var initialSapphireStack = board.TokenStacks[Token.Sapphire];

        var turn = TurnBuilder.PurchaseCard(cheapCard);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        board.TokenStacks[Token.Diamond].Should().Be(initialDiamondStack + 2, "2 diamond tokens should be returned");
        board.TokenStacks[Token.Sapphire].Should().Be(initialSapphireStack + 1, "1 sapphire token should be returned");
    }

    [Fact]
    public void PurchaseReservedCard_Successfully_RemovesFromReservedCards()
    {
        // Arrange - Create a player with a reserved card
        var reservedCard = CardBuilder.CheapLevel1Diamond();

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 3, sapphire: 3, emerald: 3)
            .WithReservedCard(reservedCard)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Give player enough tokens to purchase the reserved card
        foreach (var cost in reservedCard.Price)
        {
            TestHelpers.SetPlayerTokens(player, cost.Key, cost.Value);
        }

        var turn = TurnBuilder.PurchaseCard(reservedCard);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Cards.Should().Contain(reservedCard, "player should have purchased reserved card");
        player.ReservedCards.Should().NotContain(reservedCard, "card should be removed from reserved cards");
    }

    [Fact]
    public void PurchaseCardWithGoldTokens_Successfully_GoldTokensReturnedToStack()
    {
        // Arrange - Create a card that costs 3 diamonds, but player only has 2 diamonds + 1 gold
        var expensiveCard = new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Ruby)
            .WithPrice(diamond: 3)
            .Build();

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 2, gold: 1)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());
        TestHelpers.SetBoardCard(board, 1, 0, expensiveCard);

        var initialGoldStack = board.TokenStacks[Token.Gold];

        var turn = TurnBuilder.PurchaseCard(expensiveCard);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Cards.Should().Contain(expensiveCard, "player should have purchased card with gold");
        board.TokenStacks[Token.Gold].Should().Be(initialGoldStack + 1, "1 gold token should be returned to stack");
    }

    [Fact]
    public void PurchaseCard_WhenStackEmpty_SlotStaysNull()
    {
        // Arrange - Create a game board and manually empty one of the card stacks
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 5, sapphire: 5, emerald: 5)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Draw all cards from Level1 stack to empty it
        // We'll need to purchase multiple cards or use reflection to empty the stack
        // For simplicity, we'll test by checking if the drawn card can be null
        ICard? drawnCard = null;
        while ((drawnCard = board.CardStackLevel1.Draw()) != null)
        {
            // Keep drawing until empty
        }

        // Now the stack is empty, so next draw should return null
        var cardToPurchase = board.Level1Cards[0];
        if (cardToPurchase != null)
        {
            foreach (var cost in cardToPurchase.Price)
            {
                TestHelpers.SetPlayerTokens(player, cost.Key, cost.Value);
            }

            var turn = TurnBuilder.PurchaseCard(cardToPurchase);

            // Act
            var result = board.ExecuteTurn(turn);

            // Assert
            AssertionHelpers.AssertTurnSucceeded(result);
            board.Level1Cards[0].Should().BeNull("slot should remain null when stack is empty");
        }
    }

    [Fact]
    public void PurchaseCard_InsufficientTokens_ReturnsError()
    {
        // Arrange - Create a player without enough tokens
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithTokens(diamond: 1) // Only 1 diamond, not enough for most cards
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Create an expensive card
        var expensiveCard = new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Diamond)
            .WithPrice(diamond: 5, sapphire: 3)
            .Build();

        TestHelpers.SetBoardCard(board, 1, 0, expensiveCard);

        var turn = TurnBuilder.PurchaseCard(expensiveCard);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Error code 2 means "not enough tokens for card"
        AssertionHelpers.AssertTurnFailedWithError(result, 2);
        player.Cards.Should().NotContain(expensiveCard, "player should not have purchased card");
    }

    #endregion

    #region Card Reservation Tests (5 tests)

    [Fact]
    public void ReserveLevel1Card_Successfully_CardAddedToReservedCards()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var cardToReserve = board.Level1Cards[1];
        Assert.NotNull(cardToReserve);

        var turn = TurnBuilder.ReserveCard(cardToReserve);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.ReservedCards.Should().Contain(cardToReserve, "card should be in reserved cards");
        board.Level1Cards[1].Should().NotBeSameAs(cardToReserve, "card should be replaced on board");
    }

    [Fact]
    public void ReserveCard_GoldTokenGiven_WhenGoldAvailable()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var initialGoldStack = board.TokenStacks[Token.Gold];
        var cardToReserve = board.Level1Cards[0];
        Assert.NotNull(cardToReserve);

        var turn = TurnBuilder.ReserveCard(cardToReserve);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Tokens[Token.Gold].Should().Be(1, "player should receive 1 gold token");
        board.TokenStacks[Token.Gold].Should().Be(initialGoldStack - 1, "1 gold should be removed from stack");
    }

    [Fact]
    public void ReserveCard_NoGoldGiven_WhenGoldStackEmpty()
    {
        // Arrange
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Empty the gold stack
        TestHelpers.SetBoardTokenStack(board, Token.Gold, 0);

        var cardToReserve = board.Level1Cards[0];
        Assert.NotNull(cardToReserve);

        var turn = TurnBuilder.ReserveCard(cardToReserve);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Tokens[Token.Gold].Should().Be(0, "player should not receive gold when stack is empty");
        player.ReservedCards.Should().Contain(cardToReserve, "card should still be reserved");
    }

    [Fact]
    public void ReserveCard_ThreeCardsAlreadyReserved_ReturnsError()
    {
        // Arrange - Player already has 3 reserved cards (the maximum)
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithMaxReservedCards() // Sets player to have 3 reserved cards
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var cardToReserve = board.Level1Cards[0];
        Assert.NotNull(cardToReserve);

        var turn = TurnBuilder.ReserveCard(cardToReserve);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Error code 3 means "too many reserved cards"
        AssertionHelpers.AssertTurnFailedWithError(result, 3);
        player.ReservedCards.Should().HaveCount(3, "player should still have only 3 reserved cards");
    }

    [Fact]
    public void ReserveCard_CausingMoreThan10Tokens_ReturnsContinueAction()
    {
        // Arrange - Player has 10 tokens already (at max), reserving will give them gold
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .AtMaxTokens() // Sets player to 10 tokens
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        var cardToReserve = board.Level1Cards[0];
        Assert.NotNull(cardToReserve);

        var turn = TurnBuilder.ReserveCard(cardToReserve);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Continue action code 0 means "return tokens"
        // Note: The card IS reserved and gold IS given, then continue action is returned
        // The player must then return 1 token in a subsequent turn to complete the action
        AssertionHelpers.AssertTurnRequiresTokenReturnForReserve(result);
        player.ReservedCards.Should().Contain(cardToReserve, "card should be reserved even when gold causes overflow");
        player.NumberOfTokens().Should().Be(11, "should have 10 + 1 gold");
        player.Tokens[Token.Gold].Should().Be(1, "should have received gold token");
    }

    #endregion

    #region Noble Acquisition Tests (5 tests)

    [Fact]
    public void PlayerQualifiesForOneNoble_ReturnsContinueAction()
    {
        // Arrange - Create a player with enough cards to qualify for a noble
        var noble = NobleBuilder.ThreeOfThree(); // Requires 3 Diamond, 3 Sapphire, 3 Emerald cards

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                // Add 3 Diamond cards
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                // Add 3 Sapphire cards
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                // Add 3 Emerald cards
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build()
            )
            .WithTokens(diamond: 2) // Just need some tokens to take a turn
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Replace one of the nobles with our test noble
        TestHelpers.ClearBoardNobles(board);
        TestHelpers.AddBoardNoble(board, noble);

        // Take a simple turn (take 1 token) which should trigger noble check
        var turn = new Turn(TokenHelper.One(Token.Diamond));

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Continue action code 1 means "choose noble"
        AssertionHelpers.AssertTurnRequiresNobleSelection(result);
        result.ContinueAction!.Nobles.Should().Contain(noble, "player should be offered the noble");
    }

    [Fact]
    public void PlayerQualifiesForMultipleNobles_ReturnsContinueAction()
    {
        // Arrange - Create a player with enough cards to qualify for multiple nobles
        var noble1 = NobleBuilder.WithRequirements(diamond: 3, sapphire: 3, emerald: 3);
        var noble2 = NobleBuilder.WithRequirements(diamond: 4, onyx: 4);

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                // Add 4 Diamond cards (qualifies for both)
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                // Add 3 Sapphire cards
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                // Add 3 Emerald cards
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                // Add 4 Onyx cards
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build(),
                new CardBuilder().WithType(Token.Onyx).Build()
            )
            .WithTokens(diamond: 1)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Replace nobles with our test nobles
        TestHelpers.ClearBoardNobles(board);
        TestHelpers.AddBoardNoble(board, noble1);
        TestHelpers.AddBoardNoble(board, noble2);

        var turn = new Turn(TokenHelper.One(Token.Diamond));

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnRequiresNobleSelection(result);
        result.ContinueAction!.Nobles.Should().HaveCount(2, "player should be offered both nobles");
        result.ContinueAction.Nobles.Should().Contain(noble1);
        result.ContinueAction.Nobles.Should().Contain(noble2);
    }

    [Fact]
    public void AcquireNoble_Successfully_NobleAddedToPlayer()
    {
        // Arrange
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

        var turn = TurnBuilder.AcquireNoble(noble);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player.Nobles.Should().Contain(noble, "noble should be added to player");
        player.PrestigePoints.Should().Be(3, "player should gain 3 prestige points from noble");
    }

    [Fact]
    public void AcquireNoble_NobleRemovedFromBoard_AfterAcquisition()
    {
        // Arrange
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

        var turn = TurnBuilder.AcquireNoble(noble);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        board.Nobles.Should().NotContain(noble, "noble should be removed from board");
    }

    [Fact]
    public void AcquireNoble_NotQualified_ReturnsError()
    {
        // Arrange - Player doesn't have enough cards for the noble
        var noble = NobleBuilder.ThreeOfThree(); // Requires 3 of each of 3 types

        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                // Only 2 Diamond cards (need 3)
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build()
            )
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());
        TestHelpers.ClearBoardNobles(board);
        TestHelpers.AddBoardNoble(board, noble);

        var turn = TurnBuilder.AcquireNoble(noble);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Error code 5 means "not enough cards for noble"
        AssertionHelpers.AssertTurnFailedWithError(result, 5);
        player.Nobles.Should().NotContain(noble, "player should not acquire noble");
    }

    #endregion

    #region Win Condition Tests (5 tests)

    [Fact]
    public void PlayerReaches15Points_LastRoundBecomesTrue()
    {
        // Arrange - Create a 2-player game where player 1 will reach 15 points
        var highValueCard = new CardBuilder()
            .WithLevel(3)
            .WithType(Token.Diamond)
            .WithPrestigePoints(5)
            .AsFreeCard() // Free so player can easily purchase it
            .Build();

        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .Build();

        // Manually set prestige points to 10 using reflection
        // (PlayerBuilder doesn't calculate prestige points from cards)
        typeof(Player).GetProperty("PrestigePoints")!.SetValue(player1, 10u);

        var board = new GameBoard(new List<IPlayer> { player1, player2 }, TestHelpers.CreateMockGameDataService());
        TestHelpers.SetBoardCard(board, 3, 0, highValueCard);

        var turn = TurnBuilder.PurchaseCard(highValueCard);

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        player1.PrestigePoints.Should().BeGreaterOrEqualTo(15, "player should have at least 15 points");
        AssertionHelpers.AssertLastRound(board);
        AssertionHelpers.AssertGameNotOver(board); // Game isn't over yet, just last round
    }

    [Fact]
    public void LastRound_LastPlayer_GameOverBecomesTrue()
    {
        // Arrange - Create a 2-player game where player 1 has 15+ points
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                new CardBuilder().WithPrestigePoints(15).Build()
            )
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .Build();

        var board = new GameBoard(new List<IPlayer> { player1, player2 }, TestHelpers.CreateMockGameDataService());

        // Manually set LastRound to true
        typeof(GameBoard).GetProperty("LastRound")!.SetValue(board, true);

        // Set current player to player 2 (last player, index 1)
        typeof(GameBoard).GetProperty("CurrentPlayer")!.SetValue(board, 1);

        // Player 2 takes a simple turn
        var turn = new Turn(TokenHelper.One(Token.Diamond));

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertGameOver(board);
        result.GameOver.Should().BeTrue("result should indicate game is over");
    }

    [Fact]
    public void GameOver_TurnReturnsError()
    {
        // Arrange - Create a game and manually set it to GameOver
        var player = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .Build();

        var dummyPlayer = new Player("Dummy", 1);
        var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

        // Use reflection to set GameOver to true
        typeof(GameBoard).GetProperty("GameOver")!.SetValue(board, true);

        var turn = new Turn(TokenHelper.One(Token.Diamond));

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert - Error code 8 means "turn during game over"
        AssertionHelpers.AssertTurnFailedWithError(result, 8);
    }

    [Fact]
    public void LastRound_NotLastPlayer_GameContinues()
    {
        // Arrange - Create a 3-player game where player 1 has 15+ points
        var player1 = new PlayerBuilder()
            .WithName("Player1")
            .WithId(0)
            .WithCards(
                new CardBuilder().WithPrestigePoints(15).Build()
            )
            .Build();

        var player2 = new PlayerBuilder()
            .WithName("Player2")
            .WithId(1)
            .Build();

        var player3 = new PlayerBuilder()
            .WithName("Player3")
            .WithId(2)
            .Build();

        var board = new GameBoard(new List<IPlayer> { player1, player2, player3 }, TestHelpers.CreateMockGameDataService());

        // Manually set LastRound to true and current player to player 1 (not last player)
        typeof(GameBoard).GetProperty("LastRound")!.SetValue(board, true);
        typeof(GameBoard).GetProperty("CurrentPlayer")!.SetValue(board, 1);

        var turn = new Turn(TokenHelper.One(Token.Diamond));

        // Act
        var result = board.ExecuteTurn(turn);

        // Assert
        AssertionHelpers.AssertTurnSucceeded(result);
        AssertionHelpers.AssertLastRound(board);
        AssertionHelpers.AssertGameNotOver(board); // Game continues until last player
        result.GameOver.Should().BeFalse("game should not be over until last player finishes");
    }

    [Fact]
    public void CurrentPlayer_AdvancesAndWraps_AfterTurn()
    {
        // Arrange - Create a 3-player game
        var player1 = new PlayerBuilder().WithName("Player1").WithId(0).Build();
        var player2 = new PlayerBuilder().WithName("Player2").WithId(1).Build();
        var player3 = new PlayerBuilder().WithName("Player3").WithId(2).Build();

        var board = new GameBoard(new List<IPlayer> { player1, player2, player3 }, TestHelpers.CreateMockGameDataService());

        board.CurrentPlayer.Should().Be(0, "game should start with player 0");

        // Act - Player 0 takes a turn
        var turn1 = new Turn(TokenHelper.One(Token.Diamond));
        board.ExecuteTurn(turn1);

        // Assert - Should advance to player 1
        board.CurrentPlayer.Should().Be(1, "should advance to player 1");

        // Act - Player 1 takes a turn
        var turn2 = new Turn(TokenHelper.One(Token.Sapphire));
        board.ExecuteTurn(turn2);

        // Assert - Should advance to player 2
        board.CurrentPlayer.Should().Be(2, "should advance to player 2");

        // Act - Player 2 takes a turn
        var turn3 = new Turn(TokenHelper.One(Token.Emerald));
        board.ExecuteTurn(turn3);

        // Assert - Should wrap back to player 0
        board.CurrentPlayer.Should().Be(0, "should wrap back to player 0");
    }

    #endregion
}
