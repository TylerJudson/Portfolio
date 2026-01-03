using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Tests.TestUtilities.Builders;

/// <summary>
/// Fluent builder for creating test Card instances.
/// </summary>
public class CardBuilder
{
    private uint _level = 1;
    private Token _type = Token.Diamond;
    private uint _prestigePoints = 0;
    private Dictionary<Token, int> _price = new();
    private string _imageName = "test-card.jpg";

    public CardBuilder WithLevel(uint level)
    {
        _level = level;
        return this;
    }

    public CardBuilder WithType(Token type)
    {
        _type = type;
        return this;
    }

    public CardBuilder WithPrestigePoints(uint points)
    {
        _prestigePoints = points;
        return this;
    }

    public CardBuilder WithPrice(Dictionary<Token, int> price)
    {
        _price = price;
        return this;
    }

    public CardBuilder WithPrice(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0)
    {
        _price = new Dictionary<Token, int>();
        if (diamond > 0) _price[Token.Diamond] = diamond;
        if (sapphire > 0) _price[Token.Sapphire] = sapphire;
        if (emerald > 0) _price[Token.Emerald] = emerald;
        if (ruby > 0) _price[Token.Ruby] = ruby;
        if (onyx > 0) _price[Token.Onyx] = onyx;
        return this;
    }

    public CardBuilder WithImageName(string name)
    {
        _imageName = name;
        return this;
    }

    public CardBuilder AsFreeCard()
    {
        _price = new Dictionary<Token, int>();
        return this;
    }

    public Card Build()
    {
        return new Card(_level, _type, _prestigePoints, _price, _imageName);
    }

    // Static factory methods for common scenarios

    /// <summary>
    /// Creates a cheap level 1 diamond card with no prestige.
    /// </summary>
    public static Card CheapLevel1Diamond()
    {
        return new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Diamond)
            .WithPrestigePoints(0)
            .WithPrice(sapphire: 1, emerald: 1)
            .WithImageName("Level1-D-0P-1S-1E.jpg")
            .Build();
    }

    /// <summary>
    /// Creates an expensive level 3 card with high prestige.
    /// </summary>
    public static Card ExpensiveLevel3()
    {
        return new CardBuilder()
            .WithLevel(3)
            .WithType(Token.Sapphire)
            .WithPrestigePoints(5)
            .WithPrice(diamond: 7, sapphire: 3)
            .WithImageName("Level3-S-5P-7D-3S.jpg")
            .Build();
    }

    /// <summary>
    /// Creates a free card with no cost.
    /// </summary>
    public static Card FreeCard()
    {
        return new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Diamond)
            .WithPrestigePoints(0)
            .AsFreeCard()
            .WithImageName("test-free-card.jpg")
            .Build();
    }

    /// <summary>
    /// Creates a card with a specific cost for testing purchase logic.
    /// </summary>
    public static Card WithCost(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0)
    {
        return new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Diamond)
            .WithPrestigePoints(1)
            .WithPrice(diamond, sapphire, emerald, ruby, onyx)
            .WithImageName($"test-card-{diamond}D-{sapphire}S-{emerald}E-{ruby}R-{onyx}O.jpg")
            .Build();
    }

    /// <summary>
    /// Creates a card for each level with specified cost.
    /// </summary>
    public static Card Level1WithCost(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0)
    {
        return new CardBuilder()
            .WithLevel(1)
            .WithType(Token.Diamond)
            .WithPrestigePoints(0)
            .WithPrice(diamond, sapphire, emerald, ruby, onyx)
            .Build();
    }

    public static Card Level2WithCost(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0)
    {
        return new CardBuilder()
            .WithLevel(2)
            .WithType(Token.Sapphire)
            .WithPrestigePoints(2)
            .WithPrice(diamond, sapphire, emerald, ruby, onyx)
            .Build();
    }

    public static Card Level3WithCost(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0)
    {
        return new CardBuilder()
            .WithLevel(3)
            .WithType(Token.Emerald)
            .WithPrestigePoints(4)
            .WithPrice(diamond, sapphire, emerald, ruby, onyx)
            .Build();
    }
}
