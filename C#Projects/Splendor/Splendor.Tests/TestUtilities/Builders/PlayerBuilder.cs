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
    /// Note: Since Player has private setters, we use reflection to set the private backing fields.
    /// </summary>
    public Player Build()
    {
        var player = new Player(_name, _id);

        // Use reflection to access private backing fields
        var playerType = typeof(Player);

        // Apply tokens if specified
        if (_tokens != null)
        {
            var tokensField = playerType.GetField("_tokens", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tokensDict = tokensField?.GetValue(player) as Dictionary<Token, int>;
            if (tokensDict != null)
            {
                foreach (var kvp in _tokens)
                {
                    tokensDict[kvp.Key] = kvp.Value;
                }
            }
        }

        // Apply cards
        if (_cards.Count > 0)
        {
            var cardsField = playerType.GetField("_cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cardsList = cardsField?.GetValue(player) as List<ICard>;
            var cardTokensField = playerType.GetField("_cardTokens", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cardTokensDict = cardTokensField?.GetValue(player) as Dictionary<Token, int>;
            var prestigeProperty = playerType.GetProperty("PrestigePoints");
            var currentPrestige = (uint)(prestigeProperty?.GetValue(player) ?? 0u);

            foreach (var card in _cards)
            {
                cardsList?.Add(card);
                if (cardTokensDict != null)
                {
                    cardTokensDict[card.Type] += 1;
                }
                currentPrestige += card.PrestigePoints;
            }

            // Update prestige points
            prestigeProperty?.SetValue(player, currentPrestige);
        }

        // Apply reserved cards
        if (_reservedCards.Count > 0)
        {
            var reservedCardsField = playerType.GetField("_reservedCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var reservedCardsList = reservedCardsField?.GetValue(player) as List<ICard>;
            foreach (var card in _reservedCards)
            {
                reservedCardsList?.Add(card);
            }
        }

        // Apply nobles
        if (_nobles.Count > 0)
        {
            var noblesField = playerType.GetField("_nobles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var noblesList = noblesField?.GetValue(player) as List<INoble>;
            var prestigeProperty = playerType.GetProperty("PrestigePoints");
            var currentPrestige = (uint)(prestigeProperty?.GetValue(player) ?? 0u);

            foreach (var noble in _nobles)
            {
                noblesList?.Add(noble);
                currentPrestige += noble.PrestigePoints;
            }

            // Update prestige points
            prestigeProperty?.SetValue(player, currentPrestige);
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
