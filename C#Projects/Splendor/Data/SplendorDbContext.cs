using Microsoft.EntityFrameworkCore;
using Splendor.Data.Entities;

namespace Splendor.Data
{
    /// <summary>
    /// Database context for Splendor game persistence
    /// </summary>
    public class SplendorDbContext : DbContext
    {
        public SplendorDbContext(DbContextOptions<SplendorDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Active games
        /// </summary>
        public DbSet<GameEntity> Games { get; set; } = null!;

        /// <summary>
        /// Pending games (waiting room)
        /// </summary>
        public DbSet<PendingGameEntity> PendingGames { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure GameEntity
            modelBuilder.Entity<GameEntity>(entity =>
            {
                entity.HasKey(e => e.GameId);
                entity.Property(e => e.GameStateJson).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastUpdatedAt).IsRequired();
                entity.Property(e => e.IsPaused).IsRequired();
                entity.Property(e => e.GameOver).IsRequired();
                entity.Property(e => e.Version).IsRequired();

                // Add index on LastUpdatedAt for cleanup queries
                entity.HasIndex(e => e.LastUpdatedAt);
            });

            // Configure PendingGameEntity
            modelBuilder.Entity<PendingGameEntity>(entity =>
            {
                entity.HasKey(e => e.GameId);
                entity.Property(e => e.CreatingPlayerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PlayersJson).IsRequired();
                entity.Property(e => e.MaxPlayers).IsRequired();
                entity.Property(e => e.TimeCreated).IsRequired();

                // Add index on TimeCreated for cleanup queries
                entity.HasIndex(e => e.TimeCreated);
            });
        }
    }
}
