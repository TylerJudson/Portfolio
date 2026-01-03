using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Tests.TestUtilities.Builders;

/// <summary>
/// Fluent builder and static factories for creating test Turn instances.
/// </summary>
public class TurnBuilder
{
    private Dictionary<Token, int>? _takenTokens;
    private ICard? _card;
    private ICard? _reservedCard;
    private INoble? _noble;

    public TurnBuilder WithTakenTokens(Dictionary<Token, int> tokens)
    {
        _takenTokens = tokens;
        return this;
    }

    public TurnBuilder WithCard(ICard card)
    {
        _card = card;
        return this;
    }

    public TurnBuilder WithReservedCard(ICard card)
    {
        _reservedCard = card;
        return this;
    }

    public TurnBuilder WithNoble(INoble noble)
    {
        _noble = noble;
        return this;
    }

    public Turn Build()
    {
        if (_noble != null)
        {
            return new Turn(_noble);
        }
        if (_card != null)
        {
            return new Turn(_card, isReserve: false);
        }
        if (_reservedCard != null)
        {
            return new Turn(_takenTokens, _reservedCard);
        }
        return new Turn(_takenTokens);
    }

    // Static factory methods for common turn types

    /// <summary>
    /// Creates a turn taking 3 different tokens.
    /// </summary>
    public static Turn TakeThreeDifferentTokens(Token t1, Token t2, Token t3)
    {
        return new Turn(new Dictionary<Token, int>
        {
            { t1, 1 },
            { t2, 1 },
            { t3, 1 }
        });
    }

    /// <summary>
    /// Creates a turn taking 2 of the same token.
    /// </summary>
    public static Turn TakeTwoSameTokens(Token token)
    {
        return new Turn(new Dictionary<Token, int>
        {
            { token, 2 }
        });
    }

    /// <summary>
    /// Creates a turn taking 1 token.
    /// </summary>
    public static Turn TakeOneToken(Token token)
    {
        return new Turn(new Dictionary<Token, int>
        {
            { token, 1 }
        });
    }

    /// <summary>
    /// Creates a turn purchasing a card.
    /// </summary>
    public static Turn PurchaseCard(ICard card)
    {
        return new Turn(card, isReserve: false);
    }

    /// <summary>
    /// Creates a turn reserving a card (from visible cards).
    /// </summary>
    public static Turn ReserveCard(ICard card)
    {
        return new Turn(card, isReserve: true);
    }

    /// <summary>
    /// Creates a turn acquiring a noble.
    /// </summary>
    public static Turn AcquireNoble(INoble noble)
    {
        return new Turn(noble);
    }

    /// <summary>
    /// Creates a turn returning tokens (negative values).
    /// </summary>
    public static Turn ReturnTokens(Dictionary<Token, int> tokensToReturn)
    {
        var negativeTokens = tokensToReturn.ToDictionary(
            kvp => kvp.Key,
            kvp => -kvp.Value
        );
        return new Turn(negativeTokens);
    }

    /// <summary>
    /// Creates a turn taking tokens that would exceed the 10-token limit.
    /// </summary>
    public static Turn TakeTokensExceedingLimit()
    {
        return new Turn(new Dictionary<Token, int>
        {
            { Token.Diamond, 1 },
            { Token.Sapphire, 1 },
            { Token.Emerald, 1 }
        });
    }

    /// <summary>
    /// Creates a turn taking 4 different token types (invalid).
    /// </summary>
    public static Turn TakeFourDifferentTokens()
    {
        return new Turn(new Dictionary<Token, int>
        {
            { Token.Diamond, 1 },
            { Token.Sapphire, 1 },
            { Token.Emerald, 1 },
            { Token.Ruby, 1 }
        });
    }

    /// <summary>
    /// Creates a turn attempting to take gold (invalid).
    /// </summary>
    public static Turn TakeGold()
    {
        return new Turn(new Dictionary<Token, int>
        {
            { Token.Gold, 1 }
        });
    }

    /// <summary>
    /// Creates a turn taking 3 of the same token (invalid).
    /// </summary>
    public static Turn TakeThreeSameTokens(Token token)
    {
        return new Turn(new Dictionary<Token, int>
        {
            { token, 3 }
        });
    }
}
