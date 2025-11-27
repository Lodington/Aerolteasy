# RoR2 DevTool - Networking & Permissions System

## Overview

The mod now supports networked multiplayer sessions where clients can use dev tools with host authorization. All commands are synchronized across the network, and the host controls who can execute what commands through a permission system.

## Permission Levels

The system has 6 permission levels:

### None (0)
- No access to dev tools
- Default for all users until granted permissions

### ReadOnly (1)
- Can view game state
- Can use debug/info commands
- Commands: `refreshstate`, `debugplayeritems`, `debugitems`, `debuginteractables`, `debugmonsters`

### Basic (2)
- ReadOnly permissions +
- Can modify own inventory and basic stats
- Commands: `spawnitem`, `setmoney`, `sethealth`, `setlevel`, `configureespoverlay`

### Advanced (3)
- Basic permissions +
- Can spawn entities and teleport
- Commands: `godmode`, `changeplayer`, `teleportplayer`, `spawnmonster`, `spawninteractable`, `givemonsterbuff`, `givemonsteritem`

### Admin (4)
- Advanced permissions +
- Can modify game state and other players
- Commands: `changestage`, `killplayer`, `reviveplayer`, `chargeteleporter`, `activateteleporter`, `skipteleporterevent`, `spawnteleporter`, `setplayerstats`

### Host (5)
- Full control including permission management
- Can grant/revoke permissions for other users
- Automatically assigned to the server host

## API Endpoints

### Network Status
**GET** `/api/network/status`

Returns current network state and connected players:
```json
{
  "success": true,
  "isHost": true,
  "isClient": false,
  "isConnected": true,
  "currentUserId": "76561198012345678",
  "currentUserName": "PlayerName",
  "currentPermission": "Host",
  "isCurrentUserHost": true,
  "connectedPlayers": 3,
  "players": [
    {
      "userId": "76561198012345678",
      "userName": "Host",
      "permission": "Host",
      "isHost": true
    },
    {
      "userId": "76561198087654321",
      "userName": "Player2",
      "permission": "Basic",
      "isHost": false
    }
  ]
}
```

### Get Permissions
**GET** `/api/permissions`

Returns all user permissions:
```json
{
  "success": true,
  "users": [
    {
      "userId": "76561198012345678",
      "userName": "Host",
      "permission": "Host",
      "isHost": true
    }
  ],
  "hostId": "76561198012345678"
}
```

### Set Permission (Host Only)
**POST** `/api/permissions`

Grant permissions to a user:
```json
{
  "userId": "76561198087654321",
  "level": "Advanced"
}
```

Response:
```json
{
  "success": true,
  "message": "Permission set to Advanced for user 76561198087654321"
}
```

### Revoke Permission (Host Only)
**DELETE** `/api/permissions?userId=76561198087654321`

Response:
```json
{
  "success": true,
  "message": "Permission revoked for user 76561198087654321"
}
```

### Request Permission (Client)
**POST** `/api/permissions/request`

Request permissions from the host:
```json
{
  "level": "Basic"
}
```

Response:
```json
{
  "success": true,
  "message": "Permission request sent for level: Basic"
}
```

Note: Basic level is auto-approved. Advanced and Admin require manual host approval.

### Send Command
**POST** `/api/command`

Send a command (checks permissions automatically):
```json
{
  "Type": "spawnitem",
  "Data": {
    "itemName": "Syringe",
    "count": "10"
  }
}
```

Response (success):
```json
{
  "success": true,
  "message": "Command sent"
}
```

Response (insufficient permissions):
```json
{
  "success": false,
  "message": "Insufficient permissions for command: changestage",
  "requiredPermission": "Higher level required"
}
```

## How It Works

### For Hosts

1. When you start a server, you automatically get Host permissions
2. Other players can request permissions through the UI
3. Basic requests are auto-approved
4. Advanced/Admin requests appear in logs for manual approval
5. Use the permissions API to grant/revoke access

### For Clients

1. Connect to a host running the mod
2. Request permissions through the UI (POST to `/api/permissions/request`)
3. Once approved, you can use commands within your permission level
4. Commands are sent to the host for validation and execution
5. Results are synchronized back to all clients

### Command Flow

1. Client sends command via HTTP API
2. Command endpoint checks user permissions
3. If authorized, command is sent through NetworkingService
4. Host receives and validates the command
5. Host executes the command locally
6. Host broadcasts command to all other clients
7. All clients execute the command for synchronization

## Security

- All commands are validated on the host
- Clients cannot bypass permission checks
- Host permissions cannot be transferred or revoked
- Network messages are validated before execution
- Unknown commands default to Admin permission requirement

## UI Integration

Your WebUI should:

1. Check network status on load (`GET /api/network/status`)
2. Display current permission level
3. Show permission request UI if not host
4. Disable commands that exceed current permission level
5. Show other connected players (if host)
6. Provide permission management UI (if host)

Example UI flow:
```javascript
// Check status
const status = await fetch('http://localhost:8080/api/network/status').then(r => r.json());

if (status.isCurrentUserHost) {
  // Show host controls
  showPermissionManagement();
} else {
  // Show permission request
  if (status.currentPermission === 'None') {
    showPermissionRequestButton();
  }
}

// Request permissions
async function requestPermission(level) {
  await fetch('http://localhost:8080/api/permissions/request', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ level })
  });
}

// Grant permission (host only)
async function grantPermission(userId, level) {
  await fetch('http://localhost:8080/api/permissions', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, level })
  });
}
```

## Testing

### Single Player
- You automatically get Host permissions
- All commands work as before

### Multiplayer Host
1. Start a multiplayer game
2. Check logs for "Host set: [your-id]"
3. Other players can connect and request permissions

### Multiplayer Client
1. Join a host running the mod
2. Request Basic permissions (auto-approved)
3. Try commands within your permission level
4. Request higher permissions if needed

## Troubleshooting

**Commands not working:**
- Check your permission level: `GET /api/network/status`
- Verify you have sufficient permissions for the command
- Check host logs for permission denials

**Permission requests not working:**
- Ensure you're connected to a multiplayer session
- Verify the host has the mod installed
- Check network connectivity

**Commands not syncing:**
- Verify all clients have the mod installed
- Check host logs for network errors
- Ensure NetworkingService initialized properly
