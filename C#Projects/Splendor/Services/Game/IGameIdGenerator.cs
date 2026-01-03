namespace Splendor.Services.Game
{
    /// <summary>
    /// Service for generating unique game IDs
    /// </summary>
    public interface IGameIdGenerator
    {
        /// <summary>
        /// Generates a unique game ID that doesn't conflict with existing pending games
        /// </summary>
        /// <returns>A unique game ID</returns>
        Task<int> GenerateUniqueIdAsync();
    }
}
