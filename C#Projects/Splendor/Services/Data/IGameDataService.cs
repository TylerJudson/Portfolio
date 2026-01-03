using Splendor.Models;

namespace Splendor.Services.Data
{
    /// <summary>
    /// Service for loading game data including cards, nobles, and token configurations
    /// </summary>
    public interface IGameDataService
    {
        /// <summary>
        /// Loads all Level 1 cards from the game data source
        /// </summary>
        /// <returns>A list of Level 1 cards</returns>
        List<ICard> LoadLevel1Cards();

        /// <summary>
        /// Loads all Level 2 cards from the game data source
        /// </summary>
        /// <returns>A list of Level 2 cards</returns>
        List<ICard> LoadLevel2Cards();

        /// <summary>
        /// Loads all Level 3 cards from the game data source
        /// </summary>
        /// <returns>A list of Level 3 cards</returns>
        List<ICard> LoadLevel3Cards();

        /// <summary>
        /// Loads all nobles from the game data source
        /// </summary>
        /// <returns>A list of all nobles</returns>
        List<INoble> LoadNobles();

        /// <summary>
        /// Gets the token configuration for a specific number of players
        /// </summary>
        /// <param name="playerCount">The number of players in the game (2, 3, or 4)</param>
        /// <returns>A dictionary mapping token types to their initial quantities</returns>
        Dictionary<Token, int> GetTokenConfig(int playerCount);
    }
}
