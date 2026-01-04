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

            // Validate that only ONE action type is specified (cannot combine actions)
            // Note: Token returns (all negative values) don't count as an action since they complete a previous action
            int actionCount = 0;

            // Only count token taking as an action if there are POSITIVE tokens (taking, not just returning)
            if (turn.TakenTokens != null && turn.TakenTokens.Any(kvp => kvp.Value > 0))
            {
                actionCount++;
            }

            if (turn.Card != null) actionCount++;
            if (turn.ReservedCard != null || turn.ReserveDeckLevel > 0) actionCount++;
            if (turn.Noble != null) actionCount++;

            if (actionCount > 1)
            {
                return new CompletedTurn(new Error("Cannot perform multiple actions in one turn", 11));
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

            if (turn.ReservedCard != null || turn.ReserveDeckLevel > 0)
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

            // Check that there's at least some token activity (taking or returning, not all zeros)
            if (positiveTakenTokens.Count == 0 && !turn.TakenTokens.Any(kvp => kvp.Value < 0))
            {
                return new CompletedTurn(new Error("You must specify tokens to take or return", 11));
            }

            if (positiveTakenTokens.Count > GameConstants.MaxTokenTypes)
            {
                return new CompletedTurn(new Error("You can't take more than 3 types of token", 4));
            }

            if (positiveTakenTokens.ContainsKey(Token.Gold))
            {
                return new CompletedTurn(new Error("You can't take gold tokens", 6));
            }

            // Validate based on number of token types taken (only count positive values)
            if (positiveTakenTokens.Count > 1)
            {
                foreach (KeyValuePair<Token, int> kvp in positiveTakenTokens)
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
                foreach (KeyValuePair<Token, int> kvp in positiveTakenTokens)
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

            if (playersCompletedTurn.Error != null)
            {
                return playersCompletedTurn;
            }

            if (playersCompletedTurn.ContinueAction != null)
            {
                // Increment version so frontend knows to refresh state
                Version++;
                turn.PlayerName = _players[CurrentPlayer].Name;
                _turns.Insert(0, turn);
                LastTurn = turn;
                LastTurn.ContinueAction = playersCompletedTurn.ContinueAction;
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

            // If not a reserved card, validate it's on the board
            if (!wasReservedCard)
            {
                bool cardOnBoard = turn.Card!.Level switch
                {
                    1 => _level1Cards.Contains(turn.Card),
                    2 => _level2Cards.Contains(turn.Card),
                    3 => _level3Cards.Contains(turn.Card),
                    _ => false
                };

                if (!cardOnBoard)
                {
                    return new CompletedTurn(new Error("Card is not available for purchase", 11));
                }
            }

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
            ICard? cardToReserve = null;
            ICompletedTurn? playersCompletedTurn = null;

            // Handle blind deck reservation
            if (turn.ReserveDeckLevel > 0)
            {
                // Draw from the specified deck
                ICard? drawnCard = turn.ReserveDeckLevel switch
                {
                    1 => CardStackLevel1.Draw(),
                    2 => CardStackLevel2.Draw(),
                    3 => CardStackLevel3.Draw(),
                    _ => null
                };

                if (drawnCard == null)
                {
                    return new CompletedTurn(new Error("Cannot reserve from empty deck", 9));
                }

                cardToReserve = drawnCard;

                // Create a temporary turn with the drawn card for player execution
                var tempTurn = new Turn(drawnCard, isReserve: true);
                playersCompletedTurn = _players[CurrentPlayer].ExecuteTurn(tempTurn, _tokenStacks[Token.Gold] > 0);

                // Return immediately if there's an error
                if (playersCompletedTurn.Error != null)
                {
                    return playersCompletedTurn;
                }
            }
            else
            {
                // Normal reservation from visible cards
                cardToReserve = turn.ReservedCard;

                // Validate the card is on the board
                bool cardOnBoard = turn.ReservedCard!.Level switch
                {
                    1 => _level1Cards.Contains(turn.ReservedCard),
                    2 => _level2Cards.Contains(turn.ReservedCard),
                    3 => _level3Cards.Contains(turn.ReservedCard),
                    _ => false
                };

                if (!cardOnBoard)
                {
                    return new CompletedTurn(new Error("Card is not available for reservation", 11));
                }

                // Only execute player turn if we haven't already (when taking tokens at the same time)
                if (turn.TakenTokens == null)
                {
                    playersCompletedTurn = _players[CurrentPlayer].ExecuteTurn(turn, _tokenStacks[Token.Gold] > 0);

                    // Return immediately if there's an error
                    if (playersCompletedTurn.Error != null)
                    {
                        return playersCompletedTurn;
                    }
                }

                // Replace the reserved card from the shop
                ReplaceCardFromStack(turn.ReservedCard!);
            }

            // Remove gold token from board if available (even if continue action is needed)
            if (_tokenStacks[Token.Gold] > 0)
            {
                _tokenStacks[Token.Gold]--;
            }

            // If continue action is needed (player must return tokens), increment version but don't advance turn
            if (playersCompletedTurn?.ContinueAction != null)
            {
                Version++;
                turn.PlayerName = _players[CurrentPlayer].Name;
                _turns.Insert(0, turn);
                LastTurn = turn;
                LastTurn.ContinueAction = playersCompletedTurn.ContinueAction;
                return playersCompletedTurn;
            }

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

        public IPlayer? GetWinner()
        {
            // Only return a winner if the game is over
            if (!GameOver)
            {
                return null;
            }

            // Find player(s) with highest prestige
            uint maxPrestige = _players.Max(p => p.PrestigePoints);
            var topPlayers = _players.Where(p => p.PrestigePoints == maxPrestige).ToList();

            // If only one player has the max prestige, they win
            if (topPlayers.Count == 1)
            {
                return topPlayers[0];
            }

            // Tie-breaker: player with fewest development cards wins
            return topPlayers.OrderBy(p => p.Cards.Count).First();
        }

        public bool CancelPendingTurn()
        {
            // Check if there's a pending continue action to cancel
            if (LastTurn?.ContinueAction == null)
            {
                return false;
            }

            IPlayer currentPlayer = _players[CurrentPlayer];

            // If this was a card reservation, we need to reverse the state changes
            if (LastTurn.ReservedCard != null || LastTurn.ReserveDeckLevel > 0)
            {
                // Find the reserved card - it should be the last one added
                ICard? reservedCard = currentPlayer.ReservedCards.LastOrDefault();

                if (reservedCard != null)
                {
                    // Remove the card from player's reserved cards
                    currentPlayer.RemoveReservedCard(reservedCard);

                    // Return gold token to board (if player received one)
                    // Check if gold was given by looking at whether board had gold when reservation happened
                    // The gold was taken from board in ProcessCardReservation, so we return it
                    currentPlayer.RemoveTokens(Token.Gold, 1);
                    _tokenStacks[Token.Gold]++;

                    // Put the card back on the board in an empty slot at its level
                    RestoreCardToBoard(reservedCard);
                }
            }

            // Token taking: No state to reverse - tokens weren't added to player yet
            // (Player.TakeTokens returns ContinueAction BEFORE adding tokens)

            // Remove the turn from the list
            if (_turns.Count > 0 && _turns[0] == LastTurn)
            {
                _turns.RemoveAt(0);
            }

            // Update LastTurn to previous turn (or null if no turns left)
            LastTurn = _turns.Count > 0 ? _turns[0] : null;

            // Increment version so clients refresh
            Version++;

            return true;
        }

        private void RestoreCardToBoard(ICard card)
        {
            ICard?[] levelCards = card.Level switch
            {
                1 => _level1Cards,
                2 => _level2Cards,
                3 => _level3Cards,
                _ => _level1Cards
            };

            // Find an empty slot to put the card back
            for (int i = 0; i < levelCards.Length; i++)
            {
                if (levelCards[i] == null)
                {
                    levelCards[i] = card;
                    return;
                }
            }

            // If no empty slot, replace the first card (shouldn't happen normally)
            levelCards[0] = card;
        }

    }
}
