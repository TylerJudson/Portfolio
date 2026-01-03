using System.Text.RegularExpressions;
using Splendor.Models;

namespace Splendor.Services.Lookup
{
    /// <summary>
    /// Service for looking up cards by their image names
    /// </summary>
    public class CardLookupService : ICardLookupService
    {
        private static readonly Regex LevelRegex = new Regex(@"Level(\d)", RegexOptions.Compiled);
        private readonly ILogger<CardLookupService> _logger;

        public CardLookupService(ILogger<CardLookupService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Finds a card on the game board by its image name
        /// </summary>
        /// <param name="board">The game board to search</param>
        /// <param name="imageName">The image name of the card to find</param>
        /// <returns>The card if found, null otherwise</returns>
        public ICard? FindCardByImageName(IGameBoard board, string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return null;
            }

            // Parse the level from the image name (e.g., "Level1-..." -> 1)
            int level = ParseLevelFromImageName(imageName);

            // Search the appropriate level's cards
            IReadOnlyList<ICard?> cardsToSearch = level switch
            {
                1 => board.Level1Cards,
                2 => board.Level2Cards,
                3 => board.Level3Cards,
                _ => Array.Empty<ICard?>()
            };

            // Search for the card in the appropriate level
            foreach (var card in cardsToSearch)
            {
                if (card != null && card.ImageName == imageName)
                {
                    return card;
                }
            }

            _logger.LogWarning("Card not found on game board: {ImageName}", imageName);
            return null;
        }

        /// <summary>
        /// Finds a card in a player's reserved cards by its image name
        /// </summary>
        /// <param name="player">The player whose reserved cards to search</param>
        /// <param name="imageName">The image name of the card to find</param>
        /// <returns>The card if found, null otherwise</returns>
        public ICard? FindReservedCardByImageName(IPlayer player, string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return null;
            }

            // Search through the player's reserved cards
            foreach (var card in player.ReservedCards)
            {
                if (card != null && card.ImageName == imageName)
                {
                    return card;
                }
            }

            _logger.LogWarning("Reserved card not found for player: {ImageName}", imageName);
            return null;
        }

        /// <summary>
        /// Parses the level number from a card's image name
        /// </summary>
        /// <param name="imageName">The image name (e.g., "Level1-Diamond-2P-...")</param>
        /// <returns>The level number (1, 2, or 3), or 0 if parsing fails</returns>
        private int ParseLevelFromImageName(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return 0;
            }

            // Try regex parsing first
            var match = LevelRegex.Match(imageName);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int level))
            {
                return level;
            }

            // Fallback to the original index-based parsing for backwards compatibility
            // Image names follow format: "Level{N}-..." where position 5 contains the level digit
            if (imageName.Length > 5 && char.IsDigit(imageName[5]))
            {
                return imageName[5] - '0';
            }

            return 0;
        }
    }
}
