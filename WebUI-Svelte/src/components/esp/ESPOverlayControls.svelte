<script>
  import { Eye, Package, Target } from 'lucide-svelte';
  import { api } from '../../lib/api.js';

  export let showMonsterOverlays = false;
  export let showInteractableOverlays = false;
  export let showItemOverlays = false;
  export let overlayDistance = 50;
  export let overlayScale = 1.0;
  export let showOverlayDistances = true;
  export let showOverlayHealth = true;

  // Toggle ESP overlay labels
  async function toggleOverlay(overlayType) {
    try {
      let currentState, newState;
      
      switch (overlayType) {
        case 'monsters':
          currentState = showMonsterOverlays;
          showMonsterOverlays = !showMonsterOverlays;
          newState = showMonsterOverlays;
          break;
        case 'interactables':
          currentState = showInteractableOverlays;
          showInteractableOverlays = !showInteractableOverlays;
          newState = showInteractableOverlays;
          break;
        case 'items':
          currentState = showItemOverlays;
          showItemOverlays = !showItemOverlays;
          newState = showItemOverlays;
          break;
      }
      
      await api.toggleESPOverlay(overlayType, newState);
      console.log(`ESP ${overlayType} overlay ${newState ? 'enabled' : 'disabled'}`);
    } catch (error) {
      console.error('Failed to toggle ESP overlay:', error);
      // Revert state on error
      switch (overlayType) {
        case 'monsters':
          showMonsterOverlays = !showMonsterOverlays;
          break;
        case 'interactables':
          showInteractableOverlays = !showInteractableOverlays;
          break;
        case 'items':
          showItemOverlays = !showItemOverlays;
          break;
      }
    }
  }

  // Update overlay configuration
  async function updateOverlayConfig() {
    try {
      await api.configureESPOverlay({
        maxDistance: overlayDistance,
        labelScale: overlayScale,
        showDistances: showOverlayDistances,
        showHealth: showOverlayHealth
      });
      console.log('ESP overlay configuration updated');
    } catch (error) {
      console.error('Failed to update ESP overlay config:', error);
    }
  }
</script>

<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🏷️ In-Game Labels</h3>
    
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Label Toggles</span>
        </label>
        <div class="space-y-2">
          <button class="btn btn-sm w-full {showMonsterOverlays ? 'btn-accent' : 'btn-outline'}" on:click={() => toggleOverlay('monsters')}>
            <Eye size={14} />
            Monster Labels
          </button>
          <button class="btn btn-sm w-full {showInteractableOverlays ? 'btn-accent' : 'btn-outline'}" on:click={() => toggleOverlay('interactables')}>
            <Package size={14} />
            Interactable Labels
          </button>
          <button class="btn btn-sm w-full {showItemOverlays ? 'btn-accent' : 'btn-outline'}" on:click={() => toggleOverlay('items')}>
            <Target size={14} />
            Item Labels
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Label Distance</span>
          <span class="label-text-alt">{overlayDistance}m max</span>
        </label>
        <input type="range" bind:value={overlayDistance} min="10" max="200" step="10" on:change={updateOverlayConfig} class="range range-info" />
        <div class="w-full flex justify-between text-xs px-2 opacity-60">
          <span>10m</span>
          <span>100m</span>
          <span>200m</span>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Label Scale</span>
          <span class="label-text-alt">{overlayScale}x size</span>
        </label>
        <input type="range" bind:value={overlayScale} min="0.5" max="2.0" step="0.1" on:change={updateOverlayConfig} class="range range-warning" />
        <div class="w-full flex justify-between text-xs px-2 opacity-60">
          <span>0.5x</span>
          <span>1.0x</span>
          <span>2.0x</span>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Label Options</span>
        </label>
        <div class="space-y-2">
          <label class="label cursor-pointer">
            <span class="label-text">Show Distances</span>
            <input type="checkbox" bind:checked={showOverlayDistances} on:change={updateOverlayConfig} class="checkbox checkbox-primary" />
          </label>
          <label class="label cursor-pointer">
            <span class="label-text">Show Health</span>
            <input type="checkbox" bind:checked={showOverlayHealth} on:change={updateOverlayConfig} class="checkbox checkbox-primary" />
          </label>
        </div>
      </div>
    </div>
  </div>
</div>