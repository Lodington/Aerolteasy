# Risk of Rain 2 Development Tool - Setup Guide

## Prerequisites

1. **Risk of Rain 2** installed on Steam
2. **BepInEx** installed for Risk of Rain 2
3. **Visual Studio** or **Visual Studio Code** with C# support (for building the mod)

## Installation Steps

### 1. Install BepInEx
1. Download BepInEx 5.x from [GitHub](https://github.com/BepInEx/BepInEx/releases)
2. Extract to your Risk of Rain 2 directory (usually `Steam/steamapps/common/Risk of Rain 2/`)
3. Run the game once to generate BepInEx folders

### 2. Build the Mod
```bash
cd Mod
dotnet build
```

### 3. Install the Mod
1. Copy `Mod/bin/Debug/netstandard2.1/RoR2DevTool.dll` to `BepInEx/plugins/`
2. The mod will start an HTTP server on port 8080 when the game launches

### 4. Setup the Web Interface

**Option A (Svelte - Recommended):**
```bash
cd WebUI-Svelte
pnpm install
pnpm run dev
```
Then open `http://localhost:5173`

**Option B (Standalone HTML):**
Open `WebUI/standalone.html` directly in your browser

**Option C (Build Svelte for Production):**
```bash
cd WebUI-Svelte
pnpm install
pnpm run build
```
Then serve the `dist` folder

## Usage

1. **Launch Risk of Rain 2** with the mod installed
2. **Start the web interface**:
   - Svelte dev: `pnpm run dev` in WebUI-Svelte folder, then open `http://localhost:5173`
   - Standalone: Open `WebUI/standalone.html` directly
3. **Start a run** in Risk of Rain 2
4. **Use the web interface** to modify the game in real-time

The mod creates a built-in HTTP API server that the web interface connects to directly - no external server needed!

## Features

### Player Controls
- **God Mode**: Toggle invincibility
- **Character Change**: Switch characters mid-game
- **Health/Money**: Set custom values

### Item Management
- **Spawn Items**: Add any item to your inventory
- **View Current Items**: See all items you currently have

### Stage Controls
- **Change Stage**: Jump to any stage instantly
- **Stage Info**: View current stage and difficulty

## Troubleshooting

### Connection Issues
- Check that Risk of Rain 2 is running with the mod installed
- Verify the mod is loaded (check BepInEx console for "HTTP Server started on port 8080")
- Make sure your browser can access `http://localhost:8080/api/status`
- Try disabling browser extensions that might block local connections

### Mod Not Working
- Ensure BepInEx is properly installed
- Check BepInEx console for error messages
- Verify the mod DLL is in the correct plugins folder
- Make sure port 8080 is not blocked by firewall

### Web Interface Not Updating
- Check browser console for CORS errors
- Ensure Risk of Rain 2 is running and in a game
- Try refreshing the page
- Verify the API is responding at `http://localhost:8080/api/gamestate`

## API Endpoints
The mod exposes these endpoints on `http://localhost:8080`:
- `GET /api/gamestate` - Current game state
- `POST /api/command` - Send commands to game
- `GET /api/status` - Connection and game status

## Development

To modify the mod:
1. Edit `Mod/RoR2DevTool.cs`
2. Rebuild with `dotnet build`
3. Copy the new DLL to BepInEx plugins folder
4. Restart Risk of Rain 2

To modify the web interface:
1. Edit files in `WebUI/` folder
2. Refresh the browser (no restart needed)

## Security Note

This tool is for development and testing purposes only. The local server runs on localhost and should not be exposed to external networks.