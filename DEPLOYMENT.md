# RoR2 DevTool Deployment Guide

## Overview
The RoR2 DevTool consists of two components:
1. **Mod (C# DLL)** - BepInEx plugin that runs inside Risk of Rain 2
2. **UI (Tauri App)** - Standalone desktop application for controlling the mod

## Automatic UI Launch
The mod will automatically launch the UI when Risk of Rain 2 starts, but only if:
- The UI executable is not already running
- The UI executable can be found in one of the search locations

## Installation

### For End Users

1. **Install the Mod**:
   - Copy `RoR2DevTool.dll` to `Risk of Rain 2/BepInEx/plugins/`
   
2. **Install the UI** (Choose one option):

   **Option A: Same folder as mod (Recommended)**
   ```
   Risk of Rain 2/
   └── BepInEx/
       └── plugins/
           ├── RoR2DevTool.dll
           └── ror2-devtool-ui.exe
   ```

   **Option B: UI subfolder**
   ```
   Risk of Rain 2/
   └── BepInEx/
       └── plugins/
           ├── RoR2DevTool.dll
           └── UI/
               └── ror2-devtool-ui.exe
   ```

   **Option C: Game root directory**
   ```
   Risk of Rain 2/
   ├── Risk of Rain 2.exe
   ├── ror2-devtool-ui.exe
   └── BepInEx/
       └── plugins/
           └── RoR2DevTool.dll
   ```

3. **Launch the Game**:
   - Start Risk of Rain 2
   - The mod will automatically launch the UI
   - If the UI doesn't launch automatically, run `ror2-devtool-ui.exe` manually

## UI Search Locations

The mod searches for the UI executable in this order:
1. Same directory as the mod DLL
2. `UI` subfolder in mod directory
3. Parent directory (`BepInEx/plugins/`)
4. BepInEx root directory
5. Game root directory
6. Recursive search (up to 3 levels deep)

## Manual UI Launch

If automatic launch fails, you can:
1. Run `ror2-devtool-ui.exe` manually before starting the game
2. Run it after the game starts
3. The mod will detect it's already running and won't launch a duplicate

## Building for Distribution

### Build the Mod
```bash
cd Mod
dotnet build -c Release
```
Output: `Mod/bin/Release/netstandard2.1/RoR2DevTool.dll`

### Build the UI
```bash
cd WebUI-Svelte
pnpm install
pnpm build:portable
```
Output: `WebUI-Svelte/src-tauri/target/release/ror2-devtool-ui.exe`

### Create Distribution Package

**Option 1: Simple ZIP**
```
RoR2DevTool-v1.0.0.zip
├── README.txt
├── RoR2DevTool.dll
└── ror2-devtool-ui.exe
```

**Option 2: Installer Package**
```
RoR2DevTool-v1.0.0.zip
├── README.txt
├── RoR2DevTool.dll
└── RoR2 DevTool_1.0.0_x64-setup.exe  (NSIS installer for UI)
```

## Distribution README Template

```txt
RoR2 Development Tool v1.0.0

INSTALLATION:
1. Make sure BepInEx is installed in Risk of Rain 2
2. Copy RoR2DevTool.dll to: Risk of Rain 2/BepInEx/plugins/
3. Copy ror2-devtool-ui.exe to the same folder (or game root)
4. Launch Risk of Rain 2

The UI will automatically open when you start the game!

MANUAL LAUNCH:
If the UI doesn't open automatically, double-click ror2-devtool-ui.exe

FEATURES:
- Player management (god mode, stats, items)
- Monster spawning
- Stage control
- ESP overlays
- Multiplayer permission system

REQUIREMENTS:
- Risk of Rain 2
- BepInEx 5.x
- Windows 10/11 (64-bit)

SUPPORT:
- GitHub: [your-repo-url]
- Discord: [your-discord]
```

## Troubleshooting

### UI Doesn't Launch Automatically
**Check the BepInEx console log** (`BepInEx/LogOutput.log`):
- Look for: "Found UI executable at: [path]"
- If not found: "UI executable not found in any standard location"

**Solutions**:
1. Ensure `ror2-devtool-ui.exe` is in one of the search locations
2. Launch the UI manually
3. Check Windows Defender/Antivirus isn't blocking it

### Multiple UI Windows Open
The mod checks if the UI is already running by process name. If you renamed the executable, it won't detect it.

**Solution**: Keep the executable named `ror2-devtool-ui.exe`

### UI Launches But Doesn't Connect
**Check**:
1. Is the HTTP server running? (Check BepInEx log for "HTTP Server started")
2. Is port 8080 available?
3. Is a firewall blocking localhost connections?

**Solution**: 
- Restart the game
- Check firewall settings
- Try launching UI manually after game starts

## Advanced Configuration

### Custom UI Location
If you want to use a custom location:
1. Launch the UI manually before starting the game
2. The mod will detect it's running and skip auto-launch

### Disable Auto-Launch
Currently not configurable. To disable:
- Remove or rename `ror2-devtool-ui.exe`
- Launch it manually when needed

## Development Notes

### Testing Auto-Launch
1. Close all instances of `ror2-devtool-ui.exe`
2. Start Risk of Rain 2
3. Check BepInEx console for launch messages
4. UI should open within 1-2 seconds

### Process Detection
The mod uses `Process.GetProcessesByName("ror2-devtool-ui")` to check if UI is running.
- Works even if UI is launched manually
- Works across different launch methods
- Independent of parent-child process relationship

## Release Checklist

- [ ] Build mod in Release configuration
- [ ] Build UI with `pnpm build:portable`
- [ ] Test auto-launch functionality
- [ ] Test manual launch
- [ ] Test with UI already running
- [ ] Create distribution package
- [ ] Write release notes
- [ ] Test on clean installation
- [ ] Update version numbers
- [ ] Create GitHub release
