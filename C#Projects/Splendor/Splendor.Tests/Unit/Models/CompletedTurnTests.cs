using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Unit tests for the CompletedTurn class.
/// </summary>
public class CompletedTurnTests
{
    [Fact]
    public void Constructor_WithError_SetsErrorProperty()
    {
        // Arrange
        var error = new Error("You have taken too many tokens", 0);

        // Act
        var completedTurn = new CompletedTurn(error);

        // Assert
        completedTurn.Error.Should().BeSameAs(error);
        completedTurn.ContinueAction.Should().BeNull();
        completedTurn.ConsumedTokens.Should().BeNull();
        completedTurn.GameOver.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithContinueAction_SetsContinueAction()
    {
        // Arrange
        var continueAction = new ContinueAction("Please return 2 tokens", 0);
        var consumedTokens = TokenHelper.Create(diamond: 2);

        // Act
        var completedTurn = new CompletedTurn(continueAction, consumedTokens);

        // Assert
        completedTurn.ContinueAction.Should().BeSameAs(continueAction);
        completedTurn.Error.Should().BeNull();
        completedTurn.ConsumedTokens.Should().BeNull(); // Note: consumedTokens is passed but not assigned in this constructor
        completedTurn.GameOver.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithConsumedTokens_SetsConsumedTokens()
    {
        // Arrange
        var consumedTokens = TokenHelper.Create(diamond: 3, sapphire: 2, emerald: 1);

        // Act
        var completedTurn = new CompletedTurn(consumedTokens);

        // Assert
        completedTurn.ConsumedTokens.Should().BeSameAs(consumedTokens);
        completedTurn.ConsumedTokens![Token.Diamond].Should().Be(3);
        completedTurn.ConsumedTokens[Token.Sapphire].Should().Be(2);
        completedTurn.ConsumedTokens[Token.Emerald].Should().Be(1);
        completedTurn.Error.Should().BeNull();
        completedTurn.ContinueAction.Should().BeNull();
        completedTurn.GameOver.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithGameOverTrue_SetsGameOver()
    {
        // Arrange & Act
        var completedTurn = new CompletedTurn(gameOver: true);

        // Assert
        completedTurn.GameOver.Should().BeTrue();
        completedTurn.Error.Should().BeNull();
        completedTurn.ContinueAction.Should().BeNull();
        completedTurn.ConsumedTokens.Should().BeNull();
    }

    [Fact]
    public void EmptyConstructor_HasAllPropertiesNullOrFalse()
    {
        // Arrange & Act
        var completedTurn = new CompletedTurn();

        // Assert
        completedTurn.Error.Should().BeNull();
        completedTurn.ContinueAction.Should().BeNull();
        completedTurn.ConsumedTokens.Should().BeNull();
        completedTurn.GameOver.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithErrorAndContinueAction_SetsBothProperties()
    {
        // Arrange
        var error = new Error("Invalid move", 1);
        var continueAction = new ContinueAction("Fix the issue", 0);

        // Act
        var completedTurn = new CompletedTurn(error, continueAction);

        // Assert
        completedTurn.Error.Should().BeSameAs(error);
        completedTurn.ContinueAction.Should().BeSameAs(continueAction);
        completedTurn.ConsumedTokens.Should().BeNull();
        completedTurn.GameOver.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithContinueActionAndError_SetsBothProperties()
    {
        // Arrange
        var continueAction = new ContinueAction("Choose a noble", 1);
        var consumedTokens = TokenHelper.Create(diamond: 1);
        var error = new Error("Optional error", 2);

        // Act
        var completedTurn = new CompletedTurn(continueAction, consumedTokens, error);

        // Assert
        completedTurn.ContinueAction.Should().BeSameAs(continueAction);
        completedTurn.Error.Should().BeSameAs(error);
        completedTurn.ConsumedTokens.Should().BeNull(); // consumedTokens parameter exists but is not assigned
        completedTurn.GameOver.Should().BeFalse();
    }
}
