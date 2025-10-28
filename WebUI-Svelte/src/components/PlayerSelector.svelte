<script>
  import { gameState, selectedPlayer, selectedPlayerId, getCharacterDisplayName } from '../lib/stores.js';
  import { User, Shield, Crown } from 'lucide-svelte';

  function selectPlayer(player) {
    selectedPlayerId.set(player.PlayerId);
  }

  function getHealthPercentage(player) {
    return player.MaxHealth > 0 ? (player.Health / player.MaxHealth) * 100 : 0;
  }
</script>

<div class="card bg-base-200 shadow-lg">
  <div class="card-body">
    <h2 class="card-title">
      <User size={20} />
      Select Player
    </h2>
    
    <div class="space-y-2 max-h-96 overflow-y-auto">
      {#each $gameState.Players as player (player.PlayerId)}
        <div 
          class="card bg-base-100 shadow-sm hover:shadow-md transition-all cursor-pointer border-2 {$selectedPlayer?.PlayerId === player.PlayerId ? 'border-primary' : 'border-transparent'}"
          class:opacity-60={!player.IsAlive}
          on:click={() => selectPlayer(player)}
          on:keydown={(e) => e.key === 'Enter' && selectPlayer(player)}
          role="button"
          tabindex="0"
        >
          <div class="card-body p-3">
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <h3 class="font-semibold text-sm">{player.PlayerName}</h3>
                <p class="text-xs opacity-70 truncate">
                  {getCharacterDisplayName(player.CurrentCharacter) || 'No Character'}
                </p>
              </div>
              
              <div class="flex items-center gap-2">
                <!-- Status Badges -->
                {#if player.GodModeEnabled}
                  <div class="tooltip tooltip-left" data-tip="God Mode">
                    <div class="badge badge-success badge-sm">
                      <Shield size={10} />
                    </div>
                  </div>
                {/if}
                
                <!-- Level Badge -->
                <div class="badge badge-primary badge-sm gap-1">
                  <Crown size={10} />
                  {player.Level}
                </div>
                
                <!-- Health Status -->
                {#if !player.IsAlive}
                  <div class="badge badge-error badge-sm">Dead</div>
                {:else}
                  <div class="flex items-center gap-1">
                    <progress 
                      class="progress progress-error w-8 h-2" 
                      value={getHealthPercentage(player)} 
                      max="100"
                    ></progress>
                    <span class="text-xs">{Math.round(getHealthPercentage(player))}%</span>
                  </div>
                {/if}
              </div>
            </div>
          </div>
        </div>
      {:else}
        <div class="hero min-h-[200px]">
          <div class="hero-content text-center">
            <div>
              <User size={32} class="mx-auto mb-2 opacity-50" />
              <h3 class="font-bold">No Players Found</h3>
              <p class="text-sm opacity-70">Players will appear when they join the game</p>
            </div>
          </div>
        </div>
      {/each}
    </div>
  </div>
</div>

<!-- DaisyUI handles all the styling -->