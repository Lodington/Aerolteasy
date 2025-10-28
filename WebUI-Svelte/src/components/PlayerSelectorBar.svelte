<script>
  import { gameState, selectedPlayerId, getCharacterDisplayName } from '../lib/stores.js';
  import { User, Shield, Crown, Heart, Activity, Zap, Star, Skull, Target } from 'lucide-svelte';

  function selectPlayer(player) {
    selectedPlayerId.set(player.PlayerId);
  }

  function getHealthPercentage(player) {
    return player.MaxHealth > 0 ? (player.Health / player.MaxHealth) * 100 : 0;
  }

  function getShieldPercentage(player) {
    return player.MaxShield > 0 ? (player.Shield / player.MaxShield) * 100 : 0;
  }

  function getHealthColor(percentage) {
    if (percentage > 75) return 'progress-success';
    if (percentage > 50) return 'progress-warning';
    if (percentage > 25) return 'progress-error';
    return 'progress-error';
  }

  function formatNumber(num) {
    if (num >= 1000000) return (num / 1000000).toFixed(1) + 'M';
    if (num >= 1000) return (num / 1000).toFixed(1) + 'K';
    return Math.round(num).toString();
  }

  function getPlayerRank(player, allPlayers) {
    const sorted = [...allPlayers].sort((a, b) => b.Level - a.Level || b.Experience - a.Experience);
    return sorted.findIndex(p => p.PlayerId === player.PlayerId) + 1;
  }
</script>

<!-- Enhanced Player Selector -->
<div class="bg-gradient-to-r from-base-200 to-base-300 rounded-xl shadow-xl border border-base-300">
  <div class="p-6">
    <!-- Header Section -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
      <div class="flex items-center gap-3">
        <div class="avatar placeholder">
          <div class="bg-primary text-primary-content rounded-xl w-12">
            <User size={24} />
          </div>
        </div>
        <div>
          <h2 class="text-xl font-bold flex items-center gap-2">
            Player Selection
            <div class="badge badge-neutral">{$gameState?.Players?.length || 0}</div>
          </h2>
          <p class="text-sm opacity-70">Choose a player to manage their stats, items, and abilities</p>
        </div>
      </div>
      
      {#if $selectedPlayerId}
        <div class="flex items-center gap-2">
          <div class="badge badge-success gap-2">
            <Target size={14} />
            Active Player
          </div>
        </div>
      {/if}
    </div>
    
    <!-- Players Grid -->
    {#if $gameState?.Players && $gameState.Players.length > 0}
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {#each $gameState.Players as player, index (player.PlayerId)}
          {@const healthPercent = getHealthPercentage(player)}
          {@const shieldPercent = getShieldPercentage(player)}
          {@const isSelected = $selectedPlayerId === player.PlayerId}
          {@const playerRank = getPlayerRank(player, $gameState.Players)}
          
          <div 
            class="group relative overflow-hidden rounded-xl transition-all duration-300 cursor-pointer transform hover:scale-105 {isSelected ? 'ring-4 ring-primary ring-opacity-60 shadow-2xl' : 'hover:shadow-xl'}"
            class:opacity-75={!player.IsAlive}
            on:click={() => selectPlayer(player)}
            on:keydown={(e) => e.key === 'Enter' && selectPlayer(player)}
            role="button"
            tabindex="0"
            title="Select {player.PlayerName}"
          >
            <!-- Background Gradient -->
            <div class="absolute inset-0 bg-gradient-to-br from-base-100 to-base-200 {isSelected ? 'from-primary/10 to-primary/5' : ''}"></div>
            
            <!-- Content -->
            <div class="relative p-5 space-y-4">
              <!-- Top Row: Avatar, Name, Rank -->
              <div class="flex items-start justify-between">
                <div class="flex items-center gap-3">
                  <!-- Character Avatar -->
                  <div class="relative">
                    <div class="avatar placeholder">
                      <div class="bg-gradient-to-br from-neutral to-neutral-focus text-neutral-content rounded-xl w-14 h-14 {isSelected ? 'ring-2 ring-primary' : ''}">
                        {#if player.CharacterIcon}
                          {#if player.CharacterIcon.startsWith('data:image')}
                            <img src={player.CharacterIcon} alt={player.CurrentCharacter} class="rounded-xl" />
                          {:else}
                            <span class="text-xl">{player.CharacterIcon}</span>
                          {/if}
                        {:else}
                          <User size={24} />
                        {/if}
                      </div>
                    </div>
                    
                    <!-- Status Indicator -->
                    <div class="absolute -top-1 -right-1">
                      {#if !player.IsAlive}
                        <div class="tooltip tooltip-left" data-tip="Dead">
                          <div class="badge badge-error badge-sm p-1">
                            <Skull size={10} />
                          </div>
                        </div>
                      {:else if player.GodModeEnabled}
                        <div class="tooltip tooltip-left" data-tip="God Mode">
                          <div class="badge badge-success badge-sm p-1">
                            <Shield size={10} />
                          </div>
                        </div>
                      {:else}
                        <div class="w-3 h-3 bg-success rounded-full border-2 border-base-100"></div>
                      {/if}
                    </div>
                  </div>
                  
                  <!-- Player Info -->
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center gap-2 mb-1">
                      <h3 class="font-bold text-base truncate">{player.PlayerName}</h3>
                      {#if playerRank === 1}
                        <div class="tooltip" data-tip="Highest Level">
                          <Star size={14} class="text-warning" />
                        </div>
                      {/if}
                    </div>
                    <p class="text-xs opacity-70 truncate">
                      {getCharacterDisplayName(player.CurrentCharacter) || 'No Character'}
                    </p>
                  </div>
                </div>
                
                <!-- Rank Badge -->
                <div class="badge badge-outline badge-sm">#{playerRank}</div>
              </div>

              <!-- Stats Row -->
              <div class="grid grid-cols-2 gap-3 text-xs">
                <div class="flex items-center gap-2">
                  <Crown size={12} class="text-warning" />
                  <span class="font-medium">Level {player.Level}</span>
                </div>
                <div class="flex items-center gap-2">
                  <Zap size={12} class="text-info" />
                  <span>{formatNumber(player.Experience)} XP</span>
                </div>
              </div>

              <!-- Health & Shield Bars -->
              {#if player.IsAlive}
                <div class="space-y-2">
                  <!-- Health Bar -->
                  <div class="flex items-center gap-2">
                    <Heart size={12} class="text-error flex-shrink-0" />
                    <div class="flex-1">
                      <div class="flex justify-between text-xs mb-1">
                        <span>Health</span>
                        <span class="font-medium">{Math.round(player.Health)}/{Math.round(player.MaxHealth)}</span>
                      </div>
                      <progress 
                        class="progress {getHealthColor(healthPercent)} w-full h-2" 
                        value={healthPercent} 
                        max="100"
                      ></progress>
                    </div>
                  </div>

                  <!-- Shield Bar (if applicable) -->
                  {#if player.MaxShield > 0}
                    <div class="flex items-center gap-2">
                      <Shield size={12} class="text-info flex-shrink-0" />
                      <div class="flex-1">
                        <div class="flex justify-between text-xs mb-1">
                          <span>Shield</span>
                          <span class="font-medium">{Math.round(player.Shield)}/{Math.round(player.MaxShield)}</span>
                        </div>
                        <progress 
                          class="progress progress-info w-full h-2" 
                          value={shieldPercent} 
                          max="100"
                        ></progress>
                      </div>
                    </div>
                  {/if}
                </div>
              {:else}
                <div class="flex items-center justify-center py-4">
                  <div class="badge badge-error gap-2">
                    <Skull size={12} />
                    Player is Dead
                  </div>
                </div>
              {/if}

              <!-- Selection Indicator -->
              {#if isSelected}
                <div class="absolute top-2 right-2">
                  <div class="badge badge-primary badge-sm gap-1">
                    <Target size={10} />
                    Selected
                  </div>
                </div>
              {/if}
            </div>
          </div>
        {/each}
      </div>
    {:else}
      <!-- Empty State -->
      <div class="text-center py-16">
        <div class="inline-flex items-center justify-center w-20 h-20 bg-base-300 rounded-full mb-6">
          <User size={32} class="opacity-50" />
        </div>
        <h3 class="text-xl font-bold mb-2">No Players Available</h3>
        <p class="text-base-content/70 mb-4">Players will appear here when they join the game session</p>
        <div class="text-sm opacity-60">
          💡 Tip: Use <code class="bg-base-300 px-2 py-1 rounded">generateMockData()</code> in console for testing
        </div>
      </div>
    {/if}
  </div>
</div>