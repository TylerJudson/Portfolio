using FluentAssertions;
using Splendor.Models.Implementation;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Unit tests for the PotentialGame class.
/// </summary>
public class PotentialGameTests
{
    [Fact]
    public void Constructor_SetsId()
    {
        // Arrange
        var id = 12345;
        var creatorName = "Alice";

        // Act
        var game = new PotentialGame(id, creatorName);

        // Assert
        game.Id.Should().Be(id);
    }

    [Fact]
    public void Constructor_SetsCreatingPlayerName()
    {
        // Arrange
        var id = 67890;
        var creatorName = "Bob";

        // Act
        var game = new PotentialGame(id, creatorName);

        // Assert
        game.CreatingPlayerName.Should().Be(creatorName);
    }

    [Fact]
    public void Constructor_AddsCreatorToPlayersDictionaryAtIdZero()
    {
        // Arrange
        var id = 11111;
        var creatorName = "Charlie";

        // Act
        var game = new PotentialGame(id, creatorName);

        // Assert
        game.Players.Should().ContainKey(0);
        game.Players[0].Should().Be(creatorName);
        game.Players.Should().HaveCount(1);
    }

    [Fact]
    public void MaxPlayers_IsAlwaysFour()
    {
        // Arrange
        var id = 22222;
        var creatorName = "Diana";

        // Act
        var game = new PotentialGame(id, creatorName);

        // Assert
        game.MaxPlayers.Should().Be(4);
    }

    [Fact]
    public void TimeCreated_IsSetToUtcNow()
    {
        // Arrange
        var beforeCreate = DateTime.UtcNow;

        // Act
        var game = new PotentialGame(99999, "TestPlayer");
        var afterCreate = DateTime.UtcNow;

        // Assert
        game.TimeCreated.Should().BeOnOrAfter(beforeCreate);
        game.TimeCreated.Should().BeOnOrBefore(afterCreate);
        game.TimeCreated.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Players_InitializesAsNewDictionary()
    {
        // Arrange & Act
        var game = new PotentialGame(33333, "TestCreator");

        // Assert
        game.Players.Should().NotBeNull();
        game.Players.Should().BeOfType<Dictionary<int, string>>();
    }
}
