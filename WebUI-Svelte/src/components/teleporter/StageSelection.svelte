<script>
  import { X, Filter, Grid, List } from "lucide-svelte";

  export let stageSearch = "";
  export let selectedStage = null;
  export let stages = [];

  let selectedCategory = "all";
  let viewMode = "grid";

  // Get all categories for filter dropdown
  $: categories = stages.map(cat => cat.category);

  // Filter by category first, then by search
  $: filteredByCategory = selectedCategory === "all" 
    ? stages.flatMap(category => 
        category.stages.map(stage => ({
          ...stage,
          category: category.category,
        }))
      )
    : stages
        .find(cat => cat.category === selectedCategory)
        ?.stages.map(stage => ({
          ...stage,
          category: selectedCategory,
        })) || [];

  // Then apply search filter
  $: stageResults = stageSearch.trim()
    ? filteredByCategory.filter((stage) => {
        const searchLower = stageSearch.toLowerCase();
        return (
          stage.label.toLowerCase().includes(searchLower) ||
          stage.value.toLowerCase().includes(searchLower) ||
          stage.description.toLowerCase().includes(searchLower) ||
          stage.difficulty.toLowerCase().includes(searchLower)
        );
      }).slice(0, 20)
    : filteredByCategory.slice(0, 20);

  // Quick access stages
  const quickAccess = [
    { value: 'golemplains', label: 'Titanic Plains', emoji: '🌾' },
    { value: 'goolake', label: 'Abandoned Aqueduct', emoji: '🏛️' },
    { value: 'wispgraveyard', label: 'Scorched Acres', emoji: '🔥' },
    { value: 'skymeadow', label: 'Sky Meadow', emoji: '☁️' },
    { value: 'moon2', label: 'Commencement', emoji: '🌙' },
    { value: 'bazaar', label: 'Bazaar', emoji: '🛒' }
  ];

  function selectStage(stage) {
    selectedStage = stage;
    stageSearch = "";
  }

  function clearStage() {
    selectedStage = null;
  }

  function selectQuickStage(quickStage) {
    const fullStage = stages
      .flatMap(cat => cat.stages.map(stage => ({ ...stage, category: cat.category })))
      .find(stage => stage.value === quickStage.value);
    
    if (fullStage) {
      selectStage(fullStage);
    }
  }

  function getDifficultyColor(difficulty) {
    switch (difficulty.toLowerCase()) {
      case 'easy': return 'badge-success';
      case 'medium': return 'badge-warning';
      case 'hard': return 'badge-error';
      case 'very hard': return 'badge-error';
      case 'extreme': return 'badge-error';
      case 'ultimate': return 'badge-error';
      case 'special': return 'badge-secondary';
      case 'peaceful': return 'badge-info';
      case 'challenge': return 'badge-accent';
      default: return 'badge-neutral';
    }
  }
</script>

<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🗺️ Stage Selection</h3>

    {#if selectedStage}
      <!-- Selected Stage Display -->
      <div class="card bg-gradient-to-r from-primary/20 to-secondary/20 border-2 border-primary/30 shadow-lg">
        <div class="card-body p-4">
          <div class="flex items-center gap-4">
            <div class="avatar">
              <div class="w-16 h-16 rounded-full bg-base-100 flex items-center justify-center shadow-lg">
                <div class="text-3xl leading-none flex items-center justify-center w-full h-full">
                  {selectedStage.emoji}
                </div>
              </div>
            </div>
            <div class="flex-1">
              <h4 class="font-bold text-xl text-base-content">{selectedStage.label}</h4>
              <div class="flex gap-2 mt-2">
                <div class="badge badge-primary badge-sm text-primary-content font-medium">
                  {selectedStage.category}
                </div>
                <div class="badge {getDifficultyColor(selectedStage.difficulty)} badge-sm text-white font-medium">
                  {selectedStage.difficulty}
                </div>
              </div>
              <p class="text-sm mt-2 text-base-content/80 font-medium">{selectedStage.description}</p>
            </div>
            <button class="btn btn-circle btn-outline btn-sm hover:btn-error" on:click={clearStage}>
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
          {#each quickAccess as stage}
            <button
              class="btn btn-outline btn-xs sm:btn-sm justify-start text-xs sm:text-sm"
              on:click={() => selectQuickStage(stage)}
            >
              <span class="text-sm sm:text-lg">{stage.emoji}</span>
              <span class="truncate">{stage.label}</span>
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
              bind:value={stageSearch}
              placeholder="Search stages..."
              class="input input-bordered flex-1"
            />
            {#if stageSearch}
              <button 
                class="btn btn-square btn-outline" 
                on:click={() => stageSearch = ''}
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
            {stageResults.length} stages
          </div>
        </div>

        <!-- Results Section -->
        {#if stageResults.length > 0}
          <div class="space-y-3">
            {#if viewMode === 'grid'}
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                {#each stageResults as stage}
                  <button
                    class="card bg-base-200 hover:bg-base-300 transition-all duration-200 cursor-pointer hover:scale-[1.02] active:scale-95"
                    on:click={() => selectStage(stage)}
                  >
                    <div class="card-body p-3 sm:p-4">
                      <div class="flex items-start gap-2 sm:gap-3">
                        <span class="text-2xl sm:text-3xl flex-shrink-0">{stage.emoji}</span>
                        <div class="flex-1 text-left min-w-0">
                          <h4 class="font-semibold text-sm truncate">{stage.label}</h4>
                          <p class="text-xs opacity-70 mt-1 line-clamp-2 hidden sm:block">{stage.description}</p>
                          <div class="flex flex-wrap gap-1 mt-2">
                            <div class="badge badge-info badge-xs">
                              {stage.category}
                            </div>
                            <div class="badge {getDifficultyColor(stage.difficulty)} badge-xs text-white">
                              {stage.difficulty}
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
                {#each stageResults as stage}
                  <button
                    class="w-full p-3 bg-base-200 hover:bg-base-300 rounded-lg transition-colors cursor-pointer text-left active:scale-[0.98]"
                    on:click={() => selectStage(stage)}
                  >
                    <div class="flex items-center gap-3">
                      <span class="text-xl sm:text-2xl flex-shrink-0">{stage.emoji}</span>
                      <div class="flex-1 min-w-0">
                        <div class="flex flex-col sm:flex-row sm:items-center gap-1 sm:gap-2">
                          <h4 class="font-semibold truncate">{stage.label}</h4>
                          <div class="flex gap-1">
                            <div class="badge badge-info badge-xs">
                              {stage.category}
                            </div>
                            <div class="badge {getDifficultyColor(stage.difficulty)} badge-xs text-white">
                              {stage.difficulty}
                            </div>
                          </div>
                        </div>
                        <p class="text-sm opacity-70 mt-1 line-clamp-2">{stage.description}</p>
                      </div>
                    </div>
                  </button>
                {/each}
              </div>
            {/if}
          </div>
        {:else if stageSearch.trim() || selectedCategory !== 'all'}
          <div class="alert alert-warning">
            <div>
              <h4 class="font-bold">No Results</h4>
              <p>No stages found matching your filters</p>
            </div>
          </div>
        {:else}
          <div class="text-center py-8 opacity-70">
            <p>Use the search bar or category filter to find stages</p>
          </div>
        {/if}
      </div>
    {/if}
  </div>
</div>