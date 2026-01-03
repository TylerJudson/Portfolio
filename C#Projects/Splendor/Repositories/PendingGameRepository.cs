using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Splendor.Data;
using Splendor.Data.Entities;
using Splendor.Models;
using Splendor.Models.Implementation;
using System.Text.Json;

namespace Splendor.Repositories
{
    /// <summary>
    /// EF Core implementation of IPendingGameRepository with memory caching
    /// </summary>
    public class PendingGameRepository : IPendingGameRepository
    {
        private readonly SplendorDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "PendingGame_";
        private const string AllPendingGamesCacheKey = "AllPendingGames";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public PendingGameRepository(SplendorDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IPotentialGame?> GetPendingGameAsync(int gameId)
        {
            // Try to get from cache first
            string cacheKey = CacheKeyPrefix + gameId;
            if (_cache.TryGetValue(cacheKey, out IPotentialGame? cachedGame))
            {
                return cachedGame;
            }

            // If not in cache, get from database
            var entity = await _context.PendingGames.FindAsync(gameId);
            if (entity == null)
            {
                return null;
            }

            var potentialGame = EntityToPotentialGame(entity);

            // Cache the result
            _cache.Set(cacheKey, potentialGame, _cacheExpiration);

            return potentialGame;
        }

        public async Task<Dictionary<int, IPotentialGame>> GetAllPendingGamesAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(AllPendingGamesCacheKey, out Dictionary<int, IPotentialGame>? cachedGames))
            {
                return cachedGames!;
            }

            // If not in cache, get from database
            var entities = await _context.PendingGames.ToListAsync();
            var games = new Dictionary<int, IPotentialGame>();

            foreach (var entity in entities)
            {
                try
                {
                    var potentialGame = EntityToPotentialGame(entity);
                    games[entity.GameId] = potentialGame;

                    // Also cache individual games
                    string cacheKey = CacheKeyPrefix + entity.GameId;
                    _cache.Set(cacheKey, potentialGame, _cacheExpiration);
                }
                catch (Exception ex)
                {
                    // Log error and skip this game
                    Console.WriteLine($"Error deserializing pending game {entity.GameId}: {ex.Message}");
                }
            }

            // Cache the result
            _cache.Set(AllPendingGamesCacheKey, games, _cacheExpiration);

            return games;
        }

        public async Task AddPendingGameAsync(int gameId, IPotentialGame potentialGame)
        {
            var entity = PotentialGameToEntity(gameId, potentialGame);

            _context.PendingGames.Add(entity);
            await _context.SaveChangesAsync();

            // Update cache
            string cacheKey = CacheKeyPrefix + gameId;
            _cache.Set(cacheKey, potentialGame, _cacheExpiration);

            // Invalidate all games cache
            _cache.Remove(AllPendingGamesCacheKey);
        }

        public async Task UpdatePendingGameAsync(int gameId, IPotentialGame potentialGame)
        {
            var entity = await _context.PendingGames.FindAsync(gameId);
            if (entity == null)
            {
                throw new InvalidOperationException($"Pending game {gameId} not found");
            }

            entity.PlayersJson = JsonSerializer.Serialize(potentialGame.Players);
            entity.MaxPlayers = potentialGame.MaxPlayers;

            await _context.SaveChangesAsync();

            // Update cache
            string cacheKey = CacheKeyPrefix + gameId;
            _cache.Set(cacheKey, potentialGame, _cacheExpiration);

            // Invalidate all games cache
            _cache.Remove(AllPendingGamesCacheKey);
        }

        public async Task RemovePendingGameAsync(int gameId)
        {
            var entity = await _context.PendingGames.FindAsync(gameId);
            if (entity != null)
            {
                _context.PendingGames.Remove(entity);
                await _context.SaveChangesAsync();
            }

            // Remove from cache
            string cacheKey = CacheKeyPrefix + gameId;
            _cache.Remove(cacheKey);

            // Invalidate all games cache
            _cache.Remove(AllPendingGamesCacheKey);
        }

        public async Task RemoveStalePendingGamesAsync(TimeSpan maxAge)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(maxAge);

            var staleGames = await _context.PendingGames
                .Where(g => g.TimeCreated < cutoffTime)
                .ToListAsync();

            if (staleGames.Any())
            {
                _context.PendingGames.RemoveRange(staleGames);
                await _context.SaveChangesAsync();

                // Remove from cache
                foreach (var game in staleGames)
                {
                    string cacheKey = CacheKeyPrefix + game.GameId;
                    _cache.Remove(cacheKey);
                }

                // Invalidate all games cache
                _cache.Remove(AllPendingGamesCacheKey);
            }
        }

        public async Task<bool> PendingGameExistsAsync(int gameId)
        {
            // Check cache first
            string cacheKey = CacheKeyPrefix + gameId;
            if (_cache.TryGetValue(cacheKey, out _))
            {
                return true;
            }

            // Check database
            return await _context.PendingGames.AnyAsync(g => g.GameId == gameId);
        }

        /// <summary>
        /// Converts database entity to IPotentialGame
        /// </summary>
        private IPotentialGame EntityToPotentialGame(PendingGameEntity entity)
        {
            var players = JsonSerializer.Deserialize<Dictionary<int, string>>(entity.PlayersJson);
            if (players == null)
            {
                throw new InvalidOperationException($"Failed to deserialize players for game {entity.GameId}");
            }

            return new PotentialGameState(
                entity.GameId,
                entity.CreatingPlayerName,
                players,
                entity.MaxPlayers,
                entity.TimeCreated
            );
        }

        /// <summary>
        /// Converts IPotentialGame to database entity
        /// </summary>
        private PendingGameEntity PotentialGameToEntity(int gameId, IPotentialGame potentialGame)
        {
            return new PendingGameEntity
            {
                GameId = gameId,
                CreatingPlayerName = potentialGame.CreatingPlayerName,
                PlayersJson = JsonSerializer.Serialize(potentialGame.Players),
                MaxPlayers = potentialGame.MaxPlayers,
                TimeCreated = potentialGame.TimeCreated
            };
        }
    }

    /// <summary>
    /// Reconstructable version of PotentialGame that allows setting all properties
    /// </summary>
    internal class PotentialGameState : IPotentialGame
    {
        public int Id { get; }
        public string CreatingPlayerName { get; }

        private Dictionary<int, string> _players;
        public IReadOnlyDictionary<int, string> Players => _players;

        public int MaxPlayers { get; }
        public DateTime TimeCreated { get; }

        public PotentialGameState(
            int id,
            string creatingPlayerName,
            Dictionary<int, string> players,
            int maxPlayers,
            DateTime timeCreated)
        {
            Id = id;
            CreatingPlayerName = creatingPlayerName;
            _players = players;
            MaxPlayers = maxPlayers;
            TimeCreated = timeCreated;
        }

        public void AddPlayer(int playerId, string playerName)
        {
            _players.Add(playerId, playerName);
        }

        public bool RemovePlayer(int playerId)
        {
            return _players.Remove(playerId);
        }
    }
}
