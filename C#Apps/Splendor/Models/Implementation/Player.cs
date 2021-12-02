namespace Splendor.Models.Implementation
{
    public class Player : IPlayer
    {
        /// <summary>
        /// The maximum number of tokens a player can have
        /// </summary>
        private int MaxTokens { get; } = 10;

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
        public Player(string name)
        {
            Name = name;
        }

        public ICompletedTurn ExecuteTurn(ITurn turn)
        {
            // If we acquired tokens -> add them to our Tokens
            if (turn.TakenTokens != null) // Check to make sure there are tokens to add
            {
                // Add the acquired tokens to the players tokens
                foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens) 
                {
                    Tokens[kvp.Key] += kvp.Value;
                }
                // If we have too many tokens return a continue action
                if (NumberOfTokens() > MaxTokens)
                {
                    return new CompletedTurn( new ContinueAction("Please choose tokens to get rid of", 0));
                }
            }
            // If we purchase a card -> add the card to our Cards and balance the price
            else if (turn.Card != null) // Check to make sure there is a card to add
            {
                // Check to make sure we have enough tokens to purchase card
                foreach (KeyValuePair<Token, int> kvp in turn.Card.Price)
                {
                    // If we have too little tokens return an error
                    if (Tokens[kvp.Key] + CardTokens[kvp.Key] < kvp.Value)
                    {
                        return new CompletedTurn(new Error("You don't have enough tokens for this card", 2));
                    }
                }

                // now we can Balance the price

                // Loop through and subtract the price from our tokens
                foreach (KeyValuePair<Token, int> kvp in turn.Card.Price)
                {
                    // If we have enough cards don't do anything
                    if (kvp.Value - CardTokens[kvp.Key] > 0)
                    {
                        // Subtract from the tokens after subtraction from the cards
                        Tokens[kvp.Key] -= (kvp.Value - CardTokens[kvp.Key]);
                    }
                }

                // Add the card to our cards
                Cards.Add(turn.Card);
                CardTokens[turn.Card.Type] += 1;

                // Add the prestige points to our points
                PrestigePoints += turn.Card.PrestigePoints;
            }
            // If we reserved a card -> add the card to our reserved cards and add a golden token to our tokens
            else if (turn.ReservedCard != null)
            {
                // Check to make sure we don't have more than three reserved cards
                if (ReservedCards.Count > 3)
                {
                    return new CompletedTurn(new Error("You can only have 3 reserved cards at a time", 3));
                }
                // Add the ReservedCard to our reserved Cards
                ReservedCards.Add(turn.ReservedCard);
                
                // Add a golden token
                Tokens[Token.Gold] += 1;

                // Check to make sure we don't have more than the max tokens
                if (NumberOfTokens() > MaxTokens)
                {
                    return new CompletedTurn(new ContinueAction("Please choose tokens to get rid of", 0));
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


            return new CompletedTurn();
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

        public int NumberOfTokens()
        {
            int ret = 0;
            foreach (int count in Tokens.Values)
            {
                ret += count;
            }
            return ret;
        }

       











        public void Test()
        {
            ReservedCards.Add(new Card(1, Token.Emerald, 2, new Dictionary<Token, int>() { { Token.Ruby, 1 } }, "Level1-R-1P-4D.png"));
            ReservedCards.Add(new Card(1, Token.Emerald, 2, new Dictionary<Token, int>() { { Token.Ruby, 1 } }, "Level2-O-2P-5E-3R.png"));
            Tokens[Token.Emerald] += 1;

            CardTokens[Token.Diamond] += 1;

            PrestigePoints += 10;

        }
    }
}
