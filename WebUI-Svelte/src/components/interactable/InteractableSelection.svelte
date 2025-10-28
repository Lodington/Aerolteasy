<script>
  import { X, Filter, Grid, List } from "lucide-svelte";

  export let interactableSearch = "";
  export let selectedInteractable = null;
  export let interactables = [];

  let selectedCategory = "all";
  let viewMode = "grid"; // "grid" or "list"

  // Get all categories for filter dropdown
  $: categories = interactables.map(cat => cat.category);

  // Filter by category first, then by search
  $: filteredByCategory = selectedCategory === "all" 
    ? interactables.flatMap(category => 
        category.interactables.map(interactable => ({
          ...interactable,
          category: category.category,
        }))
      )
    : interactables
        .find(cat => cat.category === selectedCategory)
        ?.interactables.map(interactable => ({
          ...interactable,
          category: selectedCategory,
        })) || [];

  // Then apply search filter
  $: interactableResults = interactableSearch.trim()
    ? filteredByCategory.filter((interactable) => {
        const searchLower = interactableSearch.toLowerCase();
        return (
          interactable.label.toLowerCase().includes(searchLower) ||
          interactable.value.toLowerCase().includes(searchLower) ||
          interactable.description.toLowerCase().includes(searchLower)
        );
      }).slice(0, 20)
    : filteredByCategory.slice(0, 20);

  // Popular/Quick access items
  const quickAccess = [
    { value: 'Chest1', label: 'Small Chest', emoji: '📦' },
    { value: 'Chest2', label: 'Large Chest', emoji: '📋' },
    { value: 'GoldChest', label: 'Legendary Chest', emoji: '🏆' },
    { value: 'MultiShop', label: 'Multishop', emoji: '🛒' },
    { value: 'ShrineBoss', label: 'Mountain Shrine', emoji: '⛰️' },
    { value: 'ShrineChance', label: 'Chance Shrine', emoji: '🎰' }
  ];

  function selectInteractable(interactable) {
    selectedInteractable = interactable;
    interactableSearch = "";
  }

  function clearInteractable() {
    selectedInteractable = null;
  }

  function selectQuickItem(quickItem) {
    // Find the full interactable data
    const fullInteractable = interactables
      .flatMap(cat => cat.interactables.map(item => ({ ...item, category: cat.category })))
      .find(item => item.value === quickItem.value);
    
    if (fullInteractable) {
      selectInteractable(fullInteractable);
    }
  }

  function getCostColor(cost) {
    if (cost.includes('Free')) return 'badge-success';
    if (cost.includes('Lunar')) return 'badge-secondary';
    if (cost.includes('Items')) return 'badge-warning';
    return 'badge-info';
  }
</script>

<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🎯 Select Interactable</h3>

    {#if selectedInteractable}
      <!-- Selected Interactable Display -->
      <div class="card bg-gradient-to-r from-primary/20 to-secondary/20 border-2 border-primary/30 shadow-lg">
        <div class="card-body p-4">
          <div class="flex items-center gap-4">
            <div class="avatar">
              <div class="w-16 h-16 rounded-full bg-base-100 flex items-center justify-center shadow-lg">
                <div class="text-3xl leading-none flex items-center justify-center w-full h-full">
                  {selectedInteractable.emoji}
                </div>
              </div>
            </div>
            <div class="flex-1">
              <h4 class="font-bold text-xl text-base-content">{selectedInteractable.label}</h4>
              <div class="flex gap-2 mt-2">
                <div class="badge badge-primary badge-sm text-primary-content font-medium">
                  {selectedInteractable.category}
                </div>
                <div class="badge badge-accent badge-sm text-accent-content font-medium">
                  {selectedInteractable.cost}
                </div>
              </div>
              <p class="text-sm mt-2 text-base-content/80 font-medium">{selectedInteractable.description}</p>
            </div>
            <button class="btn btn-circle btn-outline btn-sm hover:btn-error" on:click={clearInteractable}>
              <X size={16} />
            </button>
          </div>
        </div>
      </div>
    {:else}
      <!-- Quick Access Section -->
      <div class="mb-6">
        <h4 class="font-semibold mb-3 flex items-center gap-2">
          ⚡ Quick Access
        </h4>
        <div class="grid grid-cols-2 sm:grid-cols-3 gap-2">
          {#each quickAccess as item}
            <button
              class="btn btn-outline btn-xs sm:btn-sm justify-start text-xs sm:text-sm"
              on:click={() => selectQuickItem(item)}
            >
              <span class="text-sm sm:text-lg">{item.emoji}</span>
              <span class="truncate">{item.label}</span>
            </button>
          {/each}
        </div>
      </div>

      <!-- Search and Filter Section -->
      <div class="space-y-4">
        <!-- Search Bar -->
        <div class="form-control">
          <div class="input-group">
            <span class="bg-base-200 px-4">🔍</span>
            <input
              type="search"
              bind:value={interactableSearch}
              placeholder="Search interactables..."
              class="input input-bordered flex-1"
            />
            {#if interactableSearch}
              <button 
                class="btn btn-square btn-outline" 
                on:click={() => interactableSearch = ''}
              >
                ×
              </button>
            {/if}
          </div>
        </div>

        <!-- Filters and View Controls -->
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4 sm:items-center">
          <div class="flex gap-3 items-center flex-1">
            <div class="form-control flex-1 sm:flex-initial">
              <select bind:value={selectedCategory} class="select select-bordered select-sm w-full">
                <option value="all">All Categories</option>
                {#each categories as category}
                  <option value={category}>{category}</option>
                {/each}
              </select>
            </div>
            
            <div class="btn-group">
              <button 
                class="btn btn-sm" 
                class:btn-active={viewMode === 'grid'}
                on:click={() => viewMode = 'grid'}
                title="Grid View"
              >
                <Grid size={14} />
              </button>
              <button 
                class="btn btn-sm" 
                class:btn-active={viewMode === 'list'}
                on:click={() => viewMode = 'list'}
                title="List View"
              >
                <List size={14} />
              </button>
            </div>
          </div>

          <div class="text-sm opacity-70 text-center sm:text-left">
            {interactableResults.length} items
          </div>
        </div>

        <!-- Results Section -->
        {#if interactableResults.length > 0}
          <div class="space-y-3">
            {#if viewMode === 'grid'}
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                {#each interactableResults as interactable}
                  <button
                    class="card bg-base-200 hover:bg-base-300 transition-all duration-200 cursor-pointer hover:scale-[1.02] active:scale-95"
                    on:click={() => selectInteractable(interactable)}
                  >
                    <div class="card-body p-3 sm:p-4">
                      <div class="flex items-start gap-2 sm:gap-3">
                        <span class="text-2xl sm:text-3xl flex-shrink-0">{interactable.emoji}</span>
                        <div class="flex-1 text-left min-w-0">
                          <h4 class="font-semibold text-sm truncate">{interactable.label}</h4>
                          <p class="text-xs opacity-70 mt-1 line-clamp-2 hidden sm:block">{interactable.description}</p>
                          <div class="flex flex-wrap gap-1 mt-2">
                            <div class="badge badge-info badge-xs">
                              {interactable.category}
                            </div>
                            <div class="badge {getCostColor(interactable.cost)} badge-xs">
                              {interactable.cost}
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </button>
                {/each}
              </div>
            {:else}
              <div class="space-y-2">
                {#each interactableResults as interactable}
                  <button
                    class="w-full p-3 bg-base-200 hover:bg-base-300 rounded-lg transition-colors cursor-pointer text-left active:scale-[0.98]"
                    on:click={() => selectInteractable(interactable)}
                  >
                    <div class="flex items-center gap-3">
                      <span class="text-xl sm:text-2xl flex-shrink-0">{interactable.emoji}</span>
                      <div class="flex-1 min-w-0">
                        <div class="flex flex-col sm:flex-row sm:items-center gap-1 sm:gap-2">
                          <h4 class="font-semibold truncate">{interactable.label}</h4>
                          <div class="flex gap-1">
                            <div class="badge badge-info badge-xs">
                              {interactable.category}
                            </div>
                            <div class="badge {getCostColor(interactable.cost)} badge-xs">
                              {interactable.cost}
                            </div>
                          </div>
                        </div>
                        <p class="text-sm opacity-70 mt-1 line-clamp-2">{interactable.description}</p>
                      </div>
                    </div>
                  </button>
                {/each}
              </div>
            {/if}
          </div>
        {:else if interactableSearch.trim() || selectedCategory !== 'all'}
          <div class="alert alert-warning">
            <div>
              <h4 class="font-bold">No Results</h4>
              <p>No interactables found matching your filters</p>
            </div>
          </div>
        {:else}
          <div class="text-center py-8 opacity-70">
            <p>Use the search bar or category filter to find interactables</p>
          </div>
        {/if}
      </div>
    {/if}
  </div>
</div>