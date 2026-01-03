namespace Splendor.Models.Implementation
{
    public class GameBoard : IGameBoard
    {
        public DateTime GameStartTimeStamp { get; } = DateTime.UtcNow;

        public int Version { get; private set; } = 0;

        public ITurn? LastTurn { get; private set; }

        private List<ITurn> _turns = new List<ITurn>();
        public IReadOnlyList<ITurn> Turns => _turns.AsReadOnly();

        public ICardStack CardStackLevel1 { get; }

        public ICardStack CardStackLevel2 { get; }

        public ICardStack CardStackLevel3 { get; }

        private ICard?[] _level1Cards;
        public IReadOnlyList<ICard?> Level1Cards => _level1Cards;

        private ICard?[] _level2Cards;
        public IReadOnlyList<ICard?> Level2Cards => _level2Cards;

        private ICard?[] _level3Cards;
        public IReadOnlyList<ICard?> Level3Cards => _level3Cards;

        private List<IPlayer> _players;
        public IReadOnlyList<IPlayer> Players => _players.AsReadOnly();

        private Dictionary<Token, int> _tokenStacks;
        public IReadOnlyDictionary<Token, int> TokenStacks => _tokenStacks;

        private List<INoble> _nobles;
        public IReadOnlyList<INoble> Nobles => _nobles.AsReadOnly();

        public int CurrentPlayer { get; private set; } = 0;

        public bool LastRound { get; private set; } = false;

        public bool GameOver { get; private set; } = false;

        public bool IsPaused { get; set; } = false;

        /// <summary>
        /// Initializes the Gameboard
        /// </summary>
        /// <param name="players">The list of players playing the game</param>
        /// <param name="gameDataService">The service for loading game data</param>
        public GameBoard(List<IPlayer> players, Services.Data.IGameDataService gameDataService)
        {
            // Load cards from the data service
            List<ICard> level1Cards = gameDataService.LoadLevel1Cards();
            List<ICard> level2Cards = gameDataService.LoadLevel2Cards();
            List<ICard> level3Cards = gameDataService.LoadLevel3Cards();

            CardStackLevel1 = new CardStack(1, (uint)level1Cards.Count, level1Cards);
            CardStackLevel2 = new CardStack(2, (uint)level2Cards.Count, level2Cards);
            CardStackLevel3 = new CardStack(3, (uint)level3Cards.Count, level3Cards);

            _level1Cards = new ICard?[4] { CardStackLevel1.Draw(), CardStackLevel1.Draw(), CardStackLevel1.Draw(), CardStackLevel1.Draw() };
            _level2Cards = new ICard?[4] { CardStackLevel2.Draw(), CardStackLevel2.Draw(), CardStackLevel2.Draw(), CardStackLevel2.Draw() };
            _level3Cards = new ICard?[4] { CardStackLevel3.Draw(), CardStackLevel3.Draw(), CardStackLevel3.Draw(), CardStackLevel3.Draw() };

            _players = players;

            _tokenStacks = gameDataService.GetTokenConfig(players.Count);

            // Load all nobles and select random ones for this game
            List<INoble> allNobles = gameDataService.LoadNobles();
            _nobles = new List<INoble>();
            Random random = Random.Shared;

            // Choose random nobles for all the players + 1
            for (int i = 0; i < players.Count + 1; i++)
            {
                int rndNum = random.Next(allNobles.Count);
                _nobles.Add(allNobles[rndNum]);
                allNobles.RemoveAt(rndNum);
            }
        }

        

        public ICompletedTurn ExecuteTurn(ITurn turn)
        {
            if (GameOver)
            {
                return new CompletedTurn(new Error("The game has ended", 8));
            }

            // Determine which type of turn this is and delegate to appropriate handler
            if (turn.TakenTokens != null)
            {
                return ValidateAndProcessTokenTaking(turn);
            }

            if (turn.Card != null)
            {
                return ProcessCardPurchase(turn);
            }

            if (turn.ReservedCard != null)
            {
                return ProcessCardReservation(turn);
            }

            if (turn.Noble != null)
            {
                return ProcessNobleAcquisition(turn);
            }

            return new CompletedTurn(new Error("Invalid turn", 11));
        }

        private ICompletedTurn ValidateAndProcessTokenTaking(ITurn turn)
        {
            // Validate token taking rules
            Dictionary<Token, int> positiveTakenTokens = new Dictionary<Token, int>();
            foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens!)
            {
                if (kvp.Value > 0)
                {
                    positiveTakenTokens.Add(kvp.Key, kvp.Value);
                }
            }

            if (positiveTakenTokens.Count > GameConstants.MaxTokenTypes)
            {
                return new CompletedTurn(new Error("You can't take more than 3 types of token", 4));
            }

            if (turn.TakenTokens.ContainsKey(Token.Gold))
            {
                return new CompletedTurn(new Error("You can't take gold tokens", 6));
            }

            // Validate based on number of token types taken
            if (turn.TakenTokens.Count > 1)
            {
                foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens)
                {
                    if (_tokenStacks[kvp.Key] < kvp.Value)
                    {
                        return new CompletedTurn(new Error("Not enough tokens to take", 0));
                    }
                    if (kvp.Value > 1)
                    {
                        return new CompletedTurn(new Error("You can only take 3 tokens of different token types", 0));
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens)
                {
                    if (kvp.Value > 2)
                    {
                        return new CompletedTurn(new Error("You can't take more than 2 tokens of the same token type", 0));
                    }

                    if (kvp.Value == 2 && _tokenStacks[kvp.Key] < GameConstants.MinTokensRequiredForDoubleAction)
                    {
                        return new CompletedTurn(new Error("You must leave at least 2 tokens in the stack when taking 2", 0));
                    }
                }
            }

            // Execute the turn for the player
            ICompletedTurn playersCompletedTurn = _players[CurrentPlayer].ExecuteTurn(turn, _tokenStacks[Token.Gold] > 0);

            if (playersCompletedTurn.Error != null || playersCompletedTurn.ContinueAction != null)
            {
                return playersCompletedTurn;
            }

            // Subtract the tokens from the stacks
            foreach (KeyValuePair<Token, int> kvp in turn.TakenTokens)
            {
                _tokenStacks[kvp.Key] -= kvp.Value;
            }

            return FinalizeTurn(turn);
        }

        private ICompletedTurn ProcessCardPurchase(ITurn turn)
        {
            // Check if the card was in reserved cards BEFORE executing the turn
            bool wasReservedCard = _players[CurrentPlayer].ReservedCards.Contains(turn.Card!);

            // Execute the turn for the player
            ICompletedTurn playersCompletedTurn = _players[CurrentPlayer].ExecuteTurn(turn, _tokenStacks[Token.Gold] > 0);

            if (playersCompletedTurn.Error != null)
            {
                return playersCompletedTurn;
            }

            // If the card is not from reserved cards, replace it from the shop
            if (!wasReservedCard)
            {
                ReplaceCardFromStack(turn.Card!);
            }

            // Return consumed tokens to the stacks
            if (playersCompletedTurn.ConsumedTokens != null)
            {
                foreach (KeyValuePair<Token, int> kvp in playersCompletedTurn.ConsumedTokens)
                {
                    _tokenStacks[kvp.Key] += kvp.Value;
                }
            }

            return FinalizeTurn(turn);
        }

        private ICompletedTurn ProcessCardReservation(ITurn turn)
        {
            // Only execute player turn if we haven't already (when taking tokens at the same time)
            if (turn.TakenTokens == null)
            {
                ICompletedTurn playersCompletedTurn = _players[CurrentPlayer].ExecuteTurn(turn, _tokenStacks[Token.Gold] > 0);

                if (playersCompletedTurn.Error != null || playersCompletedTurn.ContinueAction != null)
                {
                    return playersCompletedTurn;
                }
            }

            // Give gold token if available
            if (_tokenStacks[Token.Gold] > 0)
            {
                _tokenStacks[Token.Gold]--;
            }

            // Replace the reserved card from the shop
            ReplaceCardFromStack(turn.ReservedCard!);

            return FinalizeTurn(turn);
        }

        private ICompletedTurn ProcessNobleAcquisition(ITurn turn)
        {
            // Execute the turn for the player
            ICompletedTurn playersCompletedTurn = _players[CurrentPlayer].ExecuteTurn(turn, _tokenStacks[Token.Gold] > 0);

            if (playersCompletedTurn.Error != null)
            {
                return playersCompletedTurn;
            }

            // Remove the noble from the available nobles
            _nobles.Remove(turn.Noble!);

            return FinalizeTurn(turn);
        }

        private ICompletedTurn? CheckNobleAcquisition(ITurn turn)
        {
            // Check if the player can acquire any nobles
            List<INoble> availableNobles = new List<INoble>();
            foreach (INoble noble in _nobles)
            {
                if (_players[CurrentPlayer].CanAcquireNoble(noble))
                {
                    availableNobles.Add(noble);
                }
            }

            if (availableNobles.Count > 0)
            {
                turn.PlayerName = _players[CurrentPlayer].Name;
                _turns.Insert(0, turn);
                LastTurn = turn;
                LastTurn.ContinueAction = new ContinueAction("You can acquire nobles", 1, availableNobles);
                return new CompletedTurn(new ContinueAction("You can acquire nobles", 1, availableNobles), null);
            }

            return null;
        }

        private ICompletedTurn FinalizeTurn(ITurn turn)
        {
            // Check for noble acquisition only if player didn't already acquire one this turn
            if (turn.Noble == null)
            {
                ICompletedTurn? nobleCheck = CheckNobleAcquisition(turn);
                if (nobleCheck != null)
                {
                    return nobleCheck;
                }
            }

            // Check if the player has reached winning points
            if (_players[CurrentPlayer].PrestigePoints >= GameConstants.WinningPrestigePoints)
            {
                LastRound = true;
            }

            Version++;
            turn.PlayerName = _players[CurrentPlayer].Name;
            _turns.Insert(0, turn);
            LastTurn = turn;

            // Check for game over
            if (CurrentPlayer == _players.Count - 1 && LastRound)
            {
                GameOver = true;
                return new CompletedTurn(true);
            }

            AdvanceToNextPlayer();
            return new CompletedTurn();
        }

        private void AdvanceToNextPlayer()
        {
            CurrentPlayer++;
            if (CurrentPlayer >= _players.Count)
            {
                CurrentPlayer -= _players.Count;
            }
        }

        private void ReplaceCardFromStack(ICard card)
        {
            switch (card.Level)
            {
                case 1:
                    _level1Cards[Array.IndexOf(_level1Cards, card)] = CardStackLevel1.Draw();
                    break;
                case 2:
                    _level2Cards[Array.IndexOf(_level2Cards, card)] = CardStackLevel2.Draw();
                    break;
                case 3:
                    _level3Cards[Array.IndexOf(_level3Cards, card)] = CardStackLevel3.Draw();
                    break;
            }
        }


    }
}
