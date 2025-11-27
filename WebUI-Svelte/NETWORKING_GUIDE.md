# WebUI Networking Integration Guide

## Quick Start

### 1. Check Network Status

```javascript
async function getNetworkStatus() {
  const response = await fetch('http://localhost:8080/api/network/status');
  const data = await response.json();
  
  return {
    isHost: data.isHost,
    permission: data.currentPermission,
    userId: data.currentUserId,
    userName: data.currentUserName,
    players: data.players
  };
}
```

### 2. Request Permissions (Client)

```javascript
async function requestPermission(level) {
  // level: "ReadOnly", "Basic", "Advanced", "Admin"
  const response = await fetch('http://localhost:8080/api/permissions/request', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ level })
  });
  
  return await response.json();
}
```

### 3. Grant Permissions (Host Only)

```javascript
async function grantPermission(userId, level) {
  const response = await fetch('http://localhost:8080/api/permissions', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, level })
  });
  
  return await response.json();
}
```

### 4. Send Commands (With Permission Check)

```javascript
async function sendCommand(commandType, data) {
  const response = await fetch('http://localhost:8080/api/command', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      Type: commandType,
      Data: data
    })
  });
  
  const result = await response.json();
  
  if (response.status === 403) {
    // Insufficient permissions
    alert(`You need higher permissions to use ${commandType}`);
    return null;
  }
  
  return result;
}
```

## UI Components Needed

### 1. Network Status Indicator

```svelte
<script>
  let networkStatus = { isConnected: false, permission: 'None' };
  
  async function updateStatus() {
    const status = await getNetworkStatus();
    networkStatus = status;
  }
  
  onMount(() => {
    updateStatus();
    setInterval(updateStatus, 5000); // Update every 5 seconds
  });
</script>

<div class="network-status">
  <span class="status-dot" class:connected={networkStatus.isConnected}></span>
  <span>{networkStatus.isConnected ? 'Connected' : 'Offline'}</span>
  <span class="permission-badge">{networkStatus.permission}</span>
</div>
```

### 2. Permission Request Button (Client)

```svelte
<script>
  let requestedLevel = 'Basic';
  
  async function handleRequest() {
    const result = await requestPermission(requestedLevel);
    if (result.success) {
      alert('Permission request sent!');
    }
  }
</script>

{#if !networkStatus.isHost && networkStatus.permission === 'None'}
  <div class="permission-request">
    <select bind:value={requestedLevel}>
      <option value="ReadOnly">Read Only</option>
      <option value="Basic">Basic</option>
      <option value="Advanced">Advanced</option>
      <option value="Admin">Admin</option>
    </select>
    <button on:click={handleRequest}>Request Permission</button>
  </div>
{/if}
```

### 3. Player List with Permissions (Host)

```svelte
<script>
  let players = [];
  
  async function loadPlayers() {
    const status = await getNetworkStatus();
    players = status.players;
  }
  
  async function changePermission(userId, newLevel) {
    await grantPermission(userId, newLevel);
    await loadPlayers();
  }
</script>

{#if networkStatus.isHost}
  <div class="player-list">
    <h3>Connected Players</h3>
    {#each players as player}
      <div class="player-item">
        <span class="player-name">{player.userName}</span>
        {#if player.isHost}
          <span class="host-badge">HOST</span>
        {:else}
          <select 
            value={player.permission}
            on:change={(e) => changePermission(player.userId, e.target.value)}
          >
            <option value="None">None</option>
            <option value="ReadOnly">Read Only</option>
            <option value="Basic">Basic</option>
            <option value="Advanced">Advanced</option>
            <option value="Admin">Admin</option>
          </select>
        {/if}
      </div>
    {/each}
  </div>
{/if}
```

### 4. Command Button with Permission Check

```svelte
<script>
  const commandPermissions = {
    'spawnitem': 'Basic',
    'godmode': 'Advanced',
    'changestage': 'Admin'
  };
  
  function canUseCommand(commandType) {
    const required = commandPermissions[commandType] || 'Admin';
    const levels = ['None', 'ReadOnly', 'Basic', 'Advanced', 'Admin', 'Host'];
    const userLevel = levels.indexOf(networkStatus.permission);
    const requiredLevel = levels.indexOf(required);
    return userLevel >= requiredLevel;
  }
  
  async function handleCommand(commandType, data) {
    if (!canUseCommand(commandType)) {
      alert(`You need ${commandPermissions[commandType]} permission`);
      return;
    }
    
    await sendCommand(commandType, data);
  }
</script>

<button 
  disabled={!canUseCommand('spawnitem')}
  on:click={() => handleCommand('spawnitem', { itemName: 'Syringe', count: '10' })}
>
  Spawn Item
</button>
```

## Permission Levels Reference

| Level | Can Do |
|-------|--------|
| **None** | Nothing - must request permissions |
| **ReadOnly** | View game state, debug info |
| **Basic** | Spawn items, set money/health/level |
| **Advanced** | God mode, teleport, spawn monsters |
| **Admin** | Change stage, kill/revive players |
| **Host** | Everything + manage permissions |

## Command Permission Requirements

```javascript
const COMMAND_PERMISSIONS = {
  // ReadOnly
  'refreshstate': 'ReadOnly',
  'debugplayeritems': 'ReadOnly',
  'debugitems': 'ReadOnly',
  'debuginteractables': 'ReadOnly',
  'debugmonsters': 'ReadOnly',
  
  // Basic
  'spawnitem': 'Basic',
  'setmoney': 'Basic',
  'sethealth': 'Basic',
  'setlevel': 'Basic',
  'configureespoverlay': 'Basic',
  
  // Advanced
  'godmode': 'Advanced',
  'changeplayer': 'Advanced',
  'teleportplayer': 'Advanced',
  'spawnmonster': 'Advanced',
  'spawninteractable': 'Advanced',
  'givemonsterbuff': 'Advanced',
  'givemonsteritem': 'Advanced',
  
  // Admin
  'changestage': 'Admin',
  'killplayer': 'Admin',
  'reviveplayer': 'Admin',
  'chargeteleporter': 'Admin',
  'activateteleporter': 'Admin',
  'skipteleporterevent': 'Admin',
  'spawnteleporter': 'Admin',
  'setplayerstats': 'Admin'
};
```

## Error Handling

```javascript
async function sendCommandSafe(commandType, data) {
  try {
    const response = await fetch('http://localhost:8080/api/command', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Type: commandType, Data: data })
    });
    
    const result = await response.json();
    
    if (response.status === 403) {
      showError('Insufficient permissions for this command');
      return { success: false, error: 'permission_denied' };
    }
    
    if (response.status === 500) {
      showError('Server error executing command');
      return { success: false, error: 'server_error' };
    }
    
    return result;
  } catch (error) {
    showError('Failed to connect to game');
    return { success: false, error: 'connection_failed' };
  }
}
```

## Polling for Updates

```javascript
// Poll network status every 5 seconds
let statusInterval;

onMount(() => {
  statusInterval = setInterval(async () => {
    const status = await getNetworkStatus();
    // Update UI with new status
    updateNetworkStatus(status);
  }, 5000);
});

onDestroy(() => {
  clearInterval(statusInterval);
});
```

## Testing Checklist

- [ ] Network status indicator shows correct state
- [ ] Permission badge updates when granted
- [ ] Commands disabled when insufficient permissions
- [ ] Permission request button appears for clients
- [ ] Host sees player list with permission controls
- [ ] Error messages show for permission denials
- [ ] UI updates when permissions change
- [ ] Works in single player (auto-host)
- [ ] Works as multiplayer host
- [ ] Works as multiplayer client
