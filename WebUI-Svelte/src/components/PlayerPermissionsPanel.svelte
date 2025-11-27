<script>
  import { networkStatus } from '../lib/stores.js';
  import { networkApi, getPermissionInfo, PermissionLevels } from '../lib/networkApi.js';
  import Panel from './Panel.svelte';

  let isUpdating = false;
  let updateMessage = '';

  const permissionLevels = [
    PermissionLevels.NONE,
    PermissionLevels.READ_ONLY,
    PermissionLevels.BASIC,
    PermissionLevels.ADVANCED,
    PermissionLevels.ADMIN
  ];

  async function changePermission(userId, newLevel) {
    isUpdating = true;
    updateMessage = '';

    try {
      const result = await networkApi.grantPermission(userId, newLevel);
      if (result.success) {
        updateMessage = `✅ Permission updated successfully`;
        setTimeout(() => updateMessage = '', 3000);
      } else {
        updateMessage = `❌ Failed to update permission`;
      }
    } catch (error) {
      updateMessage = `❌ Error: ${error.message}`;
    } finally {
      isUpdating = false;
    }
  }

  async function revokePermission(userId) {
    if (!confirm('Are you sure you want to revoke this user\'s permissions?')) {
      return;
    }

    isUpdating = true;
    updateMessage = '';

    try {
      const result = await networkApi.revokePermission(userId);
      if (result.success) {
        updateMessage = `✅ Permission revoked`;
        setTimeout(() => updateMessage = '', 3000);
      } else {
        updateMessage = `❌ Failed to revoke permission`;
      }
    } catch (error) {
      updateMessage = `❌ Error: ${error.message}`;
    } finally {
      isUpdating = false;
    }
  }

  $: showPanel = $networkStatus.isHost && $networkStatus.players.length > 1;
  $: otherPlayers = $networkStatus.players.filter(p => !p.isHost);
</script>

{#if showPanel}
  <Panel title="👥 Player Permissions" icon="🔐">
    <div class="space-y-4">
      <div class="alert alert-info">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="stroke-current shrink-0 w-6 h-6">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
        </svg>
        <span class="text-sm">Manage permissions for connected players</span>
      </div>

      {#if updateMessage}
        <div class="alert alert-success">
          <span class="text-sm">{updateMessage}</span>
        </div>
      {/if}

      <div class="space-y-3">
        {#each otherPlayers as player}
          {@const permInfo = getPermissionInfo(player.permission)}
          <div class="card bg-base-200">
            <div class="card-body p-4">
              <div class="flex items-center justify-between gap-4">
                <!-- Player Info -->
                <div class="flex items-center gap-3 flex-1">
                  <div class="avatar placeholder">
                    <div class="bg-primary text-primary-content rounded-full w-10">
                      <span>👤</span>
                    </div>
                  </div>
                  <div>
                    <div class="font-semibold">{player.userName}</div>
                    <div class="text-xs opacity-70">ID: {player.userId.slice(0, 8)}...</div>
                  </div>
                </div>

                <!-- Current Permission Badge -->
                <div class="badge {permInfo.color} gap-1">
                  <span>{permInfo.icon}</span>
                  <span>{player.permission}</span>
                </div>

                <!-- Permission Selector -->
                <select 
                  class="select select-bordered select-sm w-32"
                  value={player.permission}
                  disabled={isUpdating}
                  on:change={(e) => changePermission(player.userId, e.target.value)}
                >
                  {#each permissionLevels as level}
                    <option value={level}>{level}</option>
                  {/each}
                </select>

                <!-- Revoke Button -->
                {#if player.permission !== PermissionLevels.NONE}
                  <button 
                    class="btn btn-error btn-sm btn-square"
                    disabled={isUpdating}
                    on:click={() => revokePermission(player.userId)}
                    title="Revoke all permissions"
                  >
                    🚫
                  </button>
                {/if}
              </div>
            </div>
          </div>
        {/each}

        {#if otherPlayers.length === 0}
          <div class="text-center py-8 opacity-70">
            <div class="text-4xl mb-2">👥</div>
            <p class="text-sm">No other players connected</p>
          </div>
        {/if}
      </div>
    </div>
  </Panel>
{/if}
