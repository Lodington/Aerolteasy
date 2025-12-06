<script>
  import { X } from "lucide-svelte";

  export let monsterSearch = "";
  export let selectedMonster = null;
  export let monsters = [];

  // Search results
  $: monsterResults = monsterSearch.trim()
    ? monsters
        .flatMap((category) =>
          category.monsters
            .filter((monster) => {
              const searchLower = monsterSearch.toLowerCase();
              return (
                monster.label.toLowerCase().includes(searchLower) ||
                monster.value.toLowerCase().includes(searchLower)
              );
            })
            .map((monster) => ({ ...monster, category: category.category })),
        )
        .slice(0, 8)
    : [];

  function selectMonster(monster) {
    selectedMonster = monster;
    monsterSearch = "";
  }

  function clearMonster() {
    selectedMonster = null;
  }
</script>

<div class="card bg-base-200 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🎯 Select Monster</h3>

    {#if selectedMonster}
      <!-- Selected Monster Display -->
      <div class="alert alert-success">
        <div class="flex items-center gap-4">
          <span class="text-3xl">{selectedMonster.emoji}</span>
          <div class="flex-1">
            <h4 class="font-semibold">{selectedMonster.label}</h4>
            <div class="badge badge-info badge-sm">
              {selectedMonster.category}
            </div>
            <p class="text-sm mt-1">{selectedMonster.description}</p>
          </div>
          <button class="btn btn-ghost btn-sm" on:click={clearMonster}>
            <X size={16} />
            Clear
          </button>
        </div>
      </div>
    {:else}
      <!-- Monster Search -->
      <div class="space-y-4">
        <input
          type="search"
          bind:value={monsterSearch}
          placeholder="🔍 Search for monsters to spawn..."
          class="input input-bordered w-full"
        />

        {#if monsterSearch.trim() && monsterResults.length > 0}
          <div class="grid grid-cols-1 gap-3">
            {#each monsterResults as monster}
              <button
                class="card bg-base-200 hover:bg-base-300 transition-colors cursor-pointer"
                on:click={() => selectMonster(monster)}
              >
                <div class="card-body p-4">
                  <div class="flex items-center gap-3">
                    <span class="text-2xl">{monster.emoji}</span>
                    <div class="flex-1 text-left">
                      <h4 class="font-semibold">{monster.label}</h4>
                      <div class="badge badge-info badge-sm">
                        {monster.category}
                      </div>
                    </div>
                  </div>
                </div>
              </button>
            {/each}
          </div>
        {:else if monsterSearch.trim()}
          <div class="alert alert-warning">
            <div>
              <h4 class="font-bold">No Results</h4>
              <p>No monsters found matching "{monsterSearch}"</p>
            </div>
          </div>
        {/if}
      </div>
    {/if}
  </div>
</div>