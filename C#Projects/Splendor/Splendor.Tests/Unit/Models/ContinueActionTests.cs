using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Unit tests for the ContinueAction class.
/// </summary>
public class ContinueActionTests
{
    [Fact]
    public void Constructor_SetsMessage()
    {
        // Arrange
        var message = "Please return 2 tokens";
        var actionCode = 0;

        // Act
        var continueAction = new ContinueAction(message, actionCode);

        // Assert
        continueAction.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_SetsActionCode()
    {
        // Arrange
        var message = "Choose a noble";
        var actionCode = 1;

        // Act
        var continueAction = new ContinueAction(message, actionCode);

        // Assert
        continueAction.ActionCode.Should().Be(actionCode);
        continueAction.Nobles.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNobles_SetsNoblesList()
    {
        // Arrange
        var message = "Choose a noble from the list";
        var actionCode = 1;
        var nobles = new List<INoble>
        {
            NobleBuilder.ThreeOfThree(),
            NobleBuilder.FourOfTwo()
        };

        // Act
        var continueAction = new ContinueAction(message, actionCode, nobles);

        // Assert
        continueAction.Message.Should().Be(message);
        continueAction.ActionCode.Should().Be(actionCode);
        continueAction.Nobles.Should().HaveCount(2);
        continueAction.Nobles.Should().BeEquivalentTo(nobles);
    }

    [Fact]
    public void Constructor_WithEmptyNoblesList_SetsEmptyList()
    {
        // Arrange
        var message = "No nobles available";
        var actionCode = 1;
        var nobles = new List<INoble>();

        // Act
        var continueAction = new ContinueAction(message, actionCode, nobles);

        // Assert
        continueAction.Nobles.Should().BeEmpty();
        continueAction.Nobles.Should().HaveCount(0);
    }
}
