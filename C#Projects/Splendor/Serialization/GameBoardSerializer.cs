using System.Text.Json;
using Splendor.Models;
using Splendor.Models.Implementation;

namespace Splendor.Serialization
{
    /// <summary>
    /// Handles serialization and deserialization of IGameBoard to/from JSON
    /// Converts between interface types and concrete implementations
    /// </summary>
    public static class GameBoardSerializer
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Serializes an IGameBoard to JSON string
        /// </summary>
        public static string Serialize(IGameBoard gameBoard)
        {
            var dto = ToDto(gameBoard);
            return JsonSerializer.Serialize(dto, JsonOptions);
        }

        /// <summary>
        /// Deserializes JSON string to IGameBoard
        /// </summary>
        public static IGameBoard Deserialize(string json)
        {
            var dto = JsonSerializer.Deserialize<GameBoardDto>(json, JsonOptions);
            if (dto == null)
            {
                throw new InvalidOperationException("Failed to deserialize GameBoard JSON");
            }
            return FromDto(dto);
        }

        /// <summary>
        /// Converts IGameBoard to DTO
        /// </summary>
        private static GameBoardDto ToDto(IGameBoard gameBoard)
        {
            return new GameBoardDto
            {
                GameStartTimeStamp = gameBoard.GameStartTimeStamp,
                Version = gameBoard.Version,
                LastTurn = gameBoard.LastTurn != null ? ToDto(gameBoard.LastTurn) : null,
                Turns = gameBoard.Turns.Select(ToDto).ToList(),
                CardStackLevel1 = ToDto(gameBoard.CardStackLevel1),
                CardStackLevel2 = ToDto(gameBoard.CardStackLevel2),
                CardStackLevel3 = ToDto(gameBoard.CardStackLevel3),
                Level1Cards = gameBoard.Level1Cards?.Select(c => c != null ? ToDto(c) : null).ToArray() ?? new CardDto?[4],
                Level2Cards = gameBoard.Level2Cards?.Select(c => c != null ? ToDto(c) : null).ToArray() ?? new CardDto?[4],
                Level3Cards = gameBoard.Level3Cards?.Select(c => c != null ? ToDto(c) : null).ToArray() ?? new CardDto?[4],
                Players = gameBoard.Players?.Select(ToDto).ToList() ?? new List<PlayerDto>(),
                TokenStacks = gameBoard.TokenStacks != null ? new Dictionary<Token, int>(gameBoard.TokenStacks) : new Dictionary<Token, int>(),
                Nobles = gameBoard.Nobles?.Select(ToDto).ToList() ?? new List<NobleDto>(),
                CurrentPlayer = gameBoard.CurrentPlayer,
                LastRound = gameBoard.LastRound,
                GameOver = gameBoard.GameOver,
                IsPaused = gameBoard.IsPaused
            };
        }

        /// <summary>
        /// Converts DTO to IGameBoard (using reflection to set private properties)
        /// </summary>
        private static IGameBoard FromDto(GameBoardDto dto)
        {
            // Convert DTOs back to interfaces
            var players = dto.Players.Select(FromDto).ToList();

            // Create a new GameBoard - we'll need to use reflection to restore state
            // since GameBoard's constructor initializes new game state
            var cardStack1 = FromDto(dto.CardStackLevel1);
            var cardStack2 = FromDto(dto.CardStackLevel2);
            var cardStack3 = FromDto(dto.CardStackLevel3);

            var level1Cards = dto.Level1Cards.Select(c => c != null ? (ICard?)FromDto(c) : null).ToArray();
            var level2Cards = dto.Level2Cards.Select(c => c != null ? (ICard?)FromDto(c) : null).ToArray();
            var level3Cards = dto.Level3Cards.Select(c => c != null ? (ICard?)FromDto(c) : null).ToArray();

            var nobles = dto.Nobles.Select(FromDto).ToList();
            var tokenStacks = new Dictionary<Token, int>(dto.TokenStacks);

            var turns = dto.Turns.Select(FromDto).ToList();
            var lastTurn = dto.LastTurn != null ? FromDto(dto.LastTurn) : null;

            // Create a GameBoardState object that can be constructed with all state
            return new GameBoardState(
                dto.GameStartTimeStamp,
                dto.Version,
                lastTurn,
                turns,
                cardStack1,
                cardStack2,
                cardStack3,
                level1Cards,
                level2Cards,
                level3Cards,
                players,
                tokenStacks,
                nobles,
                dto.CurrentPlayer,
                dto.LastRound,
                dto.GameOver,
                dto.IsPaused
            );
        }

        private static PlayerDto ToDto(IPlayer player)
        {
            return new PlayerDto
            {
                Id = player.Id,
                Name = player.Name,
                Tokens = new Dictionary<Token, int>(player.Tokens),
                Cards = player.Cards.Select(ToDto).ToList(),
                CardTokens = new Dictionary<Token, int>(player.CardTokens),
                Nobles = player.Nobles.Select(ToDto).ToList(),
                ReservedCards = player.ReservedCards.Select(ToDto).ToList(),
                PrestigePoints = player.PrestigePoints
            };
        }

        private static IPlayer FromDto(PlayerDto dto)
        {
            return new PlayerState(
                dto.Id,
                dto.Name,
                dto.Tokens,
                dto.Cards.Select(FromDto).ToList(),
                dto.CardTokens,
                dto.Nobles.Select(FromDto).ToList(),
                dto.ReservedCards.Select(FromDto).ToList(),
                dto.PrestigePoints
            );
        }

        private static CardDto ToDto(ICard card)
        {
            return new CardDto
            {
                ImageName = card.ImageName,
                Level = card.Level,
                Type = card.Type,
                PrestigePoints = card.PrestigePoints,
                Price = new Dictionary<Token, int>(card.Price)
            };
        }

        private static ICard FromDto(CardDto dto)
        {
            return new Card(dto.Level, dto.Type, dto.PrestigePoints, dto.Price, dto.ImageName);
        }

        private static CardStackDto ToDto(ICardStack cardStack)
        {
            return new CardStackDto
            {
                Level = cardStack.Level,
                Cards = cardStack.Cards.Select(ToDto).ToList()
            };
        }

        private static ICardStack FromDto(CardStackDto dto)
        {
            return new CardStack(dto.Level, (uint)dto.Cards.Count, dto.Cards.Select(FromDto).ToList());
        }

        private static NobleDto ToDto(INoble noble)
        {
            return new NobleDto
            {
                ImageName = noble.ImageName,
                Criteria = new Dictionary<Token, int>(noble.Criteria),
                PrestigePoints = noble.PrestigePoints
            };
        }

        private static INoble FromDto(NobleDto dto)
        {
            return new Noble(dto.Criteria, dto.ImageName);
        }

        private static TurnDto ToDto(ITurn turn)
        {
            return new TurnDto
            {
                TakenTokens = turn.TakenTokens != null ? new Dictionary<Token, int>(turn.TakenTokens) : null,
                Card = turn.Card != null ? ToDto(turn.Card) : null,
                ReservedCard = turn.ReservedCard != null ? ToDto(turn.ReservedCard) : null,
                ReserveDeckLevel = turn.ReserveDeckLevel,
                Noble = turn.Noble != null ? ToDto(turn.Noble) : null,
                ContinueAction = turn.ContinueAction != null ? ToDto(turn.ContinueAction) : null,
                PlayerName = turn.PlayerName,
                TimeStamp = turn.TimeStamp
            };
        }

        private static ITurn FromDto(TurnDto dto)
        {
            return new TurnState(
                dto.TakenTokens,
                dto.Card != null ? FromDto(dto.Card) : null,
                dto.ReservedCard != null ? FromDto(dto.ReservedCard) : null,
                dto.ReserveDeckLevel,
                dto.Noble != null ? FromDto(dto.Noble) : null,
                dto.ContinueAction != null ? FromDto(dto.ContinueAction) : null,
                dto.PlayerName,
                dto.TimeStamp
            );
        }

        private static ContinueActionDto ToDto(IContinueAction continueAction)
        {
            return new ContinueActionDto
            {
                Message = continueAction.Message,
                ActionCode = continueAction.ActionCode,
                Nobles = continueAction.Nobles?.Select(ToDto).ToList()
            };
        }

        private static IContinueAction FromDto(ContinueActionDto dto)
        {
            if (dto.Nobles != null && dto.Nobles.Count > 0)
            {
                return new ContinueAction(dto.Message, dto.ActionCode, dto.Nobles.Select(FromDto).ToList());
            }
            return new ContinueAction(dto.Message, dto.ActionCode);
        }
    }
}
