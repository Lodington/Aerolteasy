<script>
  import { Package, MapPin, Zap, RotateCcw } from "lucide-svelte";

  export let selectedInteractable = null;
  export let spawnDistance = 5;
  export let isSpawning = false;
  export let statusMessage = "";
  export let onSpawnInteractable = () => {};

  let spawnCount = 1;
  let spawnPattern = "single"; // "single", "line", "circle"

  // Quick distance presets
  const distancePresets = [
    { label: "Very Close", value: 3, icon: "🔥" },
    { label: "Close", value: 5, icon: "📍" },
    { label: "Medium", value: 10, icon: "🎯" },
    { label: "Far", value: 20, icon: "🌄" }
  ];

  function setDistance(distance) {
    spawnDistance = distance;
  }

  function resetSettings() {
    spawnDistance = 5;
    spawnCount = 1;
    spawnPattern = "single";
  }

  function getSpawnButtonText() {
    if (isSpawning) return "Spawning...";
    if (spawnCount === 1) return `Spawn ${selectedInteractable?.label || 'Interactable'}`;
    return `Spawn ${spawnCount}x ${selectedInteractable?.label || 'Interactables'}`;
  }
</script>

{#if selectedInteractable}
  <div class="card bg-base-100 shadow-lg">
    <div class="card-body">
      <div class="flex items-center justify-between mb-4">
        <h3 class="card-title text-lg">⚙️ Spawn Settings</h3>
        <button class="btn btn-ghost btn-sm" on:click={resetSettings} title="Reset to defaults">
          <RotateCcw size={14} />
          Reset
        </button>
      </div>

      <!-- Distance Settings -->
      <div class="space-y-4">
        <div class="form-control">
          <label class="label" for="distance-range">
            <span class="label-text flex items-center gap-2">
              <MapPin size={16} />
              Distance from Player
            </span>
            <span class="label-text-alt">{spawnDistance} units</span>
          </label>
          
          <!-- Distance Presets -->
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-2 mb-3">
            {#each distancePresets as preset}
              <button
                class="btn btn-outline btn-xs sm:btn-sm text-xs sm:text-sm"
                class:btn-active={spawnDistance === preset.value}
                on:click={() => setDistance(preset.value)}
              >
                <span class="text-sm">{preset.icon}</span>
                <span class="hidden sm:inline">{preset.label}</span>
                <span class="sm:hidden">{preset.label.split(' ')[0]}</span>
              </button>
            {/each}
          </div>
          
          <!-- Custom Distance Slider -->
          <input
            id="distance-range"
            type="range"
            bind:value={spawnDistance}
            min="1"
            max="50"
            class="range range-primary range-sm"
          />
          <div class="w-full flex justify-between text-xs px-2 opacity-60">
            <span>1</span>
            <span>25</span>
            <span>50</span>
          </div>
        </div>

        <!-- Advanced Options -->
        <div class="collapse collapse-arrow bg-base-200">
          <input type="checkbox" />
          <div class="collapse-title text-sm font-medium py-3">
            <Zap size={16} class="inline mr-2" />
            Advanced Options
          </div>
          <div class="collapse-content space-y-4 pb-4">
            <!-- Mobile-optimized grid for advanced options -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <!-- Spawn Count -->
              <div class="form-control">
                <label class="label" for="spawn-count">
                  <span class="label-text">Spawn Count</span>
                  <span class="label-text-alt">Max: 10</span>
                </label>
                <input
                  id="spawn-count"
                  type="number"
                  bind:value={spawnCount}
                  min="1"
                  max="10"
                  class="input input-bordered input-sm"
                />
              </div>

              <!-- Spawn Pattern -->
              <div class="form-control">
                <label class="label" for="spawn-pattern">
                  <span class="label-text">Spawn Pattern</span>
                </label>
                <select id="spawn-pattern" bind:value={spawnPattern} class="select select-bordered select-sm">
                  <option value="single">Single Location</option>
                  <option value="line">Line Formation</option>
                  <option value="circle">Circle Formation</option>
                </select>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Spawn Button -->
      <button
        class="btn btn-primary w-full mt-6"
        on:click={onSpawnInteractable}
        disabled={isSpawning || !selectedInteractable}
      >
        <Package size={16} />
        {getSpawnButtonText()}
      </button>

      <!-- Status Message -->
      {#if statusMessage}
        <div
          class="alert mt-4"
          class:alert-success={!statusMessage.includes("Failed")}
          class:alert-error={statusMessage.includes("Failed")}
        >
          <div>
            {statusMessage}
          </div>
        </div>
      {/if}

      <!-- Interactable Info -->
      <div class="mt-4 p-3 bg-base-200 rounded-lg">
        <h4 class="font-semibold text-sm mb-2">📋 Spawn Preview</h4>
        <div class="text-sm space-y-1 opacity-80">
          <div class="flex justify-between">
            <span>Item:</span>
            <span class="font-medium truncate ml-2">{selectedInteractable.label}</span>
          </div>
          <div class="flex justify-between">
            <span>Distance:</span>
            <span class="font-medium">{spawnDistance} units</span>
          </div>
          <div class="flex justify-between">
            <span>Count:</span>
            <span class="font-medium">{spawnCount}</span>
          </div>
          <div class="flex justify-between">
            <span>Pattern:</span>
            <span class="font-medium capitalize">{spawnPattern}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
{/if}