<script>
  import NetworkStatusIndicator from '../NetworkStatusIndicator.svelte';
  import PermissionRequestPanel from '../PermissionRequestPanel.svelte';
  import PlayerPermissionsPanel from '../PlayerPermissionsPanel.svelte';
  import Panel from '../Panel.svelte';
  import { networkStatus } from '../../lib/stores.js';
  import { getPermissionInfo } from '../../lib/networkApi.js';

  $: permissionInfo = getPermissionInfo($networkStatus.currentPermission);
</script>

<div class="space-y-6">
  <!-- Page Header -->
  <div class="flex items-center justify-between">
    <div>
      <h1 class="text-3xl font-bold flex items-center gap-3">
        <span>🌐</span>
        <span>Network & Permissions</span>
      </h1>
      <p class="text-sm opacity-70 mt-1">Manage multiplayer permissions and network status</p>
    </div>
  </div>

  <!-- Network Status Card -->
  <NetworkStatusIndicator />

  <!-- Current User Info -->
  <Panel title="👤 Your Information" icon="ℹ️">
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div class="stat bg-base-200 rounded-lg">
        <div class="stat-title">User Name</div>
        <div class="stat-value text-2xl">{$networkStatus.currentUserName || 'Unknown'}</div>
        <div class="stat-desc">Your display name</div>
      </div>

      <div class="stat bg-base-200 rounded-lg">
        <div class="stat-title">User ID</div>
        <div class="stat-value text-lg font-mono">
          {$networkStatus.currentUserId ? $networkStatus.currentUserId.slice(0, 16) + '...' : 'N/A'}
        </div>
        <div class="stat-desc">Your unique identifier</div>
      </div>

      <div class="stat bg-base-200 rounded-lg">
        <div class="stat-title">Permission Level</div>
        <div class="stat-value text-2xl flex items-center gap-2">
          <span>{permissionInfo.icon}</span>
          <span>{$networkStatus.currentPermission}</span>
        </div>
        <div class="stat-desc">{permissionInfo.description}</div>
      </div>

      <div class="stat bg-base-200 rounded-lg">
        <div class="stat-title">Role</div>
        <div class="stat-value text-2xl">
          {#if $networkStatus.isHost}
            👑 Host
          {:else if $networkStatus.isClient}
            👥 Client
          {:else}
            🎮 Solo
          {/if}
        </div>
        <div class="stat-desc">
          {#if $networkStatus.isHost}
            You control permissions
          {:else if $networkStatus.isClient}
            Connected to host
          {:else}
            Single player mode
          {/if}
        </div>
      </div>
    </div>
  </Panel>

  <!-- Permission Request (for clients) -->
  <PermissionRequestPanel />

  <!-- Player Management (for hosts) -->
  <PlayerPermissionsPanel />

  <!-- Permission Levels Reference -->
  <Panel title="📚 Permission Levels Reference" icon="📖">
    <div class="overflow-x-auto">
      <table class="table table-zebra">
        <thead>
          <tr>
            <th>Level</th>
            <th>Icon</th>
            <th>Description</th>
            <th>Capabilities</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td><span class="badge badge-ghost">None</span></td>
            <td>🚫</td>
            <td>No permissions</td>
            <td class="text-sm opacity-70">Must request permissions from host</td>
          </tr>
          <tr>
            <td><span class="badge badge-info">ReadOnly</span></td>
            <td>👁️</td>
            <td>View only</td>
            <td class="text-sm opacity-70">View game state, debug info</td>
          </tr>
          <tr>
            <td><span class="badge badge-success">Basic</span></td>
            <td>✅</td>
            <td>Basic commands</td>
            <td class="text-sm opacity-70">Spawn items, set money/health/level</td>
          </tr>
          <tr>
            <td><span class="badge badge-warning">Advanced</span></td>
            <td>⚡</td>
            <td>Advanced commands</td>
            <td class="text-sm opacity-70">God mode, teleport, spawn monsters</td>
          </tr>
          <tr>
            <td><span class="badge badge-error">Admin</span></td>
            <td>⚠️</td>
            <td>Admin commands</td>
            <td class="text-sm opacity-70">Change stage, kill/revive players</td>
          </tr>
          <tr>
            <td><span class="badge badge-primary">Host</span></td>
            <td>👑</td>
            <td>Full control</td>
            <td class="text-sm opacity-70">Everything + manage permissions</td>
          </tr>
        </tbody>
      </table>
    </div>
  </Panel>

  <!-- Connected Players List -->
  {#if $networkStatus.players.length > 0}
    <Panel title="👥 Connected Players" icon="🌐">
      <div class="space-y-2">
        {#each $networkStatus.players as player}
          {@const playerPermInfo = getPermissionInfo(player.permission)}
          <div class="flex items-center justify-between p-3 bg-base-200 rounded-lg">
            <div class="flex items-center gap-3">
              <div class="avatar placeholder">
                <div class="bg-primary text-primary-content rounded-full w-10">
                  <span>{player.isHost ? '👑' : '👤'}</span>
                </div>
              </div>
              <div>
                <div class="font-semibold flex items-center gap-2">
                  {player.userName}
                  {#if player.isHost}
                    <span class="badge badge-primary badge-sm">HOST</span>
                  {/if}
                </div>
                <div class="text-xs opacity-70">ID: {player.userId.slice(0, 12)}...</div>
              </div>
            </div>
            <div class="badge {playerPermInfo.color} gap-1">
              <span>{playerPermInfo.icon}</span>
              <span>{player.permission}</span>
            </div>
          </div>
        {/each}
      </div>
    </Panel>
  {/if}

  <!-- Help Section -->
  <Panel title="❓ Help & Information" icon="💡">
    <div class="space-y-4">
      <div class="alert alert-info">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="stroke-current shrink-0 w-6 h-6">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
        </svg>
        <div class="text-sm">
          <p class="font-semibold mb-1">How Networking Works:</p>
          <ul class="list-disc list-inside space-y-1 opacity-80">
            <li>The host automatically gets full permissions</li>
            <li>Clients must request permissions to use dev tools</li>
            <li>Basic permissions are auto-approved</li>
            <li>Advanced/Admin permissions require host approval</li>
            <li>All commands are validated by the host</li>
            <li>Commands are synchronized across all clients</li>
          </ul>
        </div>
      </div>

      {#if $networkStatus.isHost}
        <div class="alert alert-success">
          <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <div class="text-sm">
            <p class="font-semibold">You are the host!</p>
            <p class="opacity-80">You can grant or revoke permissions for other players using the Player Permissions panel above.</p>
          </div>
        </div>
      {:else if $networkStatus.isClient}
        <div class="alert alert-warning">
          <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
          <div class="text-sm">
            <p class="font-semibold">You are a client</p>
            <p class="opacity-80">Request permissions from the host to use dev tools. Commands you execute will be validated by the host.</p>
          </div>
        </div>
      {/if}
    </div>
  </Panel>
</div>
