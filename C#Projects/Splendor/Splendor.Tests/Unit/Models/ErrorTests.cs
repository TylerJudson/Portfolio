using FluentAssertions;
using Splendor.Models.Implementation;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Unit tests for the Error class.
/// </summary>
public class ErrorTests
{
    [Fact]
    public void Constructor_SetsMessage()
    {
        // Arrange
        var message = "You have taken too many tokens";
        var errorCode = 0;

        // Act
        var error = new Error(message, errorCode);

        // Assert
        error.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_SetsErrorCode()
    {
        // Arrange
        var message = "The player has too many tokens";
        var errorCode = 1;

        // Act
        var error = new Error(message, errorCode);

        // Assert
        error.ErrorCode.Should().Be(errorCode);
    }

    [Fact]
    public void Constructor_SetsBothProperties()
    {
        // Arrange
        var message = "The player doesn't have enough tokens for a card";
        var errorCode = 2;

        // Act
        var error = new Error(message, errorCode);

        // Assert
        error.Message.Should().Be(message);
        error.ErrorCode.Should().Be(errorCode);
    }

    [Theory]
    [InlineData("Player has taken too many tokens", 0)]
    [InlineData("Player has too many tokens", 1)]
    [InlineData("Player doesn't have enough tokens for a card", 2)]
    [InlineData("Player has too many reserved cards", 3)]
    [InlineData("Player has taken more than 3 types of tokens", 4)]
    [InlineData("Player doesn't have enough cards for a noble", 5)]
    [InlineData("Player tried to take a gold token", 6)]
    [InlineData("Player doesn't have enough tokens to return", 7)]
    [InlineData("Player tries do a turn when the game is over", 8)]
    public void Constructor_WithVariousErrorCodes_StoresCorrectly(string message, int errorCode)
    {
        // Act
        var error = new Error(message, errorCode);

        // Assert
        error.Message.Should().Be(message);
        error.ErrorCode.Should().Be(errorCode);
    }
}
