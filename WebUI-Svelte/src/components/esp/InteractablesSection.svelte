<script>
  import { Search } from 'lucide-svelte';
  import InteractableCard from './InteractableCard.svelte';

  export let showInteractables = true;
  export let filteredInteractables = [];
  export let categories = [];
  export let selectedCategory = 'all';
  export let interactableFilter = '';
  export let showDistances = true;
  export let onTeleport = () => {};
</script>

{#if showInteractables}
  <div class="card bg-base-200 shadow-lg">
    <div class="card-body">
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <h4 class="card-title text-lg">📦 Interactables</h4>
        <div class="flex gap-4">
          <select bind:value={selectedCategory} class="select select-bordered">
            <option value="all">All Categories</option>
            {#each categories as category}
              <option value={category}>{category}</option>
            {/each}
          </select>
          <div class="form-control">
            <div class="input-group">
              <span class="bg-base-200"><Search size={16} /></span>
              <input 
                type="text" 
                bind:value={interactableFilter}
                placeholder="Filter interactables..."
                class="input input-bordered"
              />
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {#each filteredInteractables as interactable}
          <InteractableCard 
            {interactable} 
            {showDistances} 
            {onTeleport}
          />
        {/each}
      </div>

      {#if filteredInteractables.length === 0}
        <div class="alert alert-info">
          <div>
            <h4 class="font-bold">No Interactables Found</h4>
            <p>No interactables found matching current filters</p>
          </div>
        </div>
      {/if}
    </div>
  </div>
{/if}