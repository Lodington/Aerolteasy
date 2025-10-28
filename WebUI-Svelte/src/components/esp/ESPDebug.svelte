<script>
  export let gameState;
  export let monsters = [];
  export let interactables = [];
  export let items = [];
  export let filteredMonsters = [];
  export let filteredInteractables = [];
  export let filteredItems = [];
  export let maxDistance = 100;
</script>

<div class="collapse collapse-arrow bg-base-200 shadow-lg">
  <input type="checkbox" />
  <div class="collapse-title text-lg font-medium">
    🔍 Debug Information
  </div>
  <div class="collapse-content">
    <div class="stats stats-vertical lg:stats-horizontal shadow">
      <div class="stat">
        <div class="stat-title">Game State</div>
        <div class="stat-value text-lg">{gameState ? 'Connected' : 'Disconnected'}</div>
      </div>
      <div class="stat">
        <div class="stat-title">Raw Entities</div>
        <div class="stat-value text-sm">M:{monsters.length} I:{interactables.length} It:{items.length}</div>
      </div>
      <div class="stat">
        <div class="stat-title">Filtered</div>
        <div class="stat-value text-sm">M:{filteredMonsters.length} I:{filteredInteractables.length} It:{filteredItems.length}</div>
      </div>
      <div class="stat">
        <div class="stat-title">Max Distance</div>
        <div class="stat-value text-lg">{maxDistance}m</div>
      </div>
    </div>
    <div class="mt-4 text-sm opacity-70">
      <p><strong>Color Scheme:</strong> Improved (White items now light gray for readability)</p>
      <p><strong>Overlays Default:</strong> Disabled (better performance)</p>
    </div>
  </div>
</div>