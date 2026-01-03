using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Splendor.Data;
using Splendor.Data.Entities;
using Splendor.Models;
using Splendor.Serialization;

namespace Splendor.Repositories
{
    /// <summary>
    /// EF Core implementation of IGameRepository with memory caching
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private readonly SplendorDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "Game_";
        private const string AllGamesCacheKey = "AllGames";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public GameRepository(SplendorDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IGameBoard?> GetGameAsync(int gameId)
        {
            // Try to get from cache first
            string cacheKey = CacheKeyPrefix + gameId;
            if (_cache.TryGetValue(cacheKey, out IGameBoard? cachedGame))
            {
                return cachedGame;
            }

            // If not in cache, get from database
            var entity = await _context.Games.FindAsync(gameId);
            if (entity == null)
            {
                return null;
            }

            var gameBoard = GameBoardSerializer.Deserialize(entity.GameStateJson);

            // Cache the result
            _cache.Set(cacheKey, gameBoard, _cacheExpiration);

            return gameBoard;
        }

        public async Task<Dictionary<int, IGameBoard>> GetAllGamesAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(AllGamesCacheKey, out Dictionary<int, IGameBoard>? cachedGames))
            {
                return cachedGames!;
            }

            // If not in cache, get from database
            var entities = await _context.Games.ToListAsync();
            var games = new Dictionary<int, IGameBoard>();

            foreach (var entity in entities)
            {
                try
                {
                    var gameBoard = GameBoardSerializer.Deserialize(entity.GameStateJson);
                    games[entity.GameId] = gameBoard;

                    // Also cache individual games
                    string cacheKey = CacheKeyPrefix + entity.GameId;
                    _cache.Set(cacheKey, gameBoard, _cacheExpiration);
                }
                catch (Exception ex)
                {
                    // Log error and skip this game
                    Console.WriteLine($"Error deserializing game {entity.GameId}: {ex.Message}");
                }
            }

            // Cache the result
            _cache.Set(AllGamesCacheKey, games, _cacheExpiration);

            return games;
        }

        public async Task AddGameAsync(int gameId, IGameBoard gameBoard)
        {
            var json = GameBoardSerializer.Serialize(gameBoard);

            var entity = new GameEntity
            {
                GameId = gameId,
                GameStateJson = json,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsPaused = gameBoard.IsPaused,
                GameOver = gameBoard.GameOver,
                Version = gameBoard.Version
            };

            _context.Games.Add(entity);
            await _context.SaveChangesAsync();

            // Update cache
            string cacheKey = CacheKeyPrefix + gameId;
            _cache.Set(cacheKey, gameBoard, _cacheExpiration);

            // Invalidate all games cache
            _cache.Remove(AllGamesCacheKey);
        }

        public async Task UpdateGameAsync(int gameId, IGameBoard gameBoard)
        {
            var entity = await _context.Games.FindAsync(gameId);
            if (entity == null)
            {
                throw new InvalidOperationException($"Game {gameId} not found");
            }

            var json = GameBoardSerializer.Serialize(gameBoard);

            entity.GameStateJson = json;
            entity.LastUpdatedAt = DateTime.UtcNow;
            entity.IsPaused = gameBoard.IsPaused;
            entity.GameOver = gameBoard.GameOver;
            entity.Version = gameBoard.Version;

            await _context.SaveChangesAsync();

            // Update cache
            string cacheKey = CacheKeyPrefix + gameId;
            _cache.Set(cacheKey, gameBoard, _cacheExpiration);

            // Invalidate all games cache
            _cache.Remove(AllGamesCacheKey);
        }

        public async Task RemoveGameAsync(int gameId)
        {
            var entity = await _context.Games.FindAsync(gameId);
            if (entity != null)
            {
                _context.Games.Remove(entity);
                await _context.SaveChangesAsync();
            }

            // Remove from cache
            string cacheKey = CacheKeyPrefix + gameId;
            _cache.Remove(cacheKey);

            // Invalidate all games cache
            _cache.Remove(AllGamesCacheKey);
        }

        public async Task RemoveStaleGamesAsync(TimeSpan maxAge)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(maxAge);

            var staleGames = await _context.Games
                .Where(g => g.LastUpdatedAt < cutoffTime && !g.IsPaused)
                .ToListAsync();

            if (staleGames.Any())
            {
                _context.Games.RemoveRange(staleGames);
                await _context.SaveChangesAsync();

                // Remove from cache
                foreach (var game in staleGames)
                {
                    string cacheKey = CacheKeyPrefix + game.GameId;
                    _cache.Remove(cacheKey);
                }

                // Invalidate all games cache
                _cache.Remove(AllGamesCacheKey);
            }
        }

        public async Task<bool> GameExistsAsync(int gameId)
        {
            // Check cache first
            string cacheKey = CacheKeyPrefix + gameId;
            if (_cache.TryGetValue(cacheKey, out _))
            {
                return true;
            }

            // Check database
            return await _context.Games.AnyAsync(g => g.GameId == gameId);
        }
    }
}
