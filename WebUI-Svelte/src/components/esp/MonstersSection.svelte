<script>
  import { Search } from 'lucide-svelte';
  import MonsterCard from './MonsterCard.svelte';

  export let showMonsters = true;
  export let filteredMonsters = [];
  export let showDistances = true;
  export let showHealth = true;
  export let monsterFilter = '';
  export let onTeleport = () => {};
</script>

{#if showMonsters}
  <div class="card bg-base-200 shadow-lg">
    <div class="card-body">
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <h4 class="card-title text-lg">🐉 Monsters</h4>
        <div class="form-control">
          <div class="input-group">
            <span class="bg-base-200"><Search size={16} /></span>
            <input 
              type="text" 
              bind:value={monsterFilter}
              placeholder="Filter monsters..."
              class="input input-bordered"
            />
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {#each filteredMonsters as monster}
          <MonsterCard 
            {monster} 
            {showDistances} 
            {showHealth} 
            {onTeleport}
          />
        {/each}
      </div>

      {#if filteredMonsters.length === 0}
        <div class="alert alert-info">
          <div>
            <h4 class="font-bold">No Monsters Found</h4>
            <p>No monsters found matching current filters</p>
          </div>
        </div>
      {/if}
    </div>
  </div>
{/if}