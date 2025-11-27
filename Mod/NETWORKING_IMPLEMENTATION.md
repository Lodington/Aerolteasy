# Networking Implementation Summary

## Files Created

### Models
- `Models/NetworkedCommand.cs` - Command wrapper with sender info and timestamp
- `Models/PermissionLevel.cs` - Enum defining 6 permission levels (None to Host)
- `Models/PermissionRequest.cs` - Request/response models for permission management

### Services
- `Services/PermissionService.cs` - Manages user permissions and command authorization
- `Services/NetworkingService.cs` - Handles network message passing and synchronization

### API Endpoints
- `Services/Endpoints/NetworkStatusEndpoint.cs` - GET network state and connected players
- `Services/Endpoints/PermissionsEndpoint.cs` - GET/POST/DELETE permission management
- `Services/Endpoints/RequestPermissionEndpoint.cs` - POST permission requests from clients

### Modified Files
- `Services/Endpoints/CommandEndpoint.cs` - Updated to use networking and check permissions
- `Services/HttpServer.cs` - Added new endpoints and networking service dependency
- `RoR2DevTool.cs` - Integrated networking service with lifecycle hooks

## Architecture

```
┌─────────────┐
│   WebUI     │
└──────┬──────┘
       │ HTTP
       ▼
┌─────────────────┐
│  HttpServer     │
│  - Endpoints    │
└──────┬──────────┘
       │
       ▼
┌──────────────────────────────┐
│  NetworkingService           │
│  - Send/Receive Commands     │
│  - Permission Validation     │
└──────┬───────────────────────┘
       │
       ├──────────────┬──────────────┐
       ▼              ▼              ▼
┌─────────────┐ ┌──────────┐ ┌──────────────┐
│ Permission  │ │ Command  │ │ RoR2 Network │
│ Service     │ │Processor │ │   Layer      │
└─────────────┘ └──────────┘ └──────────────┘
```

## Key Features

1. **Permission System**
   - 6 levels: None, ReadOnly, Basic, Advanced, Admin, Host
   - Command-level permission requirements
   - Host-controlled authorization

2. **Network Synchronization**
   - Commands sent from any client
   - Host validates and executes
   - Results broadcast to all clients
   - Automatic state synchronization

3. **Security**
   - Server-side validation
   - Permission checks on every command
   - Host cannot be overridden
   - Unknown commands require Admin level

4. **Auto-Approval**
   - Basic permissions auto-approved
   - Advanced/Admin require manual approval
   - Configurable per-command permissions

## Integration Points

### RoR2 Hooks
- `OnStartServer` - Initialize networking as host
- `OnStartClient` - Initialize networking as client
- `OnStopServer` - Cleanup and reset permissions
- `OnStopClient` - Cleanup networking

### Network Messages
- `MSG_COMMAND (1000)` - Command execution
- `MSG_PERMISSION_REQUEST (1001)` - Request permissions
- `MSG_PERMISSION_RESPONSE (1002)` - Permission grant/deny
- `MSG_PERMISSION_UPDATE (1003)` - Broadcast permission changes

## Usage Example

### Host Granting Permission
```csharp
// Via API
POST /api/permissions
{
  "userId": "76561198087654321",
  "level": "Advanced"
}

// Via Code
networkingService.GrantPermission(userId, PermissionLevel.Advanced);
```

### Client Requesting Permission
```csharp
// Via API
POST /api/permissions/request
{
  "level": "Basic"
}

// Via Code
networkingService.RequestPermission(PermissionLevel.Basic);
```

### Sending Networked Command
```csharp
// Via API (automatic)
POST /api/command
{
  "Type": "spawnitem",
  "Data": { "itemName": "Syringe", "count": "10" }
}

// Via Code
var command = new DevCommand { Type = "spawnitem", Data = {...} };
networkingService.SendCommand(command, userId, userName);
```

## Command Permission Mapping

| Command | Required Level |
|---------|---------------|
| Debug commands | ReadOnly |
| Item/Money/Health | Basic |
| Teleport/Spawn | Advanced |
| Stage/Kill/Revive | Admin |
| Permission management | Host |

## Next Steps for UI

1. Add network status indicator
2. Show current permission level
3. Add permission request button (clients)
4. Add permission management panel (host)
5. Disable commands based on permission level
6. Show connected players list
7. Add permission level badges to player list
