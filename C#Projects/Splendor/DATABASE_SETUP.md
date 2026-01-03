# Database Persistence Setup - Section 2

This document describes the database persistence infrastructure added to the Splendor project in Section 2 of the refactoring plan.

## Overview

The Splendor game now has a complete Entity Framework Core infrastructure for persisting game state to a SQLite database. This infrastructure is **ready to use** but not yet fully integrated (that happens in Section 6).

## What Was Added

### 1. NuGet Packages (Splendor.csproj)
- `Microsoft.EntityFrameworkCore.Sqlite` (v9.0.0)
- `Microsoft.EntityFrameworkCore.Design` (v9.0.0)
- `Microsoft.EntityFrameworkCore.Tools` (v9.0.0)

### 2. Database Entities (Data/Entities/)
- **GameEntity.cs** - Persists active games as JSON
- **PendingGameEntity.cs** - Persists waiting room games

### 3. DbContext (Data/SplendorDbContext.cs)
- Configured with SQLite
- DbSets for Games and PendingGames
- Indexed on LastUpdatedAt and TimeCreated for cleanup queries

### 4. JSON Serialization (Serialization/)
- **GameBoardDto.cs** - Data transfer objects for JSON serialization
- **GameBoardSerializer.cs** - Handles interface-to-concrete type conversion
- **StateClasses.cs** - Reconstructable versions of GameBoard, Player, Turn, etc.

The serializer handles the complex task of serializing interfaces (IGameBoard, IPlayer, ICard, etc.) to JSON and deserializing back to concrete implementations.

### 5. Repository Pattern (Repositories/)
- **IGameRepository.cs** / **GameRepository.cs** - Active game persistence
- **IPendingGameRepository.cs** / **PendingGameRepository.cs** - Pending game persistence

Repositories include:
- Full async/await support
- Memory caching layer for performance (5-minute expiration)
- Thread-safe EF Core transactions
- Stale game cleanup methods

### 6. Database Initializer (Data/DbInitializer.cs)
- Ensures database is created on startup
- Applies pending migrations automatically

### 7. Dependency Injection (Program.cs)
- DbContext configured with SQLite connection
- Memory cache registered
- Repositories registered as scoped services

### 8. Controller Preparation (Controllers/)
- **GameController** - Injected with IGameRepository
- **WaitingRoomController** - Injected with IPendingGameRepository
- Static dictionaries remain in place (will be migrated in Section 6)

## Database File Location

The SQLite database will be created at:
```
/Users/tylerjudson/Source/Portfolio/Portfolio/C#Projects/Splendor/splendor.db
```

## Creating the Initial Migration

To create the initial database migration, run:

```bash
cd /Users/tylerjudson/Source/Portfolio/Portfolio/C#Projects/Splendor
dotnet ef migrations add InitialCreate --project Splendor.csproj
```

This will create a `Migrations/` folder with the initial database schema.

## Applying Migrations

The database is automatically initialized when the application starts (see `DbInitializer.Initialize()` in Program.cs).

To manually apply migrations:
```bash
dotnet ef database update --project Splendor.csproj
```

## Testing the Infrastructure

You can test the serialization/deserialization without fully integrating:

```csharp
// In a controller or test
var gameBoard = new GameBoard(players);
var json = GameBoardSerializer.Serialize(gameBoard);
var restored = GameBoardSerializer.Deserialize(json);
```

## Repository Usage Example

```csharp
// Saving a game
await _gameRepository.AddGameAsync(gameId, gameBoard);

// Loading a game
var gameBoard = await _gameRepository.GetGameAsync(gameId);

// Updating a game
await _gameRepository.UpdateGameAsync(gameId, gameBoard);

// Cleanup stale games (older than 30 minutes)
await _gameRepository.RemoveStaleGamesAsync(TimeSpan.FromMinutes(30));
```

## Key Design Decisions

### Interface Serialization
Since the domain models use interfaces (IGameBoard, IPlayer, etc.), we created:
1. **DTOs** - Plain classes for JSON serialization
2. **State Classes** - Reconstructable versions that implement the interfaces but accept full state
3. **Serializer** - Converts between interfaces and DTOs/State classes

### Caching Strategy
Repositories use `IMemoryCache` with 5-minute expiration:
- Individual games cached by ID
- All games cached as a collection
- Cache invalidated on updates/deletes

### Thread Safety
EF Core contexts are scoped per request, providing natural thread isolation. Repository methods use async/await throughout.

## Next Steps (Section 6)

Section 6 will:
1. Replace static dictionaries with repository calls
2. Make all controller actions async
3. Add background service for periodic cleanup
4. Test end-to-end persistence
5. Verify no data loss on application restart

## Important Notes

- The main project builds successfully with zero errors
- Test project has expected errors due to interface changes (will be fixed separately)
- All infrastructure is in place and ready to use
- Controllers are prepared with dependency injection but still use static dictionaries
- Database initialization runs automatically on application startup
