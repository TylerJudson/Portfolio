using Splendor.Models;

namespace Splendor.Services.Lookup
{
    /// <summary>
    /// Service for looking up cards by their image names
    /// </summary>
    public interface ICardLookupService
    {
        /// <summary>
        /// Finds a card on the game board by its image name
        /// </summary>
        /// <param name="board">The game board to search</param>
        /// <param name="imageName">The image name of the card to find</param>
        /// <returns>The card if found, null otherwise</returns>
        ICard? FindCardByImageName(IGameBoard board, string imageName);

        /// <summary>
        /// Finds a card in a player's reserved cards by its image name
        /// </summary>
        /// <param name="player">The player whose reserved cards to search</param>
        /// <param name="imageName">The image name of the card to find</param>
        /// <returns>The card if found, null otherwise</returns>
        ICard? FindReservedCardByImageName(IPlayer player, string imageName);
    }
}
