<script>
  import { selectedPlayer } from '../../lib/stores.js';
  import Button from '../Button.svelte';
  import { Shield, Target, Heart, Zap, RotateCcw, Coins, TrendingUp, Users, User } from 'lucide-svelte';
  import { api } from '../../lib/api.js';
  
  export let refreshGameState = () => {};

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

  async function giveMoney(amount = 100, targetAll = false) {
    try {
      if (targetAll) {
        await api.setMoney(amount); // Team money
      } else {
        // For individual players, we'll add items that give money or use team money
        await api.setMoney(amount);
      }
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to give money:', error);
    }
  }

  // Execute action based on target mode
  function executeAction(actionFn) {
    actionFn(actionTarget === 'all');
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

          <button class="btn btn-warning w-full" on:click={() => executeAction(() => giveMoney(100, actionTarget === 'all'))}>
            <Coins size={16} />
            +100 Money
          </button>

          <button class="btn btn-warning w-full" on:click={() => executeAction(() => giveMoney(500, actionTarget === 'all'))}>
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