using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Tests.TestUtilities.Builders;

/// <summary>
/// Fluent builder for creating test Player instances with controlled state.
/// </summary>
public class PlayerBuilder
{
    private string _name = "TestPlayer";
    private int _id = 0;
    private Dictionary<Token, int>? _tokens;
    private List<ICard> _cards = new();
    private List<ICard> _reservedCards = new();
    private List<INoble> _nobles = new();

    public PlayerBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PlayerBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public PlayerBuilder WithTokens(Dictionary<Token, int> tokens)
    {
        _tokens = tokens;
        return this;
    }

    public PlayerBuilder WithTokens(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0, int gold = 0)
    {
        _tokens = new Dictionary<Token, int>
        {
            { Token.Diamond, diamond },
            { Token.Sapphire, sapphire },
            { Token.Emerald, emerald },
            { Token.Ruby, ruby },
            { Token.Onyx, onyx },
            { Token.Gold, gold }
        };
        return this;
    }

    public PlayerBuilder WithCard(ICard card)
    {
        _cards.Add(card);
        return this;
    }

    public PlayerBuilder WithCards(params ICard[] cards)
    {
        _cards.AddRange(cards);
        return this;
    }

    public PlayerBuilder WithReservedCard(ICard card)
    {
        _reservedCards.Add(card);
        return this;
    }

    public PlayerBuilder WithReservedCards(params ICard[] cards)
    {
        _reservedCards.AddRange(cards);
        return this;
    }

    public PlayerBuilder WithNoble(INoble noble)
    {
        _nobles.Add(noble);
        return this;
    }

    public PlayerBuilder WithNobles(params INoble[] nobles)
    {
        _nobles.AddRange(nobles);
        return this;
    }

    /// <summary>
    /// Sets the player to have exactly 10 tokens (the maximum).
    /// </summary>
    public PlayerBuilder AtMaxTokens()
    {
        _tokens = new Dictionary<Token, int>
        {
            { Token.Diamond, 2 },
            { Token.Sapphire, 2 },
            { Token.Emerald, 2 },
            { Token.Ruby, 2 },
            { Token.Onyx, 2 },
            { Token.Gold, 0 }
        };
        return this;
    }

    /// <summary>
    /// Sets the player to have 3 reserved cards (the maximum).
    /// </summary>
    public PlayerBuilder WithMaxReservedCards()
    {
        _reservedCards = new List<ICard>
        {
            CardBuilder.CheapLevel1Diamond(),
            CardBuilder.CheapLevel1Diamond(),
            CardBuilder.CheapLevel1Diamond()
        };
        return this;
    }

    /// <summary>
    /// Builds the player and applies the configured state.
    /// Note: Since Player has private setters, we need to use ExecuteTurn to set state.
    /// For testing purposes, we'll use reflection or create players via valid turns.
    /// </summary>
    public Player Build()
    {
        var player = new Player(_name, _id);

        // Apply tokens if specified
        if (_tokens != null)
        {
            foreach (var kvp in _tokens)
            {
                player.Tokens[kvp.Key] = kvp.Value;
            }
        }

        // Apply cards
        foreach (var card in _cards)
        {
            player.Cards.Add(card);
            player.CardTokens[card.Type] += 1;
        }

        // Apply reserved cards
        foreach (var card in _reservedCards)
        {
            player.ReservedCards.Add(card);
        }

        // Apply nobles
        foreach (var noble in _nobles)
        {
            player.Nobles.Add(noble);
        }

        return player;
    }

    // Static factory methods

    /// <summary>
    /// Creates a default player with no tokens or cards.
    /// </summary>
    public static Player Default()
    {
        return new PlayerBuilder().Build();
    }

    /// <summary>
    /// Creates a player with specified tokens ready to purchase cards.
    /// </summary>
    public static Player CreateWithTokens(int diamond = 0, int sapphire = 0, int emerald = 0, int ruby = 0, int onyx = 0, int gold = 0)
    {
        return new PlayerBuilder()
            .WithTokens(diamond, sapphire, emerald, ruby, onyx, gold)
            .Build();
    }

    /// <summary>
    /// Creates a player with enough cards to acquire a noble requiring 3 of each of 3 types.
    /// </summary>
    public static Player ReadyForThreeOfThreeNoble()
    {
        return new PlayerBuilder()
            .WithCards(
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Diamond).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Sapphire).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build(),
                new CardBuilder().WithType(Token.Emerald).Build()
            )
            .Build();
    }
}
