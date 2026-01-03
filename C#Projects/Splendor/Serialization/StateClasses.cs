using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Serialization
{
    /// <summary>
    /// Reconstructable version of GameBoard that allows setting all properties
    /// Used for deserializing persisted game state
    /// </summary>
    public class GameBoardState : IGameBoard
    {
        public DateTime GameStartTimeStamp { get; }
        public int Version { get; private set; }
        public ITurn? LastTurn { get; private set; }

        private List<ITurn> _turns;
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

        public int CurrentPlayer { get; private set; }
        public bool LastRound { get; private set; }
        public bool GameOver { get; private set; }
        public bool IsPaused { get; set; }

        public GameBoardState(
            DateTime gameStartTimeStamp,
            int version,
            ITurn? lastTurn,
            List<ITurn> turns,
            ICardStack cardStackLevel1,
            ICardStack cardStackLevel2,
            ICardStack cardStackLevel3,
            ICard?[] level1Cards,
            ICard?[] level2Cards,
            ICard?[] level3Cards,
            List<IPlayer> players,
            Dictionary<Token, int> tokenStacks,
            List<INoble> nobles,
            int currentPlayer,
            bool lastRound,
            bool gameOver,
            bool isPaused)
        {
            GameStartTimeStamp = gameStartTimeStamp;
            Version = version;
            LastTurn = lastTurn;
            _turns = turns;
            CardStackLevel1 = cardStackLevel1;
            CardStackLevel2 = cardStackLevel2;
            CardStackLevel3 = cardStackLevel3;
            _level1Cards = level1Cards;
            _level2Cards = level2Cards;
            _level3Cards = level3Cards;
            _players = players;
            _tokenStacks = tokenStacks;
            _nobles = nobles;
            CurrentPlayer = currentPlayer;
            LastRound = lastRound;
            GameOver = gameOver;
            IsPaused = isPaused;
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
            // Execute the turn for the player
            ICompletedTurn playersCompletedTurn = _players[CurrentPlayer].ExecuteTurn(turn, _tokenStacks[Token.Gold] > 0);

            if (playersCompletedTurn.Error != null)
            {
                return playersCompletedTurn;
            }

            // If the card is not from reserved cards, replace it from the shop
            if (!_players[CurrentPlayer].ReservedCards.Contains(turn.Card!))
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

    /// <summary>
    /// Reconstructable version of Player that allows setting all properties
    /// </summary>
    public class PlayerState : IPlayer
    {
        public int Id { get; }
        public string Name { get; }

        private Dictionary<Token, int> _tokens;
        public IReadOnlyDictionary<Token, int> Tokens => _tokens;

        private List<ICard> _cards;
        public IReadOnlyList<ICard> Cards => _cards.AsReadOnly();

        private Dictionary<Token, int> _cardTokens;
        public IReadOnlyDictionary<Token, int> CardTokens => _cardTokens;

        private List<INoble> _nobles;
        public IReadOnlyList<INoble> Nobles => _nobles.AsReadOnly();

        private List<ICard> _reservedCards;
        public IReadOnlyList<ICard> ReservedCards => _reservedCards.AsReadOnly();

        public uint PrestigePoints { get; private set; }

        private int MaxTokens { get; } = 10;

        public PlayerState(
            int id,
            string name,
            Dictionary<Token, int> tokens,
            List<ICard> cards,
            Dictionary<Token, int> cardTokens,
            List<INoble> nobles,
            List<ICard> reservedCards,
            uint prestigePoints)
        {
            Id = id;
            Name = name;
            _tokens = tokens;
            _cards = cards;
            _cardTokens = cardTokens;
            _nobles = nobles;
            _reservedCards = reservedCards;
            PrestigePoints = prestigePoints;
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

            // Calculate the sum of tokens being taken
            int sum = 0;
            foreach (int token in takenTokens.Values)
            {
                sum += token;
            }

            // Check if taking tokens would exceed the limit
            if (NumberOfTokens() + sum > MaxTokens)
            {
                return new CompletedTurn(new ContinueAction("Please choose tokens to get rid of", 0), consumedTokens);
            }

            // Validate that player has enough tokens when returning tokens
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

            // Add the acquired tokens to the player's tokens
            foreach (KeyValuePair<Token, int> kvp in takenTokens)
            {
                _tokens[kvp.Key] += kvp.Value;
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

            // Check if taking a gold token would exceed the token limit
            if (NumberOfTokens() + 1 > MaxTokens)
            {
                return new CompletedTurn(new ContinueAction("Please choose tokens to get rid of", 2), consumedTokens);
            }

            // Add the card to reserved cards
            _reservedCards.Add(card);

            // Add gold token if available
            if (canGetGold)
            {
                _tokens[Token.Gold] += 1;
            }

            // Check again after adding gold token
            if (NumberOfTokens() > MaxTokens)
            {
                return new CompletedTurn(new ContinueAction("Please choose tokens to get rid of", 0), consumedTokens);
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

            foreach (KeyValuePair<Token, int> kvp in card.Price)
            {
                if (_tokens[kvp.Key] + _cardTokens[kvp.Key] < kvp.Value)
                {
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

            int Gold = _tokens[Token.Gold];
            foreach (KeyValuePair<Token, int> kvp in card.Price)
            {
                if (_tokens[kvp.Key] + _cardTokens[kvp.Key] < kvp.Value)
                {
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
    }

    /// <summary>
    /// Reconstructable version of Turn that allows setting all properties
    /// </summary>
    public class TurnState : ITurn
    {
        private Dictionary<Token, int>? _takenTokens;
        public IReadOnlyDictionary<Token, int>? TakenTokens => _takenTokens;
        public ICard? Card { get; set; }
        public ICard? ReservedCard { get; set; }
        public INoble? Noble { get; set; }
        public IContinueAction? ContinueAction { get; set; }
        public string? PlayerName { get; set; }
        public DateTime TimeStamp { get; }

        public TurnState(
            Dictionary<Token, int>? takenTokens,
            ICard? card,
            ICard? reservedCard,
            INoble? noble,
            IContinueAction? continueAction,
            string? playerName,
            DateTime timeStamp)
        {
            _takenTokens = takenTokens;
            Card = card;
            ReservedCard = reservedCard;
            Noble = noble;
            ContinueAction = continueAction;
            PlayerName = playerName;
            TimeStamp = timeStamp;
        }
    }
}
