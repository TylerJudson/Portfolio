using Splendor.Models;

namespace Splendor.Tests.TestUtilities.Helpers;

/// <summary>
/// Helper class for creating token dictionaries in tests.
/// </summary>
public static class TokenHelper
{
    /// <summary>
    /// Creates an empty token dictionary with all zeros.
    /// </summary>
    public static Dictionary<Token, int> Empty()
    {
        return new Dictionary<Token, int>
        {
            { Token.Diamond, 0 },
            { Token.Sapphire, 0 },
            { Token.Emerald, 0 },
            { Token.Ruby, 0 },
            { Token.Onyx, 0 },
            { Token.Gold, 0 }
        };
    }

    /// <summary>
    /// Creates a token dictionary with specified amounts.
    /// </summary>
    public static Dictionary<Token, int> Create(
        int diamond = 0,
        int sapphire = 0,
        int emerald = 0,
        int ruby = 0,
        int onyx = 0,
        int gold = 0)
    {
        return new Dictionary<Token, int>
        {
            { Token.Diamond, diamond },
            { Token.Sapphire, sapphire },
            { Token.Emerald, emerald },
            { Token.Ruby, ruby },
            { Token.Onyx, onyx },
            { Token.Gold, gold }
        };
    }

    /// <summary>
    /// Creates a token dictionary for taking 1 of each of 3 different tokens.
    /// </summary>
    public static Dictionary<Token, int> ThreeDifferent(Token t1, Token t2, Token t3)
    {
        return new Dictionary<Token, int>
        {
            { t1, 1 },
            { t2, 1 },
            { t3, 1 }
        };
    }

    /// <summary>
    /// Creates a token dictionary for taking 2 of the same token.
    /// </summary>
    public static Dictionary<Token, int> TwoSame(Token token)
    {
        return new Dictionary<Token, int>
        {
            { token, 2 }
        };
    }

    /// <summary>
    /// Creates a token dictionary for taking 1 token.
    /// </summary>
    public static Dictionary<Token, int> One(Token token)
    {
        return new Dictionary<Token, int>
        {
            { token, 1 }
        };
    }

    /// <summary>
    /// Creates token stacks for a 2-player game (4 each gem, 5 gold).
    /// </summary>
    public static Dictionary<Token, int> TwoPlayerStacks()
    {
        return new Dictionary<Token, int>
        {
            { Token.Diamond, 4 },
            { Token.Sapphire, 4 },
            { Token.Emerald, 4 },
            { Token.Ruby, 4 },
            { Token.Onyx, 4 },
            { Token.Gold, 5 }
        };
    }

    /// <summary>
    /// Creates token stacks for a 3-player game (5 each gem, 5 gold).
    /// </summary>
    public static Dictionary<Token, int> ThreePlayerStacks()
    {
        return new Dictionary<Token, int>
        {
            { Token.Diamond, 5 },
            { Token.Sapphire, 5 },
            { Token.Emerald, 5 },
            { Token.Ruby, 5 },
            { Token.Onyx, 5 },
            { Token.Gold, 5 }
        };
    }

    /// <summary>
    /// Creates token stacks for a 4-player game (7 each gem, 5 gold).
    /// </summary>
    public static Dictionary<Token, int> FourPlayerStacks()
    {
        return new Dictionary<Token, int>
        {
            { Token.Diamond, 7 },
            { Token.Sapphire, 7 },
            { Token.Emerald, 7 },
            { Token.Ruby, 7 },
            { Token.Onyx, 7 },
            { Token.Gold, 5 }
        };
    }

    /// <summary>
    /// Gets the total count of tokens in a dictionary.
    /// </summary>
    public static int TotalCount(Dictionary<Token, int> tokens)
    {
        return tokens.Values.Sum();
    }

    /// <summary>
    /// Gets the total count of gem tokens (excluding gold) in a dictionary.
    /// </summary>
    public static int GemCount(Dictionary<Token, int> tokens)
    {
        return tokens.Where(kvp => kvp.Key != Token.Gold).Sum(kvp => kvp.Value);
    }
}
