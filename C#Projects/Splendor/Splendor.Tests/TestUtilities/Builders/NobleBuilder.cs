using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Tests.TestUtilities.Builders;

/// <summary>
/// Fluent builder for creating test Noble instances.
/// </summary>
public class NobleBuilder
{
    private Dictionary<Token, int> _criteria = new();
    private string _imageName = "test-noble.jpg";

    public NobleBuilder WithCriteria(Dictionary<Token, int> criteria)
    {
        _criteria = criteria;
        return this;
    }

    public NobleBuilder WithCriteria(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0)
    {
        _criteria = new Dictionary<Token, int>();
        if (diamond > 0) _criteria[Token.Diamond] = diamond;
        if (sapphire > 0) _criteria[Token.Sapphire] = sapphire;
        if (emerald > 0) _criteria[Token.Emerald] = emerald;
        if (ruby > 0) _criteria[Token.Ruby] = ruby;
        if (onyx > 0) _criteria[Token.Onyx] = onyx;
        return this;
    }

    public NobleBuilder WithImageName(string name)
    {
        _imageName = name;
        return this;
    }

    public Noble Build()
    {
        return new Noble(_criteria, _imageName);
    }

    // Static factory methods for common scenarios

    /// <summary>
    /// Creates a noble requiring 3 of 3 different card types (e.g., 3 Diamond, 3 Sapphire, 3 Emerald).
    /// </summary>
    public static Noble ThreeOfThree()
    {
        return new NobleBuilder()
            .WithCriteria(diamond: 3, sapphire: 3, emerald: 3)
            .WithImageName("Noble-3D-3S-3E.jpg")
            .Build();
    }

    /// <summary>
    /// Creates a noble requiring 4 of 2 different card types (e.g., 4 Diamond, 4 Onyx).
    /// </summary>
    public static Noble FourOfTwo()
    {
        return new NobleBuilder()
            .WithCriteria(diamond: 4, onyx: 4)
            .WithImageName("Noble-4D-4O.jpg")
            .Build();
    }

    /// <summary>
    /// Creates a noble with easy-to-meet criteria (just 1 of each of 3 types).
    /// </summary>
    public static Noble EasyNoble()
    {
        return new NobleBuilder()
            .WithCriteria(diamond: 1, sapphire: 1, emerald: 1)
            .WithImageName("Noble-Easy.jpg")
            .Build();
    }

    /// <summary>
    /// Creates a noble with specific criteria.
    /// </summary>
    public static Noble WithRequirements(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0)
    {
        return new NobleBuilder()
            .WithCriteria(diamond, sapphire, emerald, ruby, onyx)
            .WithImageName($"Noble-{diamond}D-{sapphire}S-{emerald}E-{ruby}R-{onyx}O.jpg")
            .Build();
    }
}
