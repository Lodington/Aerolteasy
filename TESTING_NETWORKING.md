# Testing the Networking System

## Prerequisites

1. Build the mod with the new networking code
2. Install on Risk of Rain 2
3. Have the WebUI running

## Test Scenarios

### Scenario 1: Single Player (Auto-Host)

**Expected Behavior:** You automatically get Host permissions

1. Start a single player game
2. Open WebUI
3. Check network status:
   ```bash
   curl http://localhost:8080/api/network/status
   ```
4. Verify:
   - `isHost: true`
   - `currentPermission: "Host"`
   - `isCurrentUserHost: true`

5. Try any command - should work:
   ```bash
   curl -X POST http://localhost:8080/api/command \
     -H "Content-Type: application/json" \
     -d '{"Type":"spawnitem","Data":{"itemName":"Syringe","count":"10"}}'
   ```

### Scenario 2: Multiplayer Host

**Expected Behavior:** Host controls permissions for all clients

1. Start a multiplayer game as host
2. Check logs for: `"Host set: [your-user-id]"`
3. Check network status - should show `isHost: true`
4. Wait for clients to connect
5. Check player list:
   ```bash
   curl http://localhost:8080/api/permissions
   ```
6. Grant permission to a client:
   ```bash
   curl -X POST http://localhost:8080/api/permissions \
     -H "Content-Type: application/json" \
     -d '{"userId":"CLIENT_USER_ID","level":"Basic"}'
   ```
7. Verify client can now use Basic commands

### Scenario 3: Multiplayer Client

**Expected Behavior:** Client must request permissions

1. Join a multiplayer game (host must have mod installed)
2. Check network status:
   ```bash
   curl http://localhost:8080/api/network/status
   ```
3. Verify:
   - `isHost: false`
   - `isClient: true`
   - `currentPermission: "None"`

4. Request Basic permission:
   ```bash
   curl -X POST http://localhost:8080/api/permissions/request \
     -H "Content-Type: application/json" \
     -d '{"level":"Basic"}'
   ```

5. Should auto-approve (check logs)
6. Try a Basic command:
   ```bash
   curl -X POST http://localhost:8080/api/command \
     -H "Content-Type: application/json" \
     -d '{"Type":"spawnitem","Data":{"itemName":"Syringe","count":"5"}}'
   ```

7. Try an Admin command (should fail):
   ```bash
   curl -X POST http://localhost:8080/api/command \
     -H "Content-Type: application/json" \
     -d '{"Type":"changestage","Data":{"stageName":"blackbeach"}}'
   ```
   Expected: 403 Forbidden

### Scenario 4: Permission Escalation

**Expected Behavior:** Client can request higher permissions

1. As client with Basic permissions
2. Request Advanced:
   ```bash
   curl -X POST http://localhost:8080/api/permissions/request \
     -H "Content-Type: application/json" \
     -d '{"level":"Advanced"}'
   ```

3. Check host logs - should see permission request
4. Host manually grants:
   ```bash
   curl -X POST http://localhost:8080/api/permissions \
     -H "Content-Type: application/json" \
     -d '{"userId":"CLIENT_USER_ID","level":"Advanced"}'
   ```

5. Client checks status - should now have Advanced
6. Client tries Advanced command (should work):
   ```bash
   curl -X POST http://localhost:8080/api/command \
     -H "Content-Type: application/json" \
     -d '{"Type":"godmode","Data":{"enabled":"true"}}'
   ```

### Scenario 5: Command Synchronization

**Expected Behavior:** Commands execute on all clients

1. Setup: Host + 2 clients, both clients have Basic permissions
2. Client 1 spawns item:
   ```bash
   curl -X POST http://localhost:8080/api/command \
     -H "Content-Type: application/json" \
     -d '{"Type":"spawnitem","Data":{"itemName":"Syringe","count":"10"}}'
   ```

3. Check all game instances:
   - Host should see item spawned
   - Client 1 should see item spawned
   - Client 2 should see item spawned

4. Check logs on all instances for command execution

### Scenario 6: Permission Revocation

**Expected Behavior:** Revoked users cannot execute commands

1. Host grants Basic to client
2. Client executes Basic command (works)
3. Host revokes permission:
   ```bash
   curl -X DELETE "http://localhost:8080/api/permissions?userId=CLIENT_USER_ID"
   ```

4. Client tries same command (should fail with 403)

### Scenario 7: Disconnect/Reconnect

**Expected Behavior:** Permissions reset on disconnect

1. Client connects and gets Basic permissions
2. Client disconnects
3. Client reconnects
4. Check permissions - should be None again
5. Must re-request permissions

## Automated Test Script

```bash
#!/bin/bash

echo "Testing RoR2 DevTool Networking..."

# Test 1: Network Status
echo "Test 1: Network Status"
curl -s http://localhost:8080/api/network/status | jq .
echo ""

# Test 2: Permission List
echo "Test 2: Permission List"
curl -s http://localhost:8080/api/permissions | jq .
echo ""

# Test 3: Basic Command (should work if host)
echo "Test 3: Spawn Item Command"
curl -s -X POST http://localhost:8080/api/command \
  -H "Content-Type: application/json" \
  -d '{"Type":"spawnitem","Data":{"itemName":"Syringe","count":"1"}}' | jq .
echo ""

# Test 4: Admin Command (should work if host)
echo "Test 4: Admin Command"
curl -s -X POST http://localhost:8080/api/command \
  -H "Content-Type: application/json" \
  -d '{"Type":"setmoney","Data":{"amount":"1000"}}' | jq .
echo ""

echo "Tests complete!"
```

## Expected Log Messages

### Host Startup
```
[Info   : RoR2 Development Tool] RoR2 Development Tool loaded!
[Info   : RoR2 Development Tool] HTTP Server started on port 8080
[Info   : RoR2 Development Tool] Server started - initializing networking
[Info   : RoR2 Development Tool] Registered server network handlers
[Info   : RoR2 Development Tool] Host set: 76561198012345678
[Info   : RoR2 Development Tool] NetworkingService initialized
```

### Client Connection
```
[Info   : RoR2 Development Tool] Client started - initializing networking
[Info   : RoR2 Development Tool] Registered client network handlers
[Info   : RoR2 Development Tool] NetworkingService initialized
```

### Permission Request (Auto-Approved)
```
[Info   : RoR2 Development Tool] Received permission request from PlayerName for level: Basic
[Info   : RoR2 Development Tool] Set permission for 76561198087654321: Basic
```

### Permission Request (Manual Approval)
```
[Info   : RoR2 Development Tool] Received permission request from PlayerName for level: Advanced
[Info   : RoR2 Development Tool] Permission request from PlayerName requires manual approval
```

### Command Execution
```
[Info   : RoR2 Development Tool] Server received command from PlayerName: spawnitem
[Info   : RoR2 Development Tool] Executing command: spawnitem
```

### Permission Denial
```
[Warning: RoR2 Development Tool] User PlayerName (76561198087654321) lacks permission for command: changestage
```

## Troubleshooting

### Commands Not Working

**Check:**
1. Network status: `curl http://localhost:8080/api/network/status`
2. Your permission level
3. Command permission requirements
4. Game logs for errors

**Fix:**
- Request appropriate permissions
- Verify host has granted permissions
- Check network connectivity

### Permissions Not Syncing

**Check:**
1. All clients have mod installed
2. NetworkingService initialized (check logs)
3. Network connection stable

**Fix:**
- Restart game
- Verify mod installation on all clients
- Check firewall settings

### Commands Not Syncing to Clients

**Check:**
1. Host logs for "Broadcasting command"
2. Client logs for "Client received command"
3. Network message handlers registered

**Fix:**
- Verify all clients have same mod version
- Check for network errors in logs
- Restart multiplayer session

## Performance Testing

Monitor these metrics during gameplay:

1. **Network Message Rate**
   - Should be low when idle
   - Spikes during command execution
   - Check logs for message counts

2. **Command Latency**
   - Measure time from API call to execution
   - Should be < 100ms on LAN
   - Check for delays in logs

3. **Memory Usage**
   - Monitor for memory leaks
   - Check permission dictionary size
   - Verify cleanup on disconnect

## Security Testing

Try these attacks (should all fail):

1. **Permission Bypass**
   - Client tries to execute Admin command without permission
   - Expected: 403 Forbidden

2. **Host Impersonation**
   - Client tries to grant themselves Host permission
   - Expected: Rejected by server

3. **Command Injection**
   - Send malformed command data
   - Expected: Handled gracefully, no crash

4. **Permission Escalation**
   - Client with Basic tries to grant themselves Admin
   - Expected: Rejected (only Host can grant)
