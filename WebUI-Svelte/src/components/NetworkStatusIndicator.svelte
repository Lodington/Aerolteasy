<script>
  import { networkStatus } from '../lib/stores.js';
  import { getPermissionInfo } from '../lib/networkApi.js';

  $: permissionInfo = getPermissionInfo($networkStatus.currentPermission);
  $: statusColor = $networkStatus.isConnected ? 'success' : 'error';
</script>

<div class="flex items-center gap-3 bg-base-200 rounded-lg p-3">
  <!-- Connection Status -->
  <div class="flex items-center gap-2">
    <div class="indicator">
      <span class="indicator-item badge badge-xs badge-{statusColor}"></span>
      <div class="w-8 h-8 rounded-full bg-base-300 flex items-center justify-center">
        {$networkStatus.isConnected ? '🌐' : '📡'}
      </div>
    </div>
    <div class="text-sm">
      <div class="font-semibold">
        {$networkStatus.isConnected ? 'Connected' : 'Offline'}
      </div>
      <div class="text-xs opacity-70">
        {#if $networkStatus.isHost}
          Host
        {:else if $networkStatus.isClient}
          Client
        {:else}
          Single Player
        {/if}
      </div>
    </div>
  </div>

  <!-- Divider -->
  <div class="divider divider-horizontal m-0"></div>

  <!-- Permission Badge -->
  <div class="flex items-center gap-2">
    <div class="badge {permissionInfo.color} gap-1">
      <span>{permissionInfo.icon}</span>
      <span>{$networkStatus.currentPermission}</span>
    </div>
    <div class="text-xs opacity-70 hidden sm:block">
      {permissionInfo.description}
    </div>
  </div>

  <!-- Player Count (if multiplayer) -->
  {#if $networkStatus.connectedPlayers > 1}
    <div class="divider divider-horizontal m-0"></div>
    <div class="flex items-center gap-1 text-sm">
      <span>👥</span>
      <span class="font-semibold">{$networkStatus.connectedPlayers}</span>
      <span class="text-xs opacity-70">players</span>
    </div>
  {/if}
</div>

<style>
  .indicator-item {
    animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
  }
</style>
