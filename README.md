# RoR2 Development Tool

A modern development tool for Risk of Rain 2 with a sleek web-based UI and powerful multiplayer networking support!

**Now with Automatic UI Launch!**

## 🚀 Quick Start

1. Install BepInEx in Risk of Rain 2
2. Copy `RoR2DevTool.dll` and `ror2-devtool-ui.exe` to `BepInEx/plugins/`
3. Launch Risk of Rain 2
4. The UI automatically opens - start modding!

## ✨ Features

### 🎮 Player Management
- **God Mode** - Toggle invincibility
- **Character Switching** - Change to any survivor
- **Stats Editor** - Modify health, damage, armor, speed, and more
- **Level Control** - Set player level instantly
- **Item Management** - Add, remove, or modify items with searchable catalog
- **Teleportation** - Move players to any location
- **Revive/Kill** - Control player life state

### 👾 Monster Spawning
- **Spawn Any Monster** - Full monster catalog
- **Elite Types** - Apply elite modifiers
- **Give Items/Buffs** - Customize monster loadouts
- **Team Control** - Change monster allegiance

### 🗺️ World Control
- **Stage Selection** - Jump to any stage instantly
- **Teleporter Control** - Charge, activate, or skip events
- **Spawn Teleporter** - Place teleporter anywhere
- **Money Management** - Set team money

### 👁️ ESP & Tracking
- **Interactables** - See chests, barrels, shrines
- **Monsters** - Track all enemies
- **Teleporter** - Never lose the objective
- **Customizable** - Toggle individual overlays

### 🌐 Multiplayer Networking
- **Permission System** - 6 levels from ReadOnly to Host
- **Host Controls** - Manage who can use what commands
- **Client Requests** - Request elevated permissions
- **Network Status** - See all connected players
- **Secure** - Server-side validation for all commands

## 📋 Main UI

### Player View
Select any player in your lobby and modify their stats, items, and abilities:
- Real-time stat editing (damage, armor, attack speed, crit chance)
- Character selection with default stat loading
- Health and level management
- Comprehensive item search and management
- Quick actions (god mode, heal, level up, teleport)

### Monster View
Spawn and control monsters:
- Full monster catalog with search
- Elite type selection
- Team assignment
- Item and buff management

### Interactables View
Spawn any interactable object:
- Chests (all tiers)
- Barrels and equipment barrels
- Shrines and altars
- Printers and scrappers
- Drones

### Teleporter View
Complete teleporter control:
- Instant charge
- Activate/skip events
- Spawn teleporter
- Stage selection

### ESP & Tracking
Visual overlays for game objects:
- Interactables (chests, barrels, shrines)
- Monsters
- Teleporter
- Customizable colors and toggles

### Network & Permissions
Multiplayer permission management:
- View connected players
- Grant/revoke permissions (Host only)
- Request permissions (Clients)
- See current permission level
- Permission level reference guide

## 🔐 Permission Levels

| Level | Capabilities |
|-------|-------------|
| **None** | No access - must request permissions |
| **ReadOnly** | View game state and debug info |
| **Basic** | Spawn items, set money/health/level |
| **Advanced** | God mode, teleport, spawn monsters |
| **Admin** | Change stage, kill/revive players |
| **Host** | Full control + permission management |

## 🎯 Command Permissions

- **ReadOnly**: Debug commands, refresh state
- **Basic**: Items, money, health, level, ESP
- **Advanced**: God mode, character change, teleport, spawn monsters
- **Admin**: Stage change, kill/revive, teleporter control

## 🛠️ Technical Features

### REST API
- Full REST API on `http://localhost:8080`
- GET `/api/command` - List all available commands
- POST `/api/command` - Execute commands
- GET `/api/gamestate` - Current game state
- GET `/api/network/status` - Network and player info
- Permission management endpoints

### Modern UI
- Built with Svelte + Tauri
- Responsive design with DaisyUI
- Real-time game state updates
- Toast notifications
- Dark theme optimized
- Smooth animations

### Auto-Launch
- UI automatically launches with the game
- Detects if already running (no duplicates)
- Independent process (doesn't close with game)
- Manual launch supported

## 📦 Installation

### Requirements
- Risk of Rain 2
- BepInEx 5.x
- Windows 10/11 (64-bit)

### Steps
1. Install BepInEx if not already installed
2. Download the latest release
3. Extract to `Risk of Rain 2/BepInEx/plugins/`:
   ```
   BepInEx/plugins/
   ├── RoR2DevTool.dll
   └── ror2-devtool.exe
   ```
4. Launch Risk of Rain 2
5. UI opens automatically!

### Manual UI Launch
If the UI doesn't auto-launch, double-click `ror2-devtool-ui.exe`

## 🎮 Usage

### Single Player
- All features available immediately
- Automatic Host permissions
- No setup required

### Multiplayer - Host
1. Start a multiplayer game
2. Other players appear in Network view
3. Grant permissions to players as needed
4. Manage who can use which commands

### Multiplayer - Client
1. Join a multiplayer game
2. Go to Network & Permissions view
3. Request desired permission level
4. Wait for host approval
5. Use commands based on granted level

## 🐛 Troubleshooting

### UI Doesn't Launch
- Check BepInEx console log for errors
- Ensure `ror2-devtool-ui.exe` is in the same folder as the DLL
- Try launching the UI manually
- Check Windows Defender/Antivirus

### Can't Connect to Game
- Verify HTTP server started (check BepInEx log)
- Ensure port 5173 is available
- Check firewall settings
- Restart the game

### Permission Denied
- Check your permission level in Network view
- Request higher permissions from host
- Verify you're connected to multiplayer

### Commands Not Working
- Check BepInEx console for errors
- Verify you're in an active run
- Ensure player is selected
- Check command requirements


## 📄 Privacy Policy

This mod does not collect, store, or transmit any personal information beyond what's necessary for multiplayer functionality (Steam username/id for permission management).


## 🙏 Credits

- Built with BepInEx
- UI powered by Svelte + Tauri
- Styled with DaisyUI + Tailwind CSS
- Icons from Lucide

## 📞 Support

- Discord: https://links.lodington.dev/discord


---

Made with ❤️ for the Risk of Rain 2 modding community
