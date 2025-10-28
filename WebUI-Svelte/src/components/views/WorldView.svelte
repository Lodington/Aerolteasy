<script>
  import { gameState } from '../../lib/stores.js';
  import Button from '../Button.svelte';
  import { Globe, Zap, Target, Package } from 'lucide-svelte';
  import { onMount, onDestroy } from 'svelte';
  import { api } from '../../lib/api.js';
  
  // Import ESP component
  import ESPView from '../player/ESPView.svelte';

  // UI State
  let isProcessing = false;

  async function refreshGameState() {
    try {
      isProcessing = true;
      await api.refreshGameState();
      const state = await api.getGameState();
      if (state) {
        gameState.set(state);
      }
    } catch (error) {
      console.error('Failed to refresh game state:', error);
    } finally {
      isProcessing = false;
    }
  }

  // Auto-refresh every 5 seconds for ESP
  let refreshInterval;
  
  onMount(() => {
    refreshInterval = setInterval(refreshGameState, 5000);
  });

  // Cleanup interval on component destroy
  onDestroy(() => {
    if (refreshInterval) {
      clearInterval(refreshInterval);
    }
  });
</script>

<section class="world-view">
  <header class="view-header">
    <h1>🌍 ESP & World Tracking</h1>
    <p>Real-time world state monitoring and ESP overlay controls</p>
  </header>

  <article>
    <header class="world-stats-header">
      <div class="world-stats">
        <div class="stat-item">
          <Globe size={16} />
          <span>Stage: {$gameState?.CurrentStage || 'Unknown'}</span>
        </div>
        <div class="stat-item">
          <Target size={16} />
          <span>Enemies: {$gameState?.TotalEnemiesAlive || 0}</span>
        </div>
        <div class="stat-item">
          <Package size={16} />
          <span>Interactables: {$gameState?.Interactables?.length || 0}</span>
        </div>
      </div>
      
      <Button size="small" variant="secondary" on:click={refreshGameState} disabled={isProcessing}>
        <Zap size={14} />
        {isProcessing ? 'Refreshing...' : 'Refresh ESP Data'}
      </Button>
    </header>

    <ESPView {refreshGameState} />
  </article>
</section>

<style>
/* World View using Pico CSS */
.world-view {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.view-header {
  text-align: center;
  margin-bottom: 1rem;
}

.view-header h1 {
  color: var(--pico-primary);
  margin: 0 0 0.5rem 0;
}

.view-header p {
  color: var(--pico-muted-color);
  margin: 0;
}

.world-stats-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid var(--pico-muted-border-color);
}

.world-stats {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--pico-color);
  font-size: 0.875rem;
  font-weight: 500;
}

.stat-item :global(svg) {
  color: var(--pico-primary);
}

@media (max-width: 768px) {
  .world-stats-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .world-stats {
    gap: 1rem;
  }
  
  .stat-item {
    font-size: 0.8125rem;
  }
}
</style>