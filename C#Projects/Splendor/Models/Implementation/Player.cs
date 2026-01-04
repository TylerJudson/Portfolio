namespace Splendor.Models.Implementation
{
    public class Player : IPlayer
    {
        /// <summary>
        /// The maximum number of tokens a player can have
        /// </summary>
        private int MaxTokens { get; } = GameConstants.MaxTokensPerPlayer;

        public int Id { get; }

        public string Name { get; }

        private Dictionary<Token, int> _tokens = new Dictionary<Token, int>() { { Token.Emerald, 0 }, { Token.Diamond, 0 }, { Token.Sapphire, 0 }, { Token.Onyx, 0 }, {Token.Ruby, 0 }, { Token.Gold, 0 } };
        public IReadOnlyDictionary<Token, int> Tokens => _tokens;

        private List<ICard> _cards = new List<ICard>();
        public IReadOnlyList<ICard> Cards => _cards.AsReadOnly();

        private Dictionary<Token, int> _cardTokens = new Dictionary<Token, int>() { { Token.Emerald, 0 }, { Token.Diamond, 0 }, { Token.Sapphire, 0 }, { Token.Onyx, 0 }, { Token.Ruby, 0 } };
        public IReadOnlyDictionary<Token, int> CardTokens => _cardTokens;

        private List<INoble> _nobles = new List<INoble>();
        public IReadOnlyList<INoble> Nobles => _nobles.AsReadOnly();

        private List<ICard> _reservedCards = new List<ICard>();
        public IReadOnlyList<ICard> ReservedCards => _reservedCards.AsReadOnly();

        public uint PrestigePoints { get; private set; } = 0;

        /// <summary>
        /// Initializes the name of the player
        /// </summary>
        /// <param name="name">The name of the player</param>
        public Player(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public ICompletedTurn ExecuteTurn(ITurn turn, bool canGetGold)
        {
            Dictionary<Token, int> consumedTokens = new Dictionary<Token, int>();

            if (turn.TakenTokens != null)
            {
                return TakeTokens(turn.TakenTokens);
            }

            if (turn.Card != null)
            {
                return PurchaseCard(turn.Card, consumedTokens);
            }

            if (turn.ReservedCard != null)
            {
                return ReserveCard(turn.ReservedCard, canGetGold);
            }

            if (turn.Noble != null)
            {
                return AcquireNoble(turn.Noble);
            }

            return new CompletedTurn(consumedTokens);
        }

        private ICompletedTurn TakeTokens(IReadOnlyDictionary<Token, int> takenTokens)
        {
            Dictionary<Token, int> consumedTokens = new Dictionary<Token, int>();

            // Calculate the sum of tokens being taken/returned
            int sum = 0;
            foreach (int token in takenTokens.Values)
            {
                sum += token;
            }

            // Different logic for TAKING vs RETURNING tokens
            bool isTaking = sum > 0;
            bool isReturning = sum < 0;

            if (isTaking)
            {
                // For TAKING tokens: Check limit BEFORE adding tokens
                if (NumberOfTokens() + sum > MaxTokens)
                {
                    int tokensToReturn = (NumberOfTokens() + sum) - MaxTokens;
                    return new CompletedTurn(new ContinueAction($"You must return {tokensToReturn} token{(tokensToReturn > 1 ? "s" : "")}", 0), consumedTokens);
                }
            }

            if (isReturning)
            {
                // For RETURNING tokens: Validate player has enough BEFORE removing
                foreach (KeyValuePair<Token, int> kvp in takenTokens)
                {
                    if (kvp.Value < 0)
                    {
                        if (_tokens[kvp.Key] < -kvp.Value)
                        {
                            return new CompletedTurn(new Error("You don't have enough tokens to return", 7));
                        }
                    }
                }
            }

            // Apply the token changes
            foreach (KeyValuePair<Token, int> kvp in takenTokens)
            {
                _tokens[kvp.Key] += kvp.Value;
            }

            if (isReturning)
            {
                // For RETURNING: Check if player STILL exceeds limit after returning
                int finalTokenCount = NumberOfTokens();
                if (finalTokenCount > MaxTokens)
                {
                    int tokensToReturn = finalTokenCount - MaxTokens;
                    return new CompletedTurn(new ContinueAction($"You must return {tokensToReturn} token{(tokensToReturn > 1 ? "s" : "")}", 0), consumedTokens);
                }
            }

            // Validate final token count doesn't exceed maximum (for taking tokens case)
            int totalTokens = NumberOfTokens();
            if (totalTokens > MaxTokens)
            {
                // Rollback the token changes
                foreach (KeyValuePair<Token, int> kvp in takenTokens)
                {
                    _tokens[kvp.Key] -= kvp.Value;
                }
                return new CompletedTurn(new Error($"You must return enough tokens to have exactly {MaxTokens}. You would have {totalTokens}.", 7));
            }

            return new CompletedTurn(consumedTokens);
        }

        private ICompletedTurn PurchaseCard(ICard card, Dictionary<Token, int> consumedTokens)
        {
            // Validate the player can afford the card
            if (!CanPurchaseCardWithGold(card) && !CanPurchaseCard(card))
            {
                return new CompletedTurn(new Error("You don't have enough tokens for this card", 2));
            }

            // Calculate and consume tokens for the purchase
            foreach (KeyValuePair<Token, int> kvp in card.Price)
            {
                // Only process if we need more tokens beyond what our cards provide
                if (kvp.Value - _cardTokens[kvp.Key] > 0)
                {
                    int cost = kvp.Value - _cardTokens[kvp.Key];

                    // Use gold tokens if we don't have enough of this type
                    if (cost > _tokens[kvp.Key])
                    {
                        int goldNeeded = cost - _tokens[kvp.Key];
                        _tokens[Token.Gold] -= goldNeeded;

                        if (!consumedTokens.ContainsKey(Token.Gold))
                        {
                            consumedTokens.Add(Token.Gold, goldNeeded);
                        }
                        else
                        {
                            consumedTokens[Token.Gold] += goldNeeded;
                        }

                        cost = _tokens[kvp.Key];
                    }

                    // Consume the tokens
                    _tokens[kvp.Key] -= cost;
                    consumedTokens.Add(kvp.Key, cost);
                }
            }

            // Remove from reserved cards if applicable
            if (_reservedCards.Contains(card))
            {
                _reservedCards.Remove(card);
            }

            // Add the card to the player's collection
            _cards.Add(card);
            _cardTokens[card.Type] += 1;
            PrestigePoints += card.PrestigePoints;

            return new CompletedTurn(consumedTokens);
        }

        private ICompletedTurn ReserveCard(ICard card, bool canGetGold)
        {
            Dictionary<Token, int> consumedTokens = new Dictionary<Token, int>();

            // Check if player has room for another reserved card
            if (_reservedCards.Count >= GameConstants.MaxReservedCards)
            {
                return new CompletedTurn(new Error("You can only have 3 reserved cards at a time", 3));
            }

            // Add the card to reserved cards
            _reservedCards.Add(card);

            // Add gold token if available
            if (canGetGold)
            {
                _tokens[Token.Gold] += 1;
            }

            // Check if player now exceeds token limit (after adding card and gold)
            if (NumberOfTokens() > MaxTokens)
            {
                int tokensToReturn = NumberOfTokens() - MaxTokens;
                return new CompletedTurn(new ContinueAction($"You must return {tokensToReturn} token{(tokensToReturn > 1 ? "s" : "")}", 0), consumedTokens);
            }

            return new CompletedTurn(consumedTokens);
        }

        private ICompletedTurn AcquireNoble(INoble noble)
        {
            Dictionary<Token, int> consumedTokens = new Dictionary<Token, int>();

            // Validate the player has the required cards for this noble
            if (!CanAcquireNoble(noble))
            {
                return new CompletedTurn(new Error("You do not have enough cards for this noble.", 5));
            }

            // Add the noble and prestige points
            _nobles.Add(noble);
            PrestigePoints += noble.PrestigePoints;

            return new CompletedTurn(consumedTokens);
        }
        
        public bool CanAcquireNoble(INoble noble)
        {
            foreach (KeyValuePair<Token, int> kvp in noble.Criteria)
            {
                if (_cardTokens[kvp.Key] < kvp.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanPurchaseCard(ICard card)
        {
            if (card == null)
            {
                return false;
            }

            // Check to make sure we have enough tokens to purchase card
            foreach (KeyValuePair<Token, int> kvp in card.Price)
            {
                // If we have too little tokens
                if (_tokens[kvp.Key] + _cardTokens[kvp.Key] < kvp.Value)
                {
                    // If we have too little tokens with return false
                    return false;
                }
                
            }
            return true;
        }

        public bool CanPurchaseCardWithGold(ICard card)
        {
            if (card == null)
            {
                return false;
            }
            // TODO - Implement Gold Tokens and for user to choose what tokens and cards to purchase the card with

            int Gold = _tokens[Token.Gold];
            // Check to make sure we have enough tokens to purchase card
            foreach (KeyValuePair<Token, int> kvp in card.Price)
            {
                // If we have too little tokens
                if (_tokens[kvp.Key] + _cardTokens[kvp.Key] < kvp.Value)
                {
                    // If we have too little tokens with gold return an error
                    if (_tokens[kvp.Key] + _cardTokens[kvp.Key] + Gold < kvp.Value)
                    {
                        return false;
                    }
                    else
                    {
                        Gold -= kvp.Value - _tokens[kvp.Key] - _cardTokens[kvp.Key];
                    }
                }

            }




            return true;
        }

        public int NumberOfTokens()
        {
            int ret = 0;
            foreach (int count in _tokens.Values)
            {
                ret += count;
            }
            return ret;
        }

        public void RemoveTokens(Token type, int count)
        {
            _tokens[type] = Math.Max(0, _tokens[type] - count);
        }

        public void RemoveReservedCard(ICard card)
        {
            _reservedCards.Remove(card);
        }
    }
}
