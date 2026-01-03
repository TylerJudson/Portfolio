using Splendor.Models;

namespace Splendor.Services.Lookup
{
    /// <summary>
    /// Service for looking up nobles by their image names
    /// </summary>
    public class NobleLookupService : INobleLookupService
    {
        private readonly ILogger<NobleLookupService> _logger;

        public NobleLookupService(ILogger<NobleLookupService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Finds a noble on the game board by its image name
        /// </summary>
        /// <param name="board">The game board to search</param>
        /// <param name="imageName">The image name of the noble to find</param>
        /// <returns>The noble if found, null otherwise</returns>
        public INoble? FindNobleByImageName(IGameBoard board, string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return null;
            }

            // Search through the nobles on the board
            foreach (var noble in board.Nobles)
            {
                if (noble.ImageName == imageName)
                {
                    return noble;
                }
            }

            _logger.LogWarning("Noble not found on game board: {ImageName}", imageName);
            return null;
        }
    }
}
