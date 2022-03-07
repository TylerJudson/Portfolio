namespace Splendor.Models.Implementation
{
    public class Player : IPlayer
    {
        /// <summary>
        /// The maximum number of tokens a player can have
        /// </summary>
        private int MaxTokens { get; } = 10;

        public int Id { get; }

        public string Name { get; }

        public Dictionary<Token, int> Tokens { get; } = new Dictionary<Token, int>() { { Token.Emerald, 0 }, { Token.Diamond, 0 }, { Token.Sapphire, 0 }, { Token.Onyx, 0 }, {Token.Ruby, 0 }, { Token.Gold, 0 } };    

        public List<ICard> Cards { get; } = new List<ICard>();

        public Dictionary<Token, int> CardTokens { get; } = new Dictionary<Token, int>() { { Token.Emerald, 0 }, { Token.Diamond, 0 }, { Token.Sapphire, 0 }, { Token.Onyx, 0 }, { Token.Ruby, 0 } };

        public List<INoble> Nobles { get; } = new List<INoble>();

        public List<ICard> ReservedCards { get; } = new List<ICard>();

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
            Dictionary<Token, int> ConsumedTokens = new Dictionary<Token, int>();

            // If we acquired tokens -> add them to our Tokens
            if (turn.TakenTokens != null) // Check to make sure there are tokens to add
            {
                // Get the sum of the tokens we are about to get
                int sum = 0;
                foreach (int token in turn.TakenTokens.Values)
                {
                    sum += token;
                }

                // If we have too many tokens return a continue action
                if (NumberOfTokens() + sum > MaxTokens)
                {
                    return new CompletedTurn(new ContinueAction("Please choose tokens to get rid of", 0), ConsumedTokens);
                }

                // Check to make sure that if the player is returning tokens that the player has enough
                foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens)
                {
                    if (kvp.Value < 0)
                    {
                        if (Tokens[kvp.Key] < -kvp.Value)
                        {
                            return new CompletedTurn(new Error("You don't have enough tokens to return", 7));
                        }
                    }
                }

                // Add the acquired tokens to the players tokens
                foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens) 
                {
                    Tokens[kvp.Key] += kvp.Value;
                }
            }
            // If we purchase a card -> add the card to our Cards and balance the price
            if (turn.Card != null) // Check to make sure there is a card to add
            {
                // Check to make sure we have enough tokens to purchase card
                if (!CanPurchaseCardWithGold(turn.Card) && !CanPurchaseCard(turn.Card))
                {
                    // If we have too little tokens return an error
                    return new CompletedTurn(new Error("You don't have enough tokens for this card", 2));
                }

                // now we can Balance the price

                // Loop through and subtract the price from our tokens
                foreach (KeyValuePair<Token, int> kvp in turn.Card.Price)
                {
                    // If we have enough cards don't do anything
                    if (kvp.Value - CardTokens[kvp.Key] > 0)
                    {
                        // Set cost to the amount of tokens needed after applying cards
                        int cost = kvp.Value - CardTokens[kvp.Key];

                        // If we consumed more tokens than we have take from the gold tokens
                        if (cost > Tokens[kvp.Key])
                        {
                            // Subtract the difference of the amount consumed and what we actually have from gold
                            Tokens[Token.Gold] -= cost - Tokens[kvp.Key];

                            // Add gold to the dictionary of consumed tokens
                            if (!ConsumedTokens.ContainsKey(Token.Gold))
                            {
                                ConsumedTokens.Add(Token.Gold, cost - Tokens[kvp.Key]); 
                            }
                            else
                            {
                                ConsumedTokens[Token.Gold] += cost - Tokens[kvp.Key];
                            }

                            // modify the cost based on how many gold tokens used
                            cost = Tokens[kvp.Key];
                        }
                        
                        // Subtract what was consumed from what we have
                        Tokens[kvp.Key] -= cost;

                        // Add the amount to consumed tokens
                        ConsumedTokens.Add(kvp.Key, cost);

                    }
                }

                // Add the card to our cards
                Cards.Add(turn.Card);
                CardTokens[turn.Card.Type] += 1;

                // Add the prestige points to our points
                PrestigePoints += turn.Card.PrestigePoints;
            }
            // If we reserved a card -> add the card to our reserved cards and add a golden token to our tokens
            if (turn.ReservedCard != null)
            {
                // Check to make sure we don't have more than three reserved cards
                if (ReservedCards.Count >= 3)
                {
                    return new CompletedTurn(new Error("You can only have 3 reserved cards at a time", 3));
                }

                // If we have too many tokens return a continue action -- add 1 for the gold token
                if (NumberOfTokens() + 1 > MaxTokens)
                {
                    return new CompletedTurn(new ContinueAction("Please choose tokens to get rid of", 2), ConsumedTokens);
                }

                // Add the ReservedCard to our reserved Cards
                ReservedCards.Add(turn.ReservedCard);
                
                if (canGetGold)
                {
                    // Add a golden token
                    Tokens[Token.Gold] += 1;
                }

                // Check to make sure we don't have more than the max tokens
                if (NumberOfTokens() > MaxTokens)
                {
                    return new CompletedTurn(new ContinueAction("Please choose tokens to get rid of", 0), ConsumedTokens);
                }
            }


            // If we aquired a noble -> add it to our nobles
            if (turn.Noble != null)
            {
                // Check to see if we have enough cards for the noble
                if (!CanAcquireNoble(turn.Noble))
                {
                    return new CompletedTurn(new Error("You do not have enough cards for this noble.", 5));
                }

                // Adds the noble to our nobles
                Nobles.Add(turn.Noble);

                // Adds the prestige points
                PrestigePoints += turn.Noble.PrestigePoints;
                
            }


            return new CompletedTurn(ConsumedTokens);
        }
        
        public bool CanAcquireNoble(INoble noble)
        {
            foreach (KeyValuePair<Token, int> kvp in noble.Criteria)
            {
                if (CardTokens[kvp.Key] < kvp.Value)
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
                if (Tokens[kvp.Key] + CardTokens[kvp.Key] < kvp.Value)
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

            int Gold = Tokens[Token.Gold];
            // Check to make sure we have enough tokens to purchase card
            foreach (KeyValuePair<Token, int> kvp in card.Price)
            {
                // If we have too little tokens
                if (Tokens[kvp.Key] + CardTokens[kvp.Key] < kvp.Value)
                {
                    // If we have too little tokens with gold return an error
                    if (Tokens[kvp.Key] + CardTokens[kvp.Key] + Gold < kvp.Value)
                    {
                        return false;
                    }
                    else
                    {
                        Gold -= kvp.Value - Tokens[kvp.Key] - CardTokens[kvp.Key];
                    }
                }

            }




            return true;
        }

        public int NumberOfTokens()
        {
            int ret = 0;
            foreach (int count in Tokens.Values)
            {
                ret += count;
            }
            return ret;
        }
    }
}
