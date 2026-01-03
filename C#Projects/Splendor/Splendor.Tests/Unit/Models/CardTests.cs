using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Tests for the Card class.
/// </summary>
public class CardTests
{
    [Fact]
    public void Constructor_SetsAllPropertiesCorrectly()
    {
        // Arrange
        var level = 2u;
        var type = Token.Sapphire;
        var prestigePoints = 3u;
        var price = TokenHelper.Create(diamond: 2, ruby: 3, emerald: 1);
        var imageName = "Level2-S-3P-2D-3R-1E.jpg";

        // Act
        var card = new Card(level, type, prestigePoints, price, imageName);

        // Assert
        card.Level.Should().Be(2);
        card.Type.Should().Be(Token.Sapphire);
        card.PrestigePoints.Should().Be(3);
        card.Price.Should().NotBeNull();
        card.Price[Token.Diamond].Should().Be(2);
        card.Price[Token.Ruby].Should().Be(3);
        card.Price[Token.Emerald].Should().Be(1);
        card.ImageName.Should().Be("Level2-S-3P-2D-3R-1E.jpg");
    }

    [Fact]
    public void Constructor_SetsLevel()
    {
        // Arrange
        var price = TokenHelper.Create(diamond: 1);

        // Act
        var card = new Card(3, Token.Diamond, 0, price, "test.jpg");

        // Assert
        card.Level.Should().Be(3);
    }

    [Fact]
    public void Constructor_SetsType()
    {
        // Arrange
        var price = TokenHelper.Create(sapphire: 2);

        // Act
        var card = new Card(1, Token.Emerald, 0, price, "test.jpg");

        // Assert
        card.Type.Should().Be(Token.Emerald);
    }

    [Fact]
    public void Constructor_SetsPrestigePoints()
    {
        // Arrange
        var price = TokenHelper.Create(onyx: 4);

        // Act
        var card = new Card(2, Token.Ruby, 5, price, "test.jpg");

        // Assert
        card.PrestigePoints.Should().Be(5);
    }

    [Fact]
    public void Constructor_SetsPriceDictionary()
    {
        // Arrange
        var price = TokenHelper.Create(diamond: 3, sapphire: 2, emerald: 1, ruby: 4, onyx: 2);

        // Act
        var card = new Card(1, Token.Diamond, 1, price, "test.jpg");

        // Assert
        card.Price.Should().NotBeNull();
        card.Price.Should().HaveCount(6); // All 6 token types in TokenHelper.Create
        card.Price[Token.Diamond].Should().Be(3);
        card.Price[Token.Sapphire].Should().Be(2);
        card.Price[Token.Emerald].Should().Be(1);
        card.Price[Token.Ruby].Should().Be(4);
        card.Price[Token.Onyx].Should().Be(2);
        card.Price[Token.Gold].Should().Be(0);
    }

    [Fact]
    public void Constructor_SetsImageName()
    {
        // Arrange
        var price = TokenHelper.Create(diamond: 1);
        var imageName = "Level1-D-0P-1D.jpg";

        // Act
        var card = new Card(1, Token.Diamond, 0, price, imageName);

        // Assert
        card.ImageName.Should().Be("Level1-D-0P-1D.jpg");
    }

    [Fact]
    public void ParameterlessConstructor_CreatesInstance()
    {
        // Act
        var card = new Card();

        // Assert
        card.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithEmptyPrice_CreatesValidCard()
    {
        // Arrange
        var emptyPrice = new Dictionary<Token, int>();

        // Act
        var card = new Card(1, Token.Diamond, 0, emptyPrice, "free-card.jpg");

        // Assert
        card.Price.Should().NotBeNull();
        card.Price.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithZeroPrestigePoints_CreatesValidCard()
    {
        // Arrange
        var price = TokenHelper.Create(sapphire: 1);

        // Act
        var card = new Card(1, Token.Diamond, 0, price, "no-prestige.jpg");

        // Assert
        card.PrestigePoints.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithHighPrestigePoints_CreatesValidCard()
    {
        // Arrange
        var price = TokenHelper.Create(diamond: 7, sapphire: 3);

        // Act
        var card = new Card(3, Token.Sapphire, 5, price, "high-prestige.jpg");

        // Assert
        card.PrestigePoints.Should().Be(5);
    }
}
