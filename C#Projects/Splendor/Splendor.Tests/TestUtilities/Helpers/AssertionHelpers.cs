using FluentAssertions;
using Splendor.Models;

namespace Splendor.Tests.TestUtilities.Helpers;

/// <summary>
/// Custom assertion helpers for game-specific validations.
/// </summary>
public static class AssertionHelpers
{
    /// <summary>
    /// Asserts that a completed turn was successful (no error, no continue action needed).
    /// </summary>
    public static void AssertTurnSucceeded(ICompletedTurn completedTurn)
    {
        completedTurn.Error.Should().BeNull("turn should succeed without error");
        completedTurn.ContinueAction.Should().BeNull("turn should complete without requiring further action");
    }

    /// <summary>
    /// Asserts that a completed turn failed with a specific error code.
    /// </summary>
    public static void AssertTurnFailedWithError(ICompletedTurn completedTurn, int expectedErrorCode)
    {
        completedTurn.Error.Should().NotBeNull("turn should fail with an error");
        completedTurn.Error!.ErrorCode.Should().Be(expectedErrorCode, $"error code should be {expectedErrorCode}");
    }

    /// <summary>
    /// Asserts that a completed turn failed with an error (any error code).
    /// </summary>
    public static void AssertTurnFailed(ICompletedTurn completedTurn)
    {
        completedTurn.Error.Should().NotBeNull("turn should fail with an error");
    }

    /// <summary>
    /// Asserts that a completed turn requires a continue action with a specific action code.
    /// </summary>
    public static void AssertTurnRequiresContinueAction(ICompletedTurn completedTurn, int expectedActionCode)
    {
        completedTurn.ContinueAction.Should().NotBeNull("turn should require a continue action");
        completedTurn.ContinueAction!.ActionCode.Should().Be(expectedActionCode, $"action code should be {expectedActionCode}");
    }

    /// <summary>
    /// Asserts that a completed turn requires returning tokens (action code 0).
    /// </summary>
    public static void AssertTurnRequiresTokenReturn(ICompletedTurn completedTurn)
    {
        AssertTurnRequiresContinueAction(completedTurn, 0);
    }

    /// <summary>
    /// Asserts that a completed turn requires noble selection (action code 1).
    /// </summary>
    public static void AssertTurnRequiresNobleSelection(ICompletedTurn completedTurn)
    {
        AssertTurnRequiresContinueAction(completedTurn, 1);
    }

    /// <summary>
    /// Asserts that a completed turn requires returning tokens for reserved card (action code 0).
    /// </summary>
    public static void AssertTurnRequiresTokenReturnForReserve(ICompletedTurn completedTurn)
    {
        AssertTurnRequiresContinueAction(completedTurn, 0);
    }

    /// <summary>
    /// Asserts that a player has specific token counts.
    /// </summary>
    public static void AssertPlayerTokens(
        IPlayer player,
        int diamond,
        int sapphire,
        int emerald,
        int ruby,
        int onyx,
        int gold)
    {
        player.Tokens[Token.Diamond].Should().Be(diamond, "diamond tokens should match");
        player.Tokens[Token.Sapphire].Should().Be(sapphire, "sapphire tokens should match");
        player.Tokens[Token.Emerald].Should().Be(emerald, "emerald tokens should match");
        player.Tokens[Token.Ruby].Should().Be(ruby, "ruby tokens should match");
        player.Tokens[Token.Onyx].Should().Be(onyx, "onyx tokens should match");
        player.Tokens[Token.Gold].Should().Be(gold, "gold tokens should match");
    }

    /// <summary>
    /// Asserts that a game board has specific token stack counts.
    /// </summary>
    public static void AssertTokenStacks(
        IGameBoard board,
        int diamond,
        int sapphire,
        int emerald,
        int ruby,
        int onyx,
        int gold)
    {
        board.TokenStacks[Token.Diamond].Should().Be(diamond, "diamond stack should match");
        board.TokenStacks[Token.Sapphire].Should().Be(sapphire, "sapphire stack should match");
        board.TokenStacks[Token.Emerald].Should().Be(emerald, "emerald stack should match");
        board.TokenStacks[Token.Ruby].Should().Be(ruby, "ruby stack should match");
        board.TokenStacks[Token.Onyx].Should().Be(onyx, "onyx stack should match");
        board.TokenStacks[Token.Gold].Should().Be(gold, "gold stack should match");
    }

    /// <summary>
    /// Asserts that a player can afford a card.
    /// </summary>
    public static void AssertCanAffordCard(IPlayer player, ICard card)
    {
        (player.CanPurchaseCard(card) || player.CanPurchaseCardWithGold(card))
            .Should().BeTrue("player should be able to afford the card");
    }

    /// <summary>
    /// Asserts that a player cannot afford a card.
    /// </summary>
    public static void AssertCannotAffordCard(IPlayer player, ICard card)
    {
        player.CanPurchaseCard(card).Should().BeFalse("player should not be able to afford without gold");
        player.CanPurchaseCardWithGold(card).Should().BeFalse("player should not be able to afford even with gold");
    }

    /// <summary>
    /// Asserts that a player qualifies for a noble.
    /// </summary>
    public static void AssertQualifiesForNoble(IPlayer player, INoble noble)
    {
        player.CanAcquireNoble(noble).Should().BeTrue("player should qualify for the noble");
    }

    /// <summary>
    /// Asserts that a player does not qualify for a noble.
    /// </summary>
    public static void AssertDoesNotQualifyForNoble(IPlayer player, INoble noble)
    {
        player.CanAcquireNoble(noble).Should().BeFalse("player should not qualify for the noble");
    }

    /// <summary>
    /// Asserts that the game is over.
    /// </summary>
    public static void AssertGameOver(IGameBoard board)
    {
        board.GameOver.Should().BeTrue("game should be over");
    }

    /// <summary>
    /// Asserts that the game is not over.
    /// </summary>
    public static void AssertGameNotOver(IGameBoard board)
    {
        board.GameOver.Should().BeFalse("game should not be over");
    }

    /// <summary>
    /// Asserts that it is the last round.
    /// </summary>
    public static void AssertLastRound(IGameBoard board)
    {
        board.LastRound.Should().BeTrue("it should be the last round");
    }

    /// <summary>
    /// Asserts that it is not the last round.
    /// </summary>
    public static void AssertNotLastRound(IGameBoard board)
    {
        board.LastRound.Should().BeFalse("it should not be the last round");
    }
}
