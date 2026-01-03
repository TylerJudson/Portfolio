# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Splendor is a digital implementation of the board game Splendor, built as an ASP.NET Core 6.0 MVC web application.

## Build and Run Commands

### Build
```bash
dotnet build
```

### Run Development Server
```bash
dotnet run
```

The application will be available at the URLs specified in `Properties/launchSettings.json`.

### Clean Build Artifacts
```bash
dotnet clean
```

## Architecture

### Core Game Flow

The application manages multiplayer Splendor games through a state machine pattern:

1. **Game Creation (WaitingRoomController)**: Players create or join pending games stored in `PendingGames` dictionary
2. **Game Activation (GameController)**: When ready, pending games transition to active games in `ActiveGames` dictionary
3. **Turn Execution (GameBoard)**: The `GameBoard.ExecuteTurn()` method validates and processes player actions
4. **State Management**: Game state is tracked via `Version` property; clients poll for updates

### Key Architectural Patterns

**Interface-First Design**: Core game entities are defined as interfaces (prefixed with `I`) with implementations in `Models/Implementation/`. This allows for flexible testing and potential future extensions.

**Static State Management**: Both `GameController.ActiveGames` and `WaitingRoomController.PendingGames` use static dictionaries to maintain game state across requests. This is a simple in-memory approach suitable for single-instance deployments but would need refactoring for distributed scenarios.

**Turn Validation**: `GameBoard.ExecuteTurn()` implements comprehensive rule validation before delegating to `Player.ExecuteTurn()`. Returns `ICompletedTurn` which may contain either:
- An `IError` if the turn was invalid
- An `IContinueAction` if the player needs to take additional actions (e.g., return tokens when over limit)
- Success (both null) if the turn completed normally

### Model Structure

**Game State Models**:
- `IGameBoard`: Central game state including card stacks (3 levels), visible cards, nobles, token pools, and player list
- `IPlayer`: Player state including tokens, cards, reserved cards, nobles, and prestige points
- `ITurn`: Represents a player action (take tokens, buy card, or reserve card)
- `ICompletedTurn`: Return type from turn execution containing optional error or continue action

**Token System**:
- `Token` enum represents the 6 token types (Diamond, Sapphire, Emerald, Ruby, Onyx, Gold)
- Token management uses `Dictionary<Token, int>` throughout the codebase

**Cards and Nobles**:
- `ICard`: Development cards with costs and token bonuses
- `ICardStack`: Deck management for each of the 3 levels
- `INoble`: Noble tiles that provide prestige points when requirements are met

### Controllers

**GameController**: Main gameplay endpoints
- `Index()`: Renders the game view
- `State()`: Polling endpoint for version checks and pause status
- `EndTurn()`: Processes player turns via JSON POST

**WaitingRoomController**: Pre-game lobby management
- `NewGame()`: Creates a pending game with random ID
- Cleanup of stale pending games based on timestamps

**ManagerController**: Administrative interface (details not explored)

**HomeController**: Landing page

### Client-Server Communication

The game uses a polling-based update mechanism:
1. Clients periodically call `Game/State/{gameId}/{playerId}` to check the current version
2. When version changes, clients update their UI
3. Players submit turns via POST to `Game/EndTurn/{gameId}/{playerId}`

## Important Implementation Details

### Game Initialization

`GameBoard` constructor initializes:
- Three card stacks (levels 1-3) with shuffled decks
- 4 visible cards from each level
- Token pools (quantities vary by player count)
- Noble tiles (player count + 1)

### Turn Validation Rules

The `ExecuteTurn()` method enforces Splendor rules:
- Max 3 different token types OR 2 of the same type
- Cannot take gold tokens directly (only via card reservation)
- When taking 2 of same type, must leave at least 2 in the pool
- Max 10 tokens per player (triggers return mechanism)

### Asset Management

Game assets are in `wwwroot/Images/`:
- Card images follow naming pattern: `Level{1-3}-{TokenType}-{PrestigePoints}P-{Costs}.png`
- Noble images: `Noble-{Costs}.png`
- Many card image references are explicitly excluded in `.csproj` (lines 17-116)

## Dependencies

- ASP.NET Core 6.0 Web SDK
- Microsoft.VisualStudio.Web.BrowserLink (v2.2.0)
