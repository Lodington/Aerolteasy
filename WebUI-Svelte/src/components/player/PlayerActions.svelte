<script>
  import { selectedPlayer, gameState } from '../../lib/stores.js';
  import Button from '../Button.svelte';
  import { Shield, Target, Heart, Zap, RotateCcw, Coins, TrendingUp, Users, User, Navigation } from 'lucide-svelte';
  import { api } from '../../lib/api.js';
  
  export let refreshGameState = () => {};

  // Get list of other players for teleport target
  $: otherPlayers = $gameState.Players?.filter(p => p.PlayerId !== $selectedPlayer?.PlayerId) || [];
  let teleportTargetId = null;

  // Action target mode
  let actionTarget = 'selected'; // 'selected' or 'all'

  async function togglePlayerGodMode(targetAll = false) {
    try {
      const playerId = targetAll ? -1 : $selectedPlayer.PlayerId;
      await api.toggleGodMode(playerId, !$selectedPlayer.GodModeEnabled);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to toggle god mode:', error);
    }
  }

  async function killPlayer(targetAll = false) {
    try {
      const playerId = targetAll ? -1 : $selectedPlayer.PlayerId;
      await api.killPlayer(playerId);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to kill player:', error);
    }
  }

  async function revivePlayer(targetAll = false) {
    try {
      const playerId = targetAll ? -1 : $selectedPlayer.PlayerId;
      await api.revivePlayer(playerId);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to revive player:', error);
    }
  }

  async function healPlayer(targetAll = false) {
    try {
      const playerId = targetAll ? -1 : $selectedPlayer.PlayerId;
      await api.setHealth(100, playerId); // Full heal
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to heal player:', error);
    }
  }

  async function levelUpPlayer(targetAll = false) {
    try {
      const playerId = targetAll ? -1 : $selectedPlayer.PlayerId;
      const currentLevel = $selectedPlayer.Level || 1;
      await api.setLevel(currentLevel + 1, playerId);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to level up player:', error);
    }
  }

  async function giveMoney(amount, targetAll = false) {
    try {
      const playerId = targetAll ? -1 : $selectedPlayer.PlayerId;
      await api.sendCommand({
        Type: 'setmoney',
        Data: { amount, playerId }
      });
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to give money:', error);
    }
  }

  // Execute action based on target mode
  function executeAction(actionFn) {
    actionFn(actionTarget === 'all');
  }

  // Execute money action with specific amount
  function executeMoneyAction(amount) {
    giveMoney(amount, actionTarget === 'all');
  }

  // Teleport selected player to another player
  async function teleportToPlayer() {
    if (!teleportTargetId) return;
    
    try {
      // Find target player
      const targetPlayer = $gameState.Players.find(p => p.PlayerId === parseInt(teleportTargetId));
      if (!targetPlayer) {
        console.error('Target player not found');
        return;
      }

      // Get target player's position (we'll need to add this to game state)
      // For now, use a command to teleport
      await api.sendCommand({
        Type: 'teleporttoPlayer',
        Data: { 
          playerId: $selectedPlayer.PlayerId,
          targetPlayerId: targetPlayer.PlayerId
        }
      });
      
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to teleport player:', error);
    }
  }

  // Teleport all other players to selected player
  async function teleportAllToMe() {
    try {
      await api.sendCommand({
        Type: 'teleportallto',
        Data: { 
          targetPlayerId: $selectedPlayer.PlayerId
        }
      });
      
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to teleport all players:', error);
    }
  }
</script>

<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <!-- Header with Target Selection -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
      <h3 class="card-title">⚡ Quick Actions</h3>
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Apply To:</span>
        </label>
        <div class="join">
          <button 
            class="btn join-item {actionTarget === 'selected' ? 'btn-primary' : 'btn-outline'}"
            on:click={() => actionTarget = 'selected'}
          >
            <User size={14} />
            This Player
          </button>
          <button 
            class="btn join-item {actionTarget === 'all' ? 'btn-primary' : 'btn-outline'}"
            on:click={() => actionTarget = 'all'}
          >
            <Users size={14} />
            All Players
          </button>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- Core Actions -->
      <div class="space-y-3">
        <h4 class="font-semibold text-base-content border-b border-base-300 pb-2">Core Actions</h4>
        <div class="space-y-2">
          <button 
            class="btn w-full {$selectedPlayer.GodModeEnabled ? 'btn-success' : 'btn-outline'}"
            on:click={() => executeAction(togglePlayerGodMode)}
          >
            <Shield size={16} />
            {$selectedPlayer.GodModeEnabled ? 'God Mode ON' : 'God Mode OFF'}
          </button>

          {#if $selectedPlayer.IsAlive}
            <button class="btn btn-error w-full" on:click={() => executeAction(killPlayer)}>
              <Target size={16} />
              Kill Player
            </button>
          {:else}
            <button class="btn btn-success w-full" on:click={() => executeAction(revivePlayer)}>
              <Heart size={16} />
              Revive Player
            </button>
          {/if}

          <button class="btn btn-success w-full" on:click={() => executeAction(healPlayer)}>
            <Heart size={16} />
            Full Heal
          </button>
        </div>
      </div>

      <!-- Progression Actions -->
      <div class="space-y-3">
        <h4 class="font-semibold text-base-content border-b border-base-300 pb-2">Progression</h4>
        <div class="space-y-2">
          <button class="btn btn-accent w-full" on:click={() => executeAction(levelUpPlayer)}>
            <TrendingUp size={16} />
            Level Up (+1)
          </button>

          <button class="btn btn-warning w-full" on:click={() => executeMoneyAction(100)}>
            <Coins size={16} />
            +100 Money
          </button>

          <button class="btn btn-warning w-full" on:click={() => executeMoneyAction(500)}>
            <Coins size={16} />
            +500 Money
          </button>
        </div>
      </div>

      <!-- Utility Actions -->
      <div class="space-y-3">
        <h4 class="font-semibold text-base-content border-b border-base-300 pb-2">Utility</h4>
        <div class="space-y-2">
          <button class="btn btn-outline w-full" on:click={refreshGameState}>
            <RotateCcw size={16} />
            Refresh Data
          </button>
        </div>
      </div>
    </div>

    <!-- Teleport Section -->
    <div class="divider">Teleport</div>
    
    {#if otherPlayers.length > 0}
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Teleport To Player -->
        <div class="card bg-base-200">
          <div class="card-body p-4">
            <h4 class="font-semibold text-sm mb-2 flex items-center gap-2">
              <Navigation size={16} />
              Teleport To Player
            </h4>
            <div class="form-control">
              <select 
                class="select select-bordered select-sm w-full"
                bind:value={teleportTargetId}
              >
                <option value={null}>Select player...</option>
                {#each otherPlayers as player}
                  <option value={player.PlayerId}>
                    {player.PlayerName} (Level {player.Level})
                  </option>
                {/each}
              </select>
            </div>
            <button 
              class="btn btn-primary btn-sm mt-2"
              disabled={!teleportTargetId}
              on:click={teleportToPlayer}
            >
              <Navigation size={14} />
              Teleport
            </button>
          </div>
        </div>

        <!-- Teleport All To Me -->
        <div class="card bg-base-200">
          <div class="card-body p-4">
            <h4 class="font-semibold text-sm mb-2 flex items-center gap-2">
              <Users size={16} />
              Teleport All To Me
            </h4>
            <p class="text-xs opacity-70 mb-2">
              Bring all other players to {$selectedPlayer.PlayerName}'s location
            </p>
            <button 
              class="btn btn-secondary btn-sm"
              on:click={teleportAllToMe}
            >
              <Users size={14} />
              Teleport All Here
            </button>
          </div>
        </div>
      </div>
    {:else}
      <div class="alert alert-info">
        <div class="flex items-center gap-2">
          <User size={16} />
          <span class="text-sm">
            Teleport features are available in multiplayer games with 2+ players
          </span>
        </div>
      </div>
    {/if}

    <!-- Target Info -->
    <div class="alert alert-info mt-4">
      <div class="flex items-center gap-2">
        <div class="text-info">
          {#if actionTarget === 'selected'}
            <User size={16} />
          {:else}
            <Users size={16} />
          {/if}
        </div>
        <span class="text-sm">
          Actions will be applied to: 
          <strong>
            {actionTarget === 'selected' ? $selectedPlayer.PlayerName : 'All Players'}
          </strong>
        </span>
      </div>
    </div>
  </div>
</div>

<!-- DaisyUI handles all styling -->