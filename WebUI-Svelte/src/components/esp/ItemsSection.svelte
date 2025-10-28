<script>
  import { Search, MapPin, Package, EyeOff, Target } from 'lucide-svelte';
  import Button from '../Button.svelte';

  export let showItems = true;
  export let filteredItems = [];
  export let itemTiers = [];
  export let selectedItemTier = 'all';
  export let itemFilter = '';
  export let showDistances = true;
  export let onTeleport = () => {};

  function formatDistance(distance) {
    return distance < 1000 ? `${distance.toFixed(1)}m` : `${(distance / 1000).toFixed(2)}km`;
  }

  function formatPosition(x, y, z) {
    return `(${x.toFixed(1)}, ${y.toFixed(1)}, ${z.toFixed(1)})`;
  }

  function getTierBadgeClass(tier) {
    const tierMap = {
      'Tier1': 'badge-neutral',
      'Tier2': 'badge-success', 
      'Tier3': 'badge-error',
      'Boss': 'badge-warning',
      'Lunar': 'badge-info',
      'VoidTier1': 'badge-secondary',
      'VoidTier2': 'badge-secondary',
      'VoidTier3': 'badge-secondary',
      'VoidBoss': 'badge-secondary'
    };
    return tierMap[tier] || 'badge-neutral';
  }
</script>

{#if showItems}
  <div class="card bg-base-100 shadow-lg">
    <div class="card-body">
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <h4 class="card-title text-lg">💎 Items</h4>
        <div class="flex gap-4">
          <select bind:value={selectedItemTier} class="select select-bordered select-sm">
            <option value="all">All Tiers</option>
            {#each itemTiers as tier}
              <option value={tier}>{tier}</option>
            {/each}
          </select>
          <div class="form-control">
            <div class="input-group">
              <span class="bg-base-200"><Search size={16} /></span>
              <input 
                type="text" 
                bind:value={itemFilter}
                placeholder="Filter items..."
                class="input input-bordered input-sm"
              />
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {#each filteredItems as item}
          <div class="card bg-base-200 shadow-md hover:shadow-lg transition-all {item.IsPickupable ? 'border-2 border-info' : ''}">
            <div class="card-body p-4">
              <div class="flex justify-between items-start mb-3">
                <div class="flex-1">
                  <h5 class="font-semibold text-sm">{item.DisplayName}</h5>
                  <div class="badge badge-sm mt-1 {getTierBadgeClass(item.ItemTier)}">{item.ItemTier}</div>
                </div>
                {#if showDistances}
                  <div class="badge badge-neutral badge-sm gap-1">
                    <MapPin size={10} />
                    {formatDistance(item.Distance)}
                  </div>
                {/if}
              </div>

              <div class="mb-3">
                <div class="badge badge-sm gap-1 {item.IsPickupable ? 'badge-info' : 'badge-neutral'}">
                  {#if item.IsPickupable}
                    <Package size={10} />
                    Available
                  {:else}
                    <EyeOff size={10} />
                    Unavailable
                  {/if}
                </div>
              </div>

              <div class="text-xs opacity-60 mb-3 font-mono">
                {formatPosition(item.PositionX, item.PositionY, item.PositionZ)}
              </div>

              <button class="btn btn-outline btn-sm w-full" on:click={() => onTeleport(item.PositionX, item.PositionY, item.PositionZ, item.DisplayName)}>
                <Target size={12} />
                Teleport
              </button>
            </div>
          </div>
        {/each}
      </div>

      {#if filteredItems.length === 0}
        <div class="alert alert-info">
          <div>
            <h4 class="font-bold">No Items Found</h4>
            <p>No items found matching current filters</p>
          </div>
        </div>
      {/if}
    </div>
  </div>
{/if}