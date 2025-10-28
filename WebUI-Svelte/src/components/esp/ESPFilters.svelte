<script>
  import { Zap, Settings, Target, Eye, EyeOff } from 'lucide-svelte';
  import { api } from '../../lib/api.js';

  export let maxDistance = 100;
  export let sortBy = 'distance';
  export let sortOrder = 'asc';
  export let teleportTarget = 'selected';
  export let teleportOffset = 2.5;
  export let refreshGameState = () => {};

  function toggleSort(newSortBy) {
    if (sortBy === newSortBy) {
      sortOrder = sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      sortBy = newSortBy;
      sortOrder = 'asc';
    }
  }
</script>

<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🔍 Filters & Sorting</h3>
    
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Max Distance</span>
          <span class="label-text-alt">{maxDistance}m</span>
        </label>
        <input type="range" bind:value={maxDistance} min="10" max="500" step="10" class="range range-primary" />
        <div class="w-full flex justify-between text-xs px-2 opacity-60">
          <span>10m</span>
          <span>250m</span>
          <span>500m</span>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Sort By</span>
        </label>
        <div class="join">
          <button 
            class="btn join-item btn-sm {sortBy === 'distance' ? 'btn-primary' : 'btn-outline'}"
            on:click={() => toggleSort('distance')}
          >
            Distance {sortBy === 'distance' ? (sortOrder === 'asc' ? '↑' : '↓') : ''}
          </button>
          <button 
            class="btn join-item btn-sm {sortBy === 'name' ? 'btn-primary' : 'btn-outline'}"
            on:click={() => toggleSort('name')}
          >
            Name {sortBy === 'name' ? (sortOrder === 'asc' ? '↑' : '↓') : ''}
          </button>
          <button 
            class="btn join-item btn-sm {sortBy === 'health' ? 'btn-primary' : 'btn-outline'}"
            on:click={() => toggleSort('health')}
          >
            Health {sortBy === 'health' ? (sortOrder === 'asc' ? '↑' : '↓') : ''}
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Quick Actions</span>
        </label>
        <div class="flex flex-wrap gap-2">
          <button class="btn btn-outline btn-sm" on:click={refreshGameState}>
            <Zap size={14} />
            Refresh
          </button>
          <button class="btn btn-outline btn-sm" on:click={() => api.debugESPData()}>
            <Settings size={14} />
            Debug
          </button>
          <button class="btn btn-outline btn-sm" on:click={() => api.testESPData()}>
            <Target size={14} />
            Test Data
          </button>
        </div>
      </div>
      
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Teleport Target</span>
        </label>
        <div class="flex gap-4">
          <label class="label cursor-pointer">
            <span class="label-text">Selected Player</span>
            <input type="radio" bind:group={teleportTarget} value="selected" class="radio radio-primary" />
          </label>
          <label class="label cursor-pointer">
            <span class="label-text">All Players</span>
            <input type="radio" bind:group={teleportTarget} value="all" class="radio radio-primary" />
          </label>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Height Offset</span>
          <span class="label-text-alt">{teleportOffset}m above target</span>
        </label>
        <input type="range" bind:value={teleportOffset} min="0" max="10" step="0.5" class="range range-secondary" />
        <div class="w-full flex justify-between text-xs px-2 opacity-60">
          <span>0m</span>
          <span>5m</span>
          <span>10m</span>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">ESP Actions</span>
        </label>
        <div class="flex flex-wrap gap-2">
          <button class="btn btn-accent btn-sm" on:click={() => api.testESPOverlay()}>
            <Eye size={14} />
            Test Overlays
          </button>
          <button class="btn btn-error btn-sm" on:click={() => api.disableESPOverlay()}>
            <EyeOff size={14} />
            Disable All
          </button>
        </div>
      </div>
    </div>
  </div>
</div>