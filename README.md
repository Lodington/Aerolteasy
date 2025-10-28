# Risk of Rain 2 Development Tool

A comprehensive development tool for Risk of Rain 2 that allows real-time game modification through an external web interface.

## Features
- Stage manipulation (change stages, weather, difficulty)
- Character switching during gameplay
- Item spawning and management
- God mode toggle
- Player stats modification
- External web-based UI with no websockets

## Architecture
- **Mod**: C# BepInEx plugin with built-in HTTP API server
- **WebUI**: HTML/CSS/JS interface that connects directly to the mod
- **Communication**: Direct HTTP API communication (no websockets, no file polling)

## Installation
1. Install BepInEx for Risk of Rain 2
2. Build and copy the mod DLL to `BepInEx/plugins/`
3. Open `WebUI/standalone.html` in your browser
4. Start Risk of Rain 2

## Usage
The web interface connects directly to the mod's HTTP API server running on port 8080. Real-time communication with instant command execution and live game state updates.