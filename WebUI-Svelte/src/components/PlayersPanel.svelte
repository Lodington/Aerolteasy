<script>
  import { gameState, selectedPlayer, getCharacterDisplayName } from '../lib/stores.js';
  import { api } from '../lib/api.js';
  import Button from './Button.svelte';
  import { User, Heart, Shield, Crown } from 'lucide-svelte';

  function selectPlayer(player) {
    selectedPlayer.set(player);
  }

  function getHealthPercentage(player) {
    return player.MaxHealth > 0 ? (player.Health / player.MaxHealth) * 100 : 0;
  }

  function getHealthColor(percentage) {
    if (percentage > 75) return '#4ade80';
    if (percentage > 50) return '#fbbf24';
    if (percentage > 25) return '#fb923c';
    return '#ef4444';
  }

  async function togglePlayerGodMode(player) {
    try {
      await api.toggleGodMode(player.PlayerId, !player.GodModeEnabled);
    } catch (error) {
      console.error('Failed to toggle god mode:', error);
    }
  }

  async function toggleAllGodMode() {
    try {
      const anyEnabled = $gameState.Players.some(p => p.GodModeEnabled);
      await api.toggleGodMode(-1, !anyEnabled);
    } catch (error) {
      console.error('Failed to toggle all god mode:', error);
    }
  }
</script>

<article>
  <header class="flex justify-between items-center">
    <div class="flex gap-4">
      <span class="flex items-center gap-2">
        <User size={16} />
        {$gameState.Players.length} Players
      </span>
      <span class="flex items-center gap-2 text-success">
        <Heart size={16} />
        {$gameState.Players.filter(p => p.IsAlive).length} Alive
      </span>
    </div>
    <Button variant="primary" on:click={toggleAllGodMode}>
      <Shield size={16} />
      Toggle All God Mode
    </Button>
  </header>

  <div class="players-list">
    {#each $gameState.Players as player (player.PlayerId)}
      <div 
        class="player-card" 
        class:selected={$selectedPlayer?.PlayerId === player.PlayerId}
        class:dead={!player.IsAlive}
        on:click={() => selectPlayer(player)}
        on:keydown={(e) => e.key === 'Enter' && selectPlayer(player)}
        role="button"
        tabindex="0"
      >
        <div class="player-header">
          <div class="player-info">
            <h3 class="player-name">{player.PlayerName}</h3>
            <span class="player-character">
              {getCharacterDisplayName(player.CurrentCharacter) || 'No Character'}
            </span>
          </div>
          <div class="player-badges">
            {#if player.GodModeEnabled}
              <span class="badge god-mode">
                <Shield size={12} />
                God
              </span>
            {/if}
            {#if !player.IsAlive}
              <span class="badge dead">Dead</span>
            {/if}
          </div>
        </div>

        {#if player.IsAlive}
          <div class="player-stats">
            <div class="stat-row">
              <span class="stat-label">Health:</span>
              <div class="health-display">
                <span class="health-text">
                  {Math.round(player.Health)}/{Math.round(player.MaxHealth)}
                </span>
                <div class="health-bar">
                  <div 
                    class="health-fill" 
                    style="width: {getHealthPercentage(player)}%; background-color: {getHealthColor(getHealthPercentage(player))}"
                  ></div>
                </div>
              </div>
            </div>
            
            <div class="stat-row">
              <span class="stat-label">Level:</span>
              <span class="stat-value">
                <Crown size={14} />
                {player.Level}
              </span>
            </div>
            
            <div class="stat-row">
              <span class="stat-label">Items:</span>
              <span class="stat-value">
                {Object.values(player.Items || {}).reduce((sum, count) => sum + count, 0)}
              </span>
            </div>
          </div>

          <div class="player-actions">
            <Button 
              variant={player.GodModeEnabled ? 'success' : 'danger'} 
              size="small"
              on:click={(e) => {
                e.stopPropagation();
                togglePlayerGodMode(player);
              }}
            >
              <Shield size={14} />
              {player.GodModeEnabled ? 'God ON' : 'God OFF'}
            </Button>
          </div>
        {/if}
      </div>
    {:else}
      <div class="no-players">
        <User size={48} />
        <p>No players found</p>
        <small class="text-muted">Start a game to see players</small>
      </div>
    {/each}
  </div>
</article>

<style>
/* Players panel using Pico CSS */
.players-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  max-height: 500px;
  overflow-y: auto;
}

.player-card {
  background: var(--pico-card-background-color);
  border: 2px solid var(--pico-muted-border-color);
  border-radius: var(--pico-border-radius);
  padding: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.player-card:hover {
  background: var(--pico-secondary-hover);
  border-color: var(--pico-primary);
}

.player-card.selected {
  border-color: var(--pico-primary);
  background: var(--pico-primary-background);
}

.player-card.dead {
  opacity: 0.6;
  background: rgba(239, 68, 68, 0.1);
  border-color: #ef4444;
}

.player-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.75rem;
}

.player-character {
  font-size: 0.85em;
  color: var(--pico-muted-color);
}

.player-badges {
  display: flex;
  gap: 0.375rem;
}

.badge {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  border-radius: 0.75rem;
  font-size: 0.75em;
  font-weight: bold;
}

.badge.god-mode {
  background: rgba(34, 197, 94, 0.2);
  color: #4ade80;
}

.badge.dead {
  background: rgba(239, 68, 68, 0.2);
  color: #ef4444;
}

.player-stats {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.stat-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.stat-label {
  font-size: 0.85em;
  color: var(--pico-muted-color);
}

.stat-value {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-weight: 500;
}

.health-display {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.health-text {
  font-size: 0.85em;
  min-width: 60px;
  text-align: right;
}

.health-bar {
  width: 60px;
  height: 6px;
  background: var(--pico-muted-border-color);
  border-radius: 3px;
  overflow: hidden;
}

.health-fill {
  height: 100%;
  transition: width 0.3s ease;
}

.player-actions {
  display: flex;
  gap: 0.5rem;
}

.no-players {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2.5rem 1.25rem;
  color: var(--pico-muted-color);
  text-align: center;
}

.no-players p {
  margin: 0.75rem 0 0.25rem 0;
  font-size: 1.1em;
}
</style>