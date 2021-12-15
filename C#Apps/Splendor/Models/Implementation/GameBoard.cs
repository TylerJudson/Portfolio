namespace Splendor.Models.Implementation
{
    public class GameBoard : IGameBoard
    {
        public int Version { get; private set; } = 0;

        public ITurn LastTurn { get; private set; }

        public ICardStack CardStackLevel1 { get; }

        public ICardStack CardStackLevel2 { get; }

        public ICardStack CardStackLevel3 { get; }

        public ICard[] Level1Cards { get; }

        public ICard[] Level2Cards { get; }

        public ICard[] Level3Cards { get; }

        public List<IPlayer> Players { get; }

        public Dictionary<Token, int> TokenStacks { get; private set; }

        public List<INoble> Nobles { get; }

        public int CurrentPlayer { get; private set; } = 0;

        /// <summary>
        /// Initializes the Gameboard
        /// </summary>
        /// <param name="players">The list of players playing the game</param>
        public GameBoard(List<IPlayer> players)
        {
            CardStackLevel1 = InitializeCardStackLevel1();
            CardStackLevel2 = InitializeCardStackLevel2();
            CardStackLevel3 = InitializeCardStackLevel3();


            Level1Cards = new ICard[4] { CardStackLevel1.Draw(), CardStackLevel1.Draw(), CardStackLevel1.Draw(), CardStackLevel1.Draw() };
            Level2Cards = new ICard[4] { CardStackLevel2.Draw(), CardStackLevel2.Draw(), CardStackLevel2.Draw(), CardStackLevel2.Draw() };
            Level3Cards = new ICard[4] { CardStackLevel3.Draw(), CardStackLevel3.Draw(), CardStackLevel3.Draw(), CardStackLevel3.Draw() };

            Players = players;

            TokenStacks = InitializeTokenStacks(players.Count);

            Nobles = InitializeNobles(players.Count);
        }

        

        public ICompletedTurn ExecuteTurn(ITurn turn)
        {
            
            // If the player aquired tokens -> subtract them from the stacks
            if (turn.TakenTokens != null)
            {
                // Validate that the player hasn't taken more tokens than allowed

                // Check to make sure the player has not taken more than 3 types of token
                if (turn.TakenTokens.Count > 3)
                {
                    return new CompletedTurn(new Error("You can't take more than 3 types of token", 4));
                }
                // If the player has taken more than one token type -> Check to make sure the player hasn't taken too many tokens
                else if (turn.TakenTokens.Count > 1)
                {
                    foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens)
                    {
                        // Check to make sure there is enough tokens in the stack to take
                        if (TokenStacks[kvp.Key] < kvp.Value)
                        {
                            return new CompletedTurn(new Error("Not enough tokens to take", 0));
                        }
                        // Check to make sure the player has only taken 1 of each
                        if (kvp.Value > 1)
                        {
                            return new CompletedTurn(new Error("You can only take 3 tokens of different token types", 0));
                        }
                    }
                }
                // If the player took from 1 token type -> check to make sure the player hasn't taken too many tokens
                else
                {
                    foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens)
                    {
                        // Check to make sure the player didn't take more than 2
                        if (kvp.Value > 2)
                        {
                            return new CompletedTurn(new Error("You can't take more than 2 tokens of the same token type", 0));
                        }

                        // If the player took two make sure there is at least 4 in the stack before they take
                        if (kvp.Value == 2 && TokenStacks[kvp.Key] < 4)
                        {
                            return new CompletedTurn(new Error("You must leave at least 2 tokens in the stack when taking 2", 0));
                        }
                    }
                }

                // Now we can subtract the tokens from the stacks

                // Loop through and subtract the tokens taken from each stack
                foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens)
                {
                    TokenStacks[kvp.Key] -= kvp.Value;
                }

                // Execute the turn for the player
                ICompletedTurn PlayersCompletedTurn = Players[CurrentPlayer].ExecuteTurn(turn);

                // If the player's turn threw an error return
                if (PlayersCompletedTurn.Error != null || PlayersCompletedTurn.ContinueAction != null)
                {
                    return PlayersCompletedTurn;
                }

            }
            // if the player purchased a card -> remove the card and draw a new one
            else if (turn.Card != null)
            {

                // Execute the turn for the player
                ICompletedTurn PlayersCompletedTurn = Players[CurrentPlayer].ExecuteTurn(turn);

                // If the player's turn threw an error return
                if (PlayersCompletedTurn.Error != null)
                {
                    return PlayersCompletedTurn;
                }
                
                // If the card is on the reserved cards
                if (Players[CurrentPlayer].ReservedCards.Contains(turn.Card))
                {
                    Players[CurrentPlayer].ReservedCards.Remove(turn.Card);
                }
                // IF the card is in the shop
                else
                {
                    // Remove the card and draw a new one
                    switch (turn.Card.Level)
                    {
                        case 1:
                            Level1Cards[Array.IndexOf(Level1Cards, turn.Card)] = CardStackLevel1.Draw();
                            break;
                        case 2:
                            Level2Cards[Array.IndexOf(Level2Cards, turn.Card)] = CardStackLevel2.Draw();
                            break;
                        case 3:
                            Level3Cards[Array.IndexOf(Level3Cards, turn.Card)] = CardStackLevel3.Draw();
                            break;
                    }
                }
                

                // Return the tokens to their stacks
                foreach(KeyValuePair<Token, int> kvp in turn.Card.Price)
                {
                    TokenStacks[kvp.Key] += (kvp.Value - Players[CurrentPlayer].CardTokens[kvp.Key]) < 0 ? 0 : kvp.Value - Players[CurrentPlayer].CardTokens[kvp.Key];
                }
            }
            // if the player reserved a card -> remove 1 gold, remove the card, and draw a new one 
            else if (turn.ReservedCard != null)
            {
                // Check to make sure there is enough gold
                if (TokenStacks[Token.Gold] == 0)
                {
                    return new CompletedTurn(new Error("Not enough tokens to take", 0));
                }


                // Execute the turn for the player
                ICompletedTurn PlayersCompletedTurn = Players[CurrentPlayer].ExecuteTurn(turn);

                // If the player's turn threw an error return that error
                if (PlayersCompletedTurn.Error != null)
                {
                    return PlayersCompletedTurn;
                }

                // Remove one gold
                TokenStacks[Token.Gold]--;

                // Remove the card from the gameboard and draw a new one
                switch (turn.ReservedCard.Level)
                {
                    case 1:
                        Level1Cards[Array.IndexOf(Level1Cards, turn.ReservedCard)] = CardStackLevel1.Draw();
                        break;
                    case 2:
                        Level2Cards[Array.IndexOf(Level2Cards, turn.ReservedCard)] = CardStackLevel2.Draw();
                        break;
                    case 3:
                        Level3Cards[Array.IndexOf(Level3Cards, turn.ReservedCard)] = CardStackLevel3.Draw();
                        break;
                }

                if (PlayersCompletedTurn.ContinueAction != null)
                {
                    return PlayersCompletedTurn;
                }
            }

            // If the player acquired a noble -> remove the noble
            if (turn.Noble != null)
            {  
                // Execute the turn for the player
                ICompletedTurn PlayersCompletedTurn = Players[CurrentPlayer].ExecuteTurn(turn);

                // If the player's turn threw an error return that error
                if (PlayersCompletedTurn.Error != null)
                {
                    return PlayersCompletedTurn;
                }

                // remove the noble from the nobles
                Nobles.Remove(turn.Noble);
            }


            foreach (INoble noble in Nobles)
            {
                if (Players[CurrentPlayer].CanAcquireNoble(noble))
                {
                    return new CompletedTurn(new ContinueAction("You can acquire a noble", 1));
                }
            }


            CurrentPlayer++;
            if (CurrentPlayer >= Players.Count)
            {
                CurrentPlayer -= Players.Count;
            }
            Version++;
            LastTurn = turn;
            return new CompletedTurn();
        }

        /// <summary>
        /// Initializes the card stack for the level 1 cards
        /// </summary>
        /// <returns>A list of cards in level 1</returns>
        private static CardStack InitializeCardStackLevel1()
        {
            return new CardStack(
                1,
                40,
                new List<ICard> { 
                    new Card(1, Token.Diamond, 0, new Dictionary<Token, int>() { { Token.Sapphire, 1 }, { Token.Emerald, 1 }, { Token.Ruby, 1 }, { Token.Onyx, 1 } }, "Level1-D-0P-1S-1E-1R-1O.jpg"),
                    new Card(1, Token.Diamond, 0, new Dictionary<Token, int>() { { Token.Sapphire, 1 }, { Token.Emerald, 2 }, { Token.Ruby, 1 }, { Token.Onyx, 1 } }, "Level1-D-0P-1S-2E-1R-1O.jpg"),
                    new Card(1, Token.Diamond, 0, new Dictionary<Token, int>() { { Token.Ruby, 2 }, { Token.Onyx, 1 } }, "Level1-D-0P-2R-1O.jpg"),
                    new Card(1, Token.Diamond, 0, new Dictionary<Token, int>() { { Token.Sapphire, 2 }, { Token.Emerald, 2 }, { Token.Onyx, 1 } }, "Level1-D-0P-2S-2E-1O.jpg"),
                    new Card(1, Token.Diamond, 0, new Dictionary<Token, int>() { { Token.Sapphire, 2 }, { Token.Onyx, 2 } }, "Level1-D-0P-2S-2O.jpg"),
                    new Card(1, Token.Diamond, 0, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Sapphire, 1 }, { Token.Onyx, 1 } }, "Level1-D-0P-3D-1S-1O.jpg"),
                    new Card(1, Token.Diamond, 0, new Dictionary<Token, int>() { { Token.Sapphire, 3 } }, "Level1-D-0P-3S.jpg"),
                    new Card(1, Token.Diamond, 1, new Dictionary<Token, int>() { { Token.Emerald, 4 } }, "Level1-D-1P-4E.jpg"),
                    new Card(1, Token.Emerald, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Sapphire, 1 }, { Token.Ruby, 1 }, { Token.Onyx, 1 } }, "Level1-E-0P-1D-1S-1R-1O.jpg"),
                    new Card(1, Token.Emerald, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Sapphire, 1 }, { Token.Ruby, 1 }, { Token.Onyx, 2 } }, "Level1-E-0P-1D-1S-1R-2O.jpg"),
                    new Card(1, Token.Emerald, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Sapphire, 3 }, { Token.Emerald, 1 } }, "Level1-E-0P-1D-3S-1E.jpg"),
                    new Card(1, Token.Emerald, 0, new Dictionary<Token, int>() { { Token.Sapphire, 1 }, { Token.Ruby, 2 }, { Token.Onyx, 2 } }, "Level1-E-0P-1S-2R-2O.jpg"),
                    new Card(1, Token.Emerald, 0, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Sapphire, 1 } }, "Level1-E-0P-2D-1S.jpg"),
                    new Card(1, Token.Emerald, 0, new Dictionary<Token, int>() { { Token.Sapphire, 2 }, { Token.Ruby, 2 } }, "Level1-E-0P-2S-2R.jpg"),
                    new Card(1, Token.Emerald, 0, new Dictionary<Token, int>() { { Token.Ruby, 3 } }, "Level1-E-0P-3R.jpg"),
                    new Card(1, Token.Emerald, 1, new Dictionary<Token, int>() { { Token.Onyx, 4 } }, "Level1-E-1P-4O.jpg"),
                    new Card(1, Token.Onyx, 0, new Dictionary<Token, int>() { { Token.Diamond, 1}, { Token.Sapphire, 1 }, { Token.Emerald, 1 }, { Token.Ruby, 1 } }, "Level1-O-0P-1D-1S-1E-1R.jpg"),
                    new Card(1, Token.Onyx, 0, new Dictionary<Token, int>() { { Token.Diamond, 1}, { Token.Sapphire, 2 }, { Token.Emerald, 1 }, { Token.Ruby, 1 } }, "Level1-O-0P-1D-2S-1E-1R.jpg"),
                    new Card(1, Token.Onyx, 0, new Dictionary<Token, int>() { { Token.Emerald, 1}, { Token.Ruby, 3 }, { Token.Onyx, 1 } }, "Level1-O-0P-1E-3R-1O.jpg"),
                    new Card(1, Token.Onyx, 0, new Dictionary<Token, int>() { { Token.Diamond, 2}, { Token.Emerald, 2 } }, "Level1-O-0P-2D-2E.jpg"),
                    new Card(1, Token.Onyx, 0, new Dictionary<Token, int>() { { Token.Diamond, 2}, { Token.Sapphire, 2 }, { Token.Ruby, 1 } }, "Level1-O-0P-2D-2S-1R.jpg"),
                    new Card(1, Token.Onyx, 0, new Dictionary<Token, int>() { { Token.Emerald, 2}, { Token.Ruby, 1 } }, "Level1-O-0P-2E-1R.jpg"),
                    new Card(1, Token.Onyx, 0, new Dictionary<Token, int>() { { Token.Emerald, 3} }, "Level1-O-0P-3E.jpg"),
                    new Card(1, Token.Onyx, 1, new Dictionary<Token, int>() { { Token.Sapphire, 4} }, "Level1-O-1P-4S.jpg"),
                    new Card(1, Token.Ruby, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Ruby, 1 }, { Token.Onyx, 3 } }, "Level1-R-0P-1D-1R-3O.jpg"),
                    new Card(1, Token.Ruby, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Sapphire, 1 }, { Token.Emerald, 1 }, { Token.Onyx, 1 } }, "Level1-R-0P-1D-1S-1E-1O.jpg"),
                    new Card(1, Token.Ruby, 0, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Emerald, 1 }, { Token.Onyx, 2 } }, "Level1-R-0P-2D-1E-2O.jpg"),
                    new Card(1, Token.Ruby, 0, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Sapphire, 1 }, { Token.Emerald, 1 }, { Token.Onyx, 1 } }, "Level1-R-0P-2D-1S-1E-1O.jpg"),
                    new Card(1, Token.Ruby, 0, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Ruby, 2 } }, "Level1-R-0P-2D-2R.jpg"),
                    new Card(1, Token.Ruby, 0, new Dictionary<Token, int>() { { Token.Sapphire, 2 }, { Token.Emerald, 1 } }, "Level1-R-0P-2S-1E.jpg"),
                    new Card(1, Token.Ruby, 0, new Dictionary<Token, int>() { { Token.Diamond, 3 } }, "Level1-R-0P-3D.jpg"),
                    new Card(1, Token.Ruby, 1, new Dictionary<Token, int>() { { Token.Diamond, 4 } }, "Level1-R-1P-4D.jpg"),
                    new Card(1, Token.Sapphire, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Emerald, 1 }, { Token.Ruby, 1 }, { Token.Onyx, 1 } }, "Level1-S-0P-1D-1E-1R-1O.jpg"),
                    new Card(1, Token.Sapphire, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Emerald, 1 }, { Token.Ruby, 2 }, { Token.Onyx, 1 } }, "Level1-S-0P-1D-1E-2R-1O.jpg"),
                    new Card(1, Token.Sapphire, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Emerald, 2 }, { Token.Ruby, 2 } }, "Level1-S-0P-1D-2E-2R.jpg"),
                    new Card(1, Token.Sapphire, 0, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Onyx, 2 } }, "Level1-S-0P-1D-2O.jpg"),
                    new Card(1, Token.Sapphire, 0, new Dictionary<Token, int>() { { Token.Sapphire, 1 }, { Token.Emerald, 3 }, { Token.Ruby, 1 } }, "Level1-S-0P-1S-3E-1R.jpg"),
                    new Card(1, Token.Sapphire, 0, new Dictionary<Token, int>() { { Token.Emerald, 2 }, { Token.Onyx, 2 } }, "Level1-S-0P-2E-2O.jpg"),
                    new Card(1, Token.Sapphire, 0, new Dictionary<Token, int>() { { Token.Onyx, 3 } }, "Level1-S-0P-3O.jpg"),
                    new Card(1, Token.Sapphire, 1, new Dictionary<Token, int>() { { Token.Ruby, 4 } }, "Level1-S-1P-4R.jpg"),
                }
            );
        }

        /// <summary>
        /// Initializes the card stack for the level 2 cards
        /// </summary>
        /// <returns>A list of cards in level 2</returns>
        private static CardStack InitializeCardStackLevel2()
        {
            return new CardStack(
                2,
                30,
                new List<ICard> {
                    new Card(2, Token.Diamond, 1, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Sapphire, 3 }, { Token.Ruby, 3 } }, "Level2-D-1P-2D-3S-3R.jpg"),
                    new Card(2, Token.Diamond, 1, new Dictionary<Token, int>() { { Token.Emerald, 3 }, { Token.Ruby, 2 }, { Token.Onyx, 2 } }, "Level2-D-1P-3E-2R-2O.jpg"),
                    new Card(2, Token.Diamond, 2, new Dictionary<Token, int>() { { Token.Emerald, 1 }, { Token.Ruby, 4 }, { Token.Onyx, 2 } }, "Level2-D-2P-1E-4R-2O.jpg"),
                    new Card(2, Token.Diamond, 2, new Dictionary<Token, int>() { { Token.Ruby, 5 } }, "Level2-D-2P-5R.jpg"),
                    new Card(2, Token.Diamond, 2, new Dictionary<Token, int>() { { Token.Ruby, 5 }, { Token.Onyx, 3 } }, "Level2-D-2P-5R-3O.jpg"),
                    new Card(2, Token.Diamond, 3, new Dictionary<Token, int>() { { Token.Diamond, 6 } }, "Level2-D-3P-6D.jpg"),
                    new Card(2, Token.Emerald, 1, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Sapphire, 3 }, { Token.Onyx, 2 } }, "Level2-E-1P-2D-3S-2O.jpg"),
                    new Card(2, Token.Emerald, 1, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Emerald, 2 }, { Token.Ruby, 3 } }, "Level2-E-1P-3D-2E-3R.jpg"),
                    new Card(2, Token.Emerald, 2, new Dictionary<Token, int>() { { Token.Diamond, 4 }, { Token.Sapphire, 2 }, { Token.Onyx, 1 } }, "Level2-E-2P-4D-2S-1O.jpg"),
                    new Card(2, Token.Emerald, 2, new Dictionary<Token, int>() { { Token.Emerald, 5 } }, "Level2-E-2P-5E.jpg"),
                    new Card(2, Token.Emerald, 2, new Dictionary<Token, int>() { { Token.Sapphire, 5 }, { Token.Emerald, 3 } }, "Level2-E-2P-5S-3E.jpg"),
                    new Card(2, Token.Emerald, 3, new Dictionary<Token, int>() { { Token.Emerald, 6 } }, "Level2-E-3P-6E.jpg"),
                    new Card(2, Token.Onyx, 1, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Sapphire, 2 }, { Token.Emerald, 2 } }, "Level2-O-1P-3D-2S-2E.jpg"),
                    new Card(2, Token.Onyx, 1, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Emerald, 3 }, { Token.Onyx, 2 } }, "Level2-O-1P-3D-3E-2O.jpg"),
                    new Card(2, Token.Onyx, 2, new Dictionary<Token, int>() { { Token.Sapphire, 1 }, { Token.Emerald, 4 }, { Token.Ruby, 2 } }, "Level2-O-2P-1S-4E-2R.jpg"),
                    new Card(2, Token.Onyx, 2, new Dictionary<Token, int>() { { Token.Diamond, 5 } }, "Level2-O-2P-5D.jpg"),
                    new Card(2, Token.Onyx, 2, new Dictionary<Token, int>() { { Token.Emerald, 5 }, { Token.Ruby, 3 } }, "Level2-O-2P-5E-3R.jpg"),
                    new Card(2, Token.Onyx, 3, new Dictionary<Token, int>() { { Token.Onyx, 6 } }, "Level2-O-3P-6O.jpg"),
                    new Card(2, Token.Ruby, 1, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Ruby, 2 }, { Token.Onyx, 3 } }, "Level2-R-1P-2D-2R-3O.jpg"),
                    new Card(2, Token.Ruby, 1, new Dictionary<Token, int>() { { Token.Sapphire, 3 }, { Token.Ruby, 2 }, { Token.Onyx, 3 } }, "Level2-R-1P-3S-2R-3O.jpg"),
                    new Card(2, Token.Ruby, 2, new Dictionary<Token, int>() { { Token.Diamond, 1 }, { Token.Sapphire, 4 }, { Token.Emerald, 2 } }, "Level2-R-2P-1D-4S-2E.jpg"),
                    new Card(2, Token.Ruby, 2, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Onyx, 5 } }, "Level2-R-2P-3D-5O.jpg"),
                    new Card(2, Token.Ruby, 2, new Dictionary<Token, int>() { { Token.Onyx, 5 } }, "Level2-R-2P-5O.jpg"),
                    new Card(2, Token.Ruby, 3, new Dictionary<Token, int>() { { Token.Ruby, 6 } }, "Level2-R-3P-6R.jpg"),
                    new Card(2, Token.Sapphire, 1, new Dictionary<Token, int>() { { Token.Sapphire, 2 }, { Token.Emerald, 2 }, { Token.Ruby, 3 } }, "Level2-S-1P-2S-2E-3R.jpg"),
                    new Card(2, Token.Sapphire, 1, new Dictionary<Token, int>() { { Token.Sapphire, 2 }, { Token.Emerald, 3 }, { Token.Onyx, 3 } }, "Level2-S-1P-2S-3E-3O.jpg"),
                    new Card(2, Token.Sapphire, 2, new Dictionary<Token, int>() { { Token.Diamond, 2 }, { Token.Ruby, 1 }, { Token.Onyx, 4 } }, "Level2-S-2P-2D-1R-4O.jpg"),
                    new Card(2, Token.Sapphire, 2, new Dictionary<Token, int>() { { Token.Diamond, 5 }, { Token.Sapphire, 3 } }, "Level2-S-2P-5D-3S.jpg"),
                    new Card(2, Token.Sapphire, 2, new Dictionary<Token, int>() { { Token.Sapphire, 5 } }, "Level2-S-2P-5S.jpg"),
                    new Card(2, Token.Sapphire, 3, new Dictionary<Token, int>() { { Token.Sapphire, 6 } }, "Level2-S-3P-6S.jpg")
                }
            );
        }

        /// <summary>
        /// Initializes the card stack for the level 3 cards
        /// </summary>
        /// <returns>A list of cards in level 3</returns>
        private static CardStack InitializeCardStackLevel3()
        {
            return new CardStack(
                3,
                20,
                new List<ICard> {
                    new Card(3, Token.Diamond, 3, new Dictionary<Token, int>() { { Token.Sapphire, 3 }, { Token.Emerald, 3 }, { Token.Ruby, 5 }, { Token.Onyx, 3 } }, "Level3-D-3P-3S-3E-5R-3O.jpg"),
                    new Card(3, Token.Diamond, 4, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Ruby, 3 }, { Token.Onyx, 6 } }, "Level3-D-4P-3D-3R-6O.jpg"),
                    new Card(3, Token.Diamond, 4, new Dictionary<Token, int>() { { Token.Onyx, 7 } }, "Level3-D-4P-7O.jpg"),
                    new Card(3, Token.Diamond, 5, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Onyx, 7 } }, "Level3-D-5P-3D-7O.jpg"),
                    new Card(3, Token.Emerald, 3, new Dictionary<Token, int>() { { Token.Diamond, 5 }, { Token.Sapphire, 3 }, { Token.Ruby, 3 }, { Token.Onyx, 3 } }, "Level3-E-3P-5D-3S-3R-3O.jpg"),
                    new Card(3, Token.Emerald, 4, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Sapphire, 6 }, { Token.Emerald, 3 } }, "Level3-E-4P-3D-6S-3E.jpg"),
                    new Card(3, Token.Emerald, 4, new Dictionary<Token, int>() { { Token.Sapphire, 7 } }, "Level3-E-4P-7S.jpg"),
                    new Card(3, Token.Emerald, 5, new Dictionary<Token, int>() { { Token.Sapphire, 7 }, { Token.Emerald, 3 } }, "Level3-E-5P-7S-3E.jpg"),
                    new Card(3, Token.Onyx, 3, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Sapphire, 3 }, { Token.Emerald, 5 }, { Token.Ruby, 3 } }, "Level3-O-3P-3D-3S-5E-3R.jpg"),
                    new Card(3, Token.Onyx, 4, new Dictionary<Token, int>() { { Token.Emerald, 3 }, { Token.Ruby, 6 }, { Token.Onyx, 3 } }, "Level3-O-4P-3E-6R-3O.jpg"),
                    new Card(3, Token.Onyx, 4, new Dictionary<Token, int>() { { Token.Ruby, 7 } }, "Level3-O-4P-7R.jpg"),
                    new Card(3, Token.Onyx, 5, new Dictionary<Token, int>() { { Token.Ruby, 7 }, { Token.Onyx, 3 } }, "Level3-O-5P-7R-3O.jpg"),
                    new Card(3, Token.Ruby, 3, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Sapphire, 5 }, { Token.Emerald, 3 }, { Token.Onyx, 3 } }, "Level3-R-3P-3D-5S-3E-3O.jpg"),
                    new Card(3, Token.Ruby, 4, new Dictionary<Token, int>() { { Token.Sapphire, 3 }, { Token.Emerald, 6 }, { Token.Ruby, 3 } }, "Level3-R-4P-3S-6E-3R.jpg"),
                    new Card(3, Token.Ruby, 4, new Dictionary<Token, int>() { { Token.Emerald, 7 } }, "Level3-R-4P-7E.jpg"),
                    new Card(3, Token.Ruby, 5, new Dictionary<Token, int>() { { Token.Emerald, 7 }, { Token.Ruby, 3 } }, "Level3-R-5P-7E-3R.jpg"),
                    new Card(3, Token.Sapphire, 3, new Dictionary<Token, int>() { { Token.Diamond, 3 }, { Token.Emerald, 3 }, { Token.Ruby, 3 }, { Token.Onyx, 5 } }, "Level3-S-3P-3D-3E-3R-5O.jpg"),
                    new Card(3, Token.Sapphire, 4, new Dictionary<Token, int>() { { Token.Diamond, 6 }, { Token.Sapphire, 3 }, { Token.Onyx, 3 } }, "Level3-S-4P-6D-3S-3O.jpg"),
                    new Card(3, Token.Sapphire, 4, new Dictionary<Token, int>() { { Token.Diamond, 7 } }, "Level3-S-4P-7D.jpg"),
                    new Card(3, Token.Sapphire, 5, new Dictionary<Token, int>() { { Token.Diamond, 7 }, { Token.Sapphire, 3 } }, "Level3-S-5P-7D-3S.jpg"),
                }
            );
        }

        /// <summary>
        /// Initializes all the nobles
        /// </summary>
        /// <param name="numberOfPlayers">The number of players playing the game</param>
        /// <returns>A list of nobles</returns>
        private static List<INoble> InitializeNobles(int numberOfPlayers)
        {
            List<INoble> nobles = new List<INoble>() {
                new Noble(new Dictionary<Token, int>() { { Token.Emerald, 3 }, {Token.Sapphire, 3 }, {Token.Diamond, 3 } }, "Noble-3E-3S-3D.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Emerald, 3 }, {Token.Sapphire, 3 }, {Token.Ruby, 3 } }, "Noble-3E-3S-3R.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Onyx, 3 }, {Token.Ruby, 3 }, {Token.Diamond, 3 } }, "Noble-3O-3R-3D.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Onyx, 3 }, {Token.Ruby, 3 }, {Token.Emerald, 3 } }, "Noble-3O-3R-3E.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Onyx, 3 }, {Token.Sapphire, 3 }, {Token.Diamond, 3 } }, "Noble-3O-3S-3D.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Onyx, 4}, {Token.Diamond, 4 } }, "Noble-4O-4D.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Onyx, 4}, {Token.Ruby, 4 } }, "Noble-4O-4R.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Ruby, 4}, {Token.Emerald, 4 } }, "Noble-4R-4E.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Sapphire, 4}, {Token.Diamond, 4 } }, "Noble-4S-4D.jpg"),
                new Noble(new Dictionary<Token, int>() { { Token.Sapphire, 4}, {Token.Emerald, 4 } }, "Noble-4S-4E.jpg")
            };

            Random random = new Random();
            List<INoble> ret = new List<INoble>();

            // Choose random nobles for all the players + 1
            for (int i = 0; i < numberOfPlayers + 1; i++)
            {
                int rndNum = random.Next(nobles.Count);
                ret.Add(nobles[rndNum]);
                nobles.RemoveAt(rndNum);
            }

            return ret;
        }

        /// <summary>
        /// Initializes the token stacks
        /// </summary>
        /// <param name="numberOfPlayers">The number of players playing the game</param>
        /// <returns>The stacks of tokens</returns>
        private static Dictionary<Token, int> InitializeTokenStacks(int numberOfPlayers)
        {
            if (numberOfPlayers == 2)
            {
                return new Dictionary<Token, int>() { { Token.Emerald, 4}, { Token.Diamond, 4}, { Token.Sapphire, 4 }, { Token.Onyx, 4 }, { Token.Ruby, 4 }, { Token.Gold, 5 } };
            }
            else if (numberOfPlayers == 3)
            {
                return new Dictionary<Token, int>() { { Token.Emerald, 5 }, { Token.Diamond, 5 }, { Token.Sapphire, 5 }, { Token.Onyx, 5 }, { Token.Ruby, 5 }, { Token.Gold, 5 } };
            }
            else
            {
                return new Dictionary<Token, int>() { { Token.Emerald, 7 }, { Token.Diamond, 7 }, { Token.Sapphire, 7 }, { Token.Onyx, 7 }, { Token.Ruby, 7 }, { Token.Gold, 5 } };

            }
        }

       
    }
}
