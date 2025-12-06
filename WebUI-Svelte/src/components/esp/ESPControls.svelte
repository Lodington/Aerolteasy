<script>
  export let showMonsters = true;
  export let showInteractables = true;
  export let showItems = true;
  export let showDistances = true;
  export let showHealth = true;
  export let showElites = true;
  export let filteredMonsters = [];
  export let filteredInteractables = [];
  export let filteredItems = [];
</script>

<div class="card bg-base-200 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">👁️ ESP Controls</h3>
    
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div class="form-control">
        <label class="label cursor-pointer">
          <span class="label-text">Show Monsters</span>
          <div class="flex items-center gap-2">
            <span class="badge badge-neutral badge-sm">({filteredMonsters.length})</span>
            <input type="checkbox" bind:checked={showMonsters} class="checkbox checkbox-primary" />
          </div>
        </label>
      </div>

      <div class="form-control">
        <label class="label cursor-pointer">
          <span class="label-text">Show Interactables</span>
          <div class="flex items-center gap-2">
            <span class="badge badge-neutral badge-sm">({filteredInteractables.length})</span>
            <input type="checkbox" bind:checked={showInteractables} class="checkbox checkbox-primary" />
          </div>
        </label>
      </div>

      <div class="form-control">
        <label class="label cursor-pointer">
          <span class="label-text">Show Items</span>
          <div class="flex items-center gap-2">
            <span class="badge badge-neutral badge-sm">({filteredItems.length})</span>
            <input type="checkbox" bind:checked={showItems} class="checkbox checkbox-primary" />
          </div>
        </label>
      </div>

      <div class="form-control">
        <label class="label cursor-pointer">
          <span class="label-text">Show Distances</span>
          <input type="checkbox" bind:checked={showDistances} class="checkbox checkbox-primary" />
        </label>
      </div>

      <div class="form-control">
        <label class="label cursor-pointer">
          <span class="label-text">Show Health</span>
          <input type="checkbox" bind:checked={showHealth} class="checkbox checkbox-primary" />
        </label>
      </div>

      <div class="form-control">
        <label class="label cursor-pointer">
          <span class="label-text">Include Elites</span>
          <input type="checkbox" bind:checked={showElites} class="checkbox checkbox-primary" />
        </label>
      </div>
    </div>
  </div>
</div>