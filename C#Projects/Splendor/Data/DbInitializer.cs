using Microsoft.EntityFrameworkCore;

namespace Splendor.Data
{
    /// <summary>
    /// Initializes the database on application startup
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Ensures the database is created and migrated to the latest version
        /// </summary>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SplendorDbContext>();

                try
                {
                    // Ensure database is created
                    context.Database.EnsureCreated();

                    // Apply any pending migrations
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }

                    Console.WriteLine("Database initialized successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing database: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously ensures the database is created and migrated to the latest version
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SplendorDbContext>();

                try
                {
                    // Ensure database is created
                    await context.Database.EnsureCreatedAsync();

                    // Apply any pending migrations
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        await context.Database.MigrateAsync();
                    }

                    Console.WriteLine("Database initialized successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing database: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
