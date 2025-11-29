# Changelog

All notable changes to RoR2 Development Tool will be documented in this file.

## [5.0.0] - 2024-11-28 - Complete Rewrite 🎉

### 🚀 Major Changes
**Complete ground-up rewrite** with modern architecture and web-based UI!

### ✨ New Features

#### Modern Web UI
- **Tauri-based Desktop App** - Standalone application with modern web technologies
- **Svelte + DaisyUI** - Beautiful, responsive interface with dark theme
- **Real-time Updates** - Live game state synchronization
- **Toast Notifications** - Beautiful animated notifications for all actions
- **Tabbed Interface** - Organized views for different features
- **Auto-Launch** - UI automatically opens when game starts

#### Player Management
- **Enhanced Stats Editor** - Edit all player stats (damage, armor, speed, crit, etc.)
- **Character Defaults System** - Load base stats for any character
- **Searchable Item Catalog** - Find and add items with intelligent search
- **Item Management** - Add, remove, or modify item counts with expandable cards
- **Quick Actions Panel** - God mode, heal, level up, revive with one click
- **Target Mode** - Apply changes to selected player or all players
- **Teleportation** - Move players to any coordinates with visual effects

#### Multiplayer Networking
- **6-Level Permission System** - None, ReadOnly, Basic, Advanced, Admin, Host
- **Host Controls** - Manage permissions for all connected players
- **Client Requests** - Request elevated permissions from host
- **Network Status View** - See all connected players and their permissions
- **Command-Level Permissions** - Each command has specific permission requirements
- **Server-Side Validation** - All commands validated on host
- **Permission Badges** - Visual indicators for permission levels

#### REST API
- **Full REST API** - HTTP server on port 8080
- **GET /api/command** - List all available commands with metadata
- **POST /api/command** - Execute commands with validation
- **GET /api/gamestate** - Current game state
- **GET /api/network/status** - Network and player information
- **GET /api/permissions** - Permission management
- **Enhanced Error Responses** - Detailed error messages with status codes
- **Command Validation** - Checks for unknown commands and missing parameters

#### ESP & Tracking
- **Modular ESP System** - Toggle individual overlay types
- **Interactables Tracking** - Chests, barrels, shrines, printers
- **Monster Tracking** - See all enemies with health bars
- **Teleporter Tracking** - Never lose the objective
- **Customizable** - Configure colors and visibility per type

#### World Control
- **Stage Selection** - Jump to any stage instantly
- **Teleporter Control** - Charge, activate, skip events
- **Spawn Teleporter** - Place teleporter anywhere
- **Money Management** - Set team money with validation

#### Monster Spawning
- **Full Monster Catalog** - Spawn any monster in the game
- **Elite Types** - Apply elite modifiers
- **Team Control** - Change monster allegiance
- **Item/Buff Management** - Give items and buffs to monsters

### 🛠️ Technical Improvements
- **Service-Based Architecture** - Clean separation of concerns
- **Command Processor** - Queue-based command execution
- **HTTP Server** - Robust endpoint system with CORS support
- **Game State Service** - Centralized state management
- **ESP Overlay Service** - Optimized rendering system
- **UI Launcher Service** - Automatic UI process management
- **Error Handling** - Comprehensive error logging and recovery
- **Performance Optimizations** - Reduced update frequency for god mode and ESP


### 🎨 UI/UX Improvements
- **Responsive Design** - Works on all screen sizes
- **Icon System** - Lucide icons throughout
- **Smooth Animations** - Fade, slide, and scale transitions
- **Loading States** - Visual feedback for all actions
- **Error Messages** - Clear, actionable error notifications
- **Tooltips** - Helpful hints throughout the interface
- **Keyboard Navigation** - Full keyboard support

---

## Previous Versions (Aerolt)

### [4.1.1]
- Fixed issue where Aerolt wouldn't load due to long load times

### [4.1.0]
- Attempted fix for not spawning at the start of the stage (Bubbet)
- Fixed interactable not spawning
- Fixed resize shooting off the screen

### [4.0.14]
- Changed Discord link

### [4.0.13]
- Fixed users stats not being populated

### [4.0.12]
- Brought the changes from 4.0.10 back

### [4.0.11]
- Added logo to Risk of Options UI
- Cleaned up some code

### [4.0.10]
- Fixed issue where items wouldn't populate due to a nametoken missing from an item

### [4.0.9]
- Fixed logo not being persistent on scene change

### [4.0.8]
**New ESP Enhancements from ruinedyourlife:**
- Added shrine colors
- Added items rarity display
- Fixed shrine & altar names
- Added chest items rarity
- Fixed rename & sort methods
- Added equipment barrel color
- Fixed getitemtierdef
- Added teleporter & duplicator
- WIP: multishop rarity

### [4.0.7]
- Fixed SetUser bug not allowing users to edit other users

### [4.0.3, 4.0.4, 4.0.5, 4.0.6]
- Potential fix to the infinite item bug

### [4.0.2]
- Fixed bug with users not being able to type in IRC Chat

### [4.0.1]
- Updated clients connection to chat server

### [4.0.0]
- Completely revamped the mod

### [3.0.0]
- Not released to public

### [2.0.0]
**Mod developed from the ground up (again)**

#### ESP
- Added ESP for shops
- Changed how ESP displays

#### Player
- Added disable mob spawns
- Reworked character stats so you can edit more stats

#### Monsters
- Added edit items
- Added team change

#### Lobby
- Added currency, lunar, and void marker edit fields
- Added edit inventory for all players

### [1.3.3]
- Fixed item tab crashing

### [1.3.2]
- Fixed monster tab not spawning elites
- Added remove and add item button in players tab

### [1.3.1]
- Fixed monsters spawning in like crazy

### [1.3.0]
- Added custom console
- Added monsters tab
- Added interactable tab
- Fixed player respawn button
- Changed the player tabs format
- Optimized UI code
- Stopped cursor from moving the character when menu is active
- Moved ESP colors to the ESP tab

### [1.2.3]
- Fixed stat modifiers

### [1.2.2]
- Updated for SOV update

### [1.2.1]
- Fixed ESP lag issue

### [1.2.0]
- Added damage and crit modifiers for each level up
- Added equipment tab

### [1.0.0]
- Initial release

---

## Version Numbering

- **Major (X.0.0)** - Complete rewrites, breaking changes
- **Minor (0.X.0)** - New features, significant improvements
- **Patch (0.0.X)** - Bug fixes, minor improvements
