using Splendor.Models;

namespace Splendor.Services.Lookup
{
    /// <summary>
    /// Service for looking up nobles by their image names
    /// </summary>
    public interface INobleLookupService
    {
        /// <summary>
        /// Finds a noble on the game board by its image name
        /// </summary>
        /// <param name="board">The game board to search</param>
        /// <param name="imageName">The image name of the noble to find</param>
        /// <returns>The noble if found, null otherwise</returns>
        INoble? FindNobleByImageName(IGameBoard board, string imageName);
    }
}
