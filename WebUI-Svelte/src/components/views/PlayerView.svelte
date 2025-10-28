<script>
  import PlayerSelectorBar from '../PlayerSelectorBar.svelte';
  import PlayerManagement from '../PlayerManagement.svelte';
</script>

<div class="player-view">
  <div class="player-selector-section">
    <PlayerSelectorBar />
  </div>
  <div class="player-management-section">
    <PlayerManagement />
  </div>
</div>

<style>
.player-view {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  max-width: 100%;
}

.player-selector-section {
  flex-shrink: 0;
}

.player-management-section {
  flex: 1;
  min-height: 0;
}

@media (max-width: 768px) {
  .player-view {
    gap: 1rem;
  }
}
</style>