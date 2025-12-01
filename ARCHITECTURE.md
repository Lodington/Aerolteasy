# RoR2 DevTool Architecture

## Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         RISK OF RAIN 2 GAME                             │
│                                                                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐                 │
│  │   Players    │  │   Monsters   │  │ Interactables│                 │
│  │   (RoR2)     │  │   (RoR2)     │  │    (RoR2)    │                 │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘                 │
│         │                  │                  │                          │
│         └──────────────────┴──────────────────┘                          │
│                            │                                             │
│                            ▼                                             │
│                  ┌─────────────────────┐                                │
│                  │   Game State Data   │                                │
│                  │  (Unity Objects)    │                                │
│                  └─────────┬───────────┘                                │
└────────────────────────────┼─────────────────────────────────────────────┘
                             │
                             │ Unity API Calls
                             │
┌────────────────────────────▼─────────────────────────────────────────────┐
│                      BEPINEX MOD (C#)                                    │
│                                                                          │
│  ┌────────────────────────────────────────────────────────────────────┐ │
│  │                    RoR2DevToolPlugin.cs                            │ │
│  │                    (Main Entry Point)                              │ │
│  └────────────────────────────────────────────────────────────────────┘ │
│                             │                                            │
│         ┌───────────────────┼───────────────────┐                       │
│         │                   │                   │                       │
│         ▼                   ▼                   ▼                       │
│  ┌─────────────┐   ┌──────────────┐   ┌──────────────┐                │
│  │ GameState   │   │  Command     │   │  Permission  │                │
│  │  Service    │   │  Processor   │   │   Service    │                │
│  └──────┬──────┘   └──────┬───────┘   └──────┬───────┘                │
│         │                  │                   │                        │
│         │ Reads Game Data  │ Executes Commands │ Checks Permissions     │
│         │                  │                   │                        │
│         └──────────────────┴───────────────────┘                        │
│                            │                                             │
│                            ▼                                             │
│                  ┌─────────────────────┐                                │
│                  │    HTTP Server      │                                │
│                  │  (Port 8080)        │                                │
│                  │                     │                                │
│                  │  Endpoints:         │                                │
│                  │  • /api/gamestate   │                                │
│                  │  • /api/command     │                                │
│                  │  • /api/status      │                                │
│                  │  • /api/permissions │                                │
│                  └─────────┬───────────┘                                │
└────────────────────────────┼─────────────────────────────────────────────┘
                             │
                             │ HTTP (localhost:8080)
                             │ JSON Data
                             │
┌────────────────────────────▼─────────────────────────────────────────────┐
│                    TAURI DESKTOP APP (Rust + Svelte)                    │
│                                                                          │
│  ┌────────────────────────────────────────────────────────────────────┐ │
│  │                         Frontend (Svelte)                          │ │
│  │                                                                    │ │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │ │
│  │  │  api.js      │  │  stores.js   │  │ Components   │           │ │
│  │  │              │  │              │  │              │           │ │
│  │  │ • Fetch API  │  │ • Game State │  │ • PlayerView │           │ │
│  │  │ • Commands   │  │ • Reactive   │  │ • ItemMgmt   │           │ │
│  │  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘           │ │
│  │         │                  │                  │                   │ │
│  │         └──────────────────┴──────────────────┘                   │ │
│  │                            │                                       │ │
│  └────────────────────────────┼───────────────────────────────────────┘ │
│                               │                                         │
│  ┌────────────────────────────▼───────────────────────────────────────┐ │
│  │                    Tauri Backend (Rust)                            │ │
│  │                    • Window Management                             │ │
│  │                    • Native OS Integration                         │ │
│  └────────────────────────────────────────────────────────────────────┘ │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

## Request/Response Flow

### 1. Getting Game State (Server-Sent Events - Real-time)

```
┌─────────┐                ┌─────────┐                ┌─────────┐
│   UI    │                │   Mod   │                │  Game   │
└────┬────┘                └────┬────┘                └────┬────┘
     │                          │                          │
     │ GET /api/events          │                          │
     │ (EventSource)            │                          │
     │─────────────────────────>│                          │
     │                          │                          │
     │ Connection Established   │                          │
     │<─────────────────────────│                          │
     │                          │                          │
     │                          │ [Every 100ms]            │
     │                          │                          │
     │                          │ Read Player Data         │
     │                          │─────────────────────────>│
     │                          │                          │
     │                          │ Player Objects           │
     │                          │<─────────────────────────│
     │                          │                          │
     │                          │ Read Monster Data        │
     │                          │─────────────────────────>│
     │                          │                          │
     │                          │ Monster Objects          │
     │                          │<─────────────────────────│
     │                          │                          │
     │                          │ Detect Changes           │
     │                          │──────────┐               │
     │                          │          │               │
     │                          │<─────────┘               │
     │                          │                          │
     │ event: gamestate         │                          │
     │ data: {...JSON...}       │                          │
     │<─────────────────────────│                          │
     │                          │                          │
     │ Update UI (Reactive)     │                          │
     │──────────┐               │                          │
     │          │               │                          │
     │<─────────┘               │                          │
     │                          │                          │
     │ [Connection stays open]  │                          │
     │<════════════════════════>│                          │
     │                          │                          │
```

### 2. Sending Commands (Write Operation)

```
┌─────────┐                ┌─────────┐                ┌─────────┐
│   UI    │                │   Mod   │                │  Game   │
└────┬────┘                └────┬────┘                └────┬────┘
     │                          │                          │
     │ POST /api/command        │                          │
     │ {Type: "spawnitem",      │                          │
     │  Data: {itemName: "Bear",│                          │
     │         count: 5}}       │                          │
     │─────────────────────────>│                          │
     │                          │                          │
     │                          │ Check Permissions        │
     │                          │──────────┐               │
     │                          │          │               │
     │                          │<─────────┘               │
     │                          │                          │
     │                          │ Queue Command            │
     │                          │──────────┐               │
     │                          │          │               │
     │                          │<─────────┘               │
     │                          │                          │
     │ {success: true}          │                          │
     │<─────────────────────────│                          │
     │                          │                          │
     │                          │ (Next Update() cycle)    │
     │                          │                          │
     │                          │ Execute Command          │
     │                          │─────────────────────────>│
     │                          │                          │
     │                          │                          │
     │                          │ Game State Modified      │
     │                          │<─────────────────────────│
     │                          │                          │
```

## Component Breakdown

### Game → Mod (Data Collection)

**Location:** `Mod/Services/GameStateService.cs`

```csharp
// Collects data from Unity game objects
public GameState GetCurrentState()
{
    var state = new GameState();
    
    // Read from RoR2 API
    state.Players = GetAllPlayers();      // PlayerCharacterMasterController
    state.Monsters = GetAllMonsters();    // CharacterBody.readOnlyInstancesList
    state.Interactables = GetInteractables(); // InstanceTracker
    state.Teleporter = GetTeleporter();   // TeleporterInteraction
    
    return state;
}
```

### Mod → UI (HTTP Server)

**Location:** `Mod/Services/HttpServer.cs`

```csharp
// Serves data over HTTP
httpListener.Prefixes.Add("http://localhost:8080/");

// Endpoints handle requests
endpoints["/api/gamestate"] = new GameStateEndpoint(gameStateService);
endpoints["/api/command"] = new CommandEndpoint(networkingService);
```

### UI → Mod (API Calls)

**Location:** `WebUI-Svelte/src/lib/api.js`

```javascript
// Fetches data from mod
async getGameState() {
    const response = await fetch('http://localhost:8080/api/gamestate');
    return await response.json();
}

// Sends commands to mod
async sendCommand(command) {
    const response = await fetch('http://localhost:8080/api/command', {
        method: 'POST',
        body: JSON.stringify(command)
    });
    return await response.json();
}
```

### UI State Management

**Location:** `WebUI-Svelte/src/lib/stores.js`

```javascript
// Reactive stores update UI automatically
export const gameState = writable({
    IsInRun: false,
    Players: [],
    Monsters: [],
    // ... etc
});

// Polling keeps data fresh
setInterval(async () => {
    const state = await api.getGameState();
    gameState.set(state);
}, 1000); // Every second
```

## Key Technologies

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **Game** | Unity (C#) | Risk of Rain 2 game engine |
| **Mod** | BepInEx (C#) | Mod framework, hooks into game |
| **HTTP Server** | HttpListener (C#) | Serves data to UI |
| **Desktop App** | Tauri (Rust) | Native desktop wrapper |
| **Frontend** | Svelte (JS) | Reactive UI framework |
| **Styling** | DaisyUI + Tailwind | Component library |

## Data Format Example

### Game State JSON
```json
{
  "IsInRun": true,
  "CurrentStage": "golemplains",
  "GameTime": 1234.56,
  "Players": [
    {
      "PlayerId": 1,
      "PlayerName": "Lodington",
      "CurrentCharacter": "CommandoBody",
      "Level": 3,
      "Health": 176,
      "MaxHealth": 176,
      "Items": {
        "Syringe": 5,
        "Bear": 3
      }
    }
  ],
  "Monsters": [...],
  "Interactables": [...]
}
```

### Command JSON
```json
{
  "Type": "spawnitem",
  "Data": {
    "itemName": "Syringe",
    "count": 5,
    "playerId": 1
  }
}
```

## Security & Permissions

```
┌─────────────────────────────────────────────────────────────┐
│                    Permission Levels                        │
├─────────────────────────────────────────────────────────────┤
│  None       → No access                                     │
│  ReadOnly   → View game state only                          │
│  Basic      → Spawn items, change character                 │
│  Advanced   → Modify stats, teleport                        │
│  Admin      → Kill players, change stage                    │
│  Host       → Full control (auto-granted to host)           │
└─────────────────────────────────────────────────────────────┘
```

**Location:** `Mod/Services/PermissionService.cs`

Each command requires a specific permission level. The mod checks permissions before executing any command.

## Performance Considerations

- **SSE Updates:** Mod checks for changes every 100ms, only sends when state changes
- **No Polling Overhead:** UI receives updates instantly via SSE (push model)
- **Command Queue:** Commands are queued and executed in Update() cycle
- **God Mode:** Updated every 100ms (not every frame)
- **ESP Overlays:** Updated every 50ms (20 FPS)
- **HTTP Server:** Runs on background thread to avoid blocking game
- **SSE Thread:** Separate thread for SSE updates, doesn't block game or HTTP server

## Startup Sequence

1. **Game Launches** → BepInEx loads mods
2. **Mod Initializes** → Services start, HTTP server binds to port 8080
3. **Mod Launches UI** → Checks if `ror2-devtool.exe` is running, launches if not
4. **UI Connects** → Polls `/api/status` to verify connection
5. **Data Flows** → UI polls game state, sends commands as needed
