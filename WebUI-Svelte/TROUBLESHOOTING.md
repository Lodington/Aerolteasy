# Troubleshooting Connection Issues

## Quick Checklist

When the built Tauri app doesn't connect to the mod:

### 1. Check if the Mod is Running
- Launch Risk of Rain 2
- Check the BepInEx console for: `"HTTP Server started on port 8080"`
- Look for: `"Successfully launched RoR2 DevTool UI"`

### 2. Check if Port 8080 is Available
Open PowerShell and run:
```powershell
netstat -ano | findstr :8080
```
You should see something like:
```
TCP    127.0.0.1:8080    0.0.0.0:0    LISTENING    <PID>
```

### 3. Test the Connection Manually
Open your browser and go to:
```
http://localhost:8080/api/status
```

You should see JSON like:
```json
{
  "connected": true,
  "gameRunning": false
}
```

### 4. Check Tauri App Console
1. Open the built app
2. Press `F12` or `Ctrl+Shift+I` to open DevTools
3. Look in the Console tab for:
   - `[API] Checking status at: http://localhost:8080/api/status`
   - Any error messages

### 5. Common Issues

**Issue: "Failed to fetch" or CORS error**
- The mod's HTTP server isn't running
- Check BepInEx console for errors
- Make sure Risk of Rain 2 is actually running

**Issue: "Connection check failed: NetworkError"**
- Firewall might be blocking localhost connections
- Try disabling firewall temporarily to test
- Add exception for the Tauri app

**Issue: App shows "Run Required" overlay even when in-game**
- The mod is running but not detecting the game state
- Start a run in Risk of Rain 2
- Check BepInEx console for errors

**Issue: Port 8080 already in use**
- Another application is using port 8080
- Find and close the conflicting application
- Or modify the port in `Mod/Services/HttpServer.cs` (line 16)

### 6. Enable Debug Logging

The app now has enhanced logging. Open DevTools (F12) and you'll see:
- `[API] Checking status at: ...` - Every connection attempt
- `[API] Status response: ...` - HTTP response codes
- `[API] Status data: ...` - The actual game state data

### 7. Test with Development Mode

Instead of the built app, try running in dev mode:
```bash
cd WebUI-Svelte
npm run tauri dev
```

This gives you more detailed error messages.

### 8. Verify File Locations

The mod expects the UI executable at:
```
BepInEx/plugins/RoR2DevTool/ror2-devtool.exe
```

Make sure your built executable is in the correct location.

## Still Not Working?

1. Check BepInEx/LogOutput.log for mod errors
2. Check Windows Event Viewer for application crashes
3. Try rebuilding both the mod and UI from scratch
4. Make sure you're using the latest version of both components
