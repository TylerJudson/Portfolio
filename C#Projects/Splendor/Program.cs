using Microsoft.EntityFrameworkCore;
using Splendor.Data;
using Splendor.Repositories;
using Splendor.Services.Data;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core with SQLite
builder.Services.AddDbContext<SplendorDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=splendor.db"));

// Add memory cache for repository caching
builder.Services.AddMemoryCache();

// Register repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPendingGameRepository, PendingGameRepository>();

// Register game data service
builder.Services.AddSingleton<IGameDataService, GameDataService>();

// Register lookup services
builder.Services.AddScoped<Splendor.Services.Lookup.ICardLookupService, Splendor.Services.Lookup.CardLookupService>();
builder.Services.AddScoped<Splendor.Services.Lookup.INobleLookupService, Splendor.Services.Lookup.NobleLookupService>();
builder.Services.AddScoped<Splendor.Services.Lookup.IPlayerLookupService, Splendor.Services.Lookup.PlayerLookupService>();

// Register game management services
builder.Services.AddScoped<Splendor.Services.Game.IGameActivationService, Splendor.Services.Game.GameActivationService>();
builder.Services.AddSingleton<Splendor.Services.Game.IGameCleanupService, Splendor.Services.Game.GameCleanupService>();
builder.Services.AddSingleton<Splendor.Services.Game.IGameIdGenerator, Splendor.Services.Game.GameIdGenerator>();
builder.Services.AddScoped<Splendor.Services.Game.IPlayerIdAssignmentService, Splendor.Services.Game.PlayerIdAssignmentService>();

// Configure rate limiting to prevent abuse
// TODO: Once authentication is implemented, partition by user ID instead of IP
builder.Services.AddRateLimiter(options =>
{
    // Default policy for general endpoints - limits by IP address
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100, // 100 requests
                Window = TimeSpan.FromMinutes(1), // per minute
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Initialize the database
DbInitializer.Initialize(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable rate limiting middleware
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
