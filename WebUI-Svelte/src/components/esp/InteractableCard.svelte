<script>
  import { MapPin, Eye, EyeOff, Package, Target } from 'lucide-svelte';

  export let interactable;
  export let showDistances = true;
  export let onTeleport = () => {};

  function formatDistance(distance) {
    return distance < 1000 ? `${distance.toFixed(1)}m` : `${(distance / 1000).toFixed(2)}km`;
  }

  function formatPosition(x, y, z) {
    return `(${x.toFixed(1)}, ${y.toFixed(1)}, ${z.toFixed(1)})`;
  }
</script>

<div class="card bg-base-100 shadow-md hover:shadow-lg transition-all {interactable.IsAvailable ? 'border-2 border-success' : interactable.IsActivated ? 'border-2 border-neutral opacity-70' : ''}">
  <div class="card-body p-4">
    <div class="flex justify-between items-start mb-3">
      <div class="flex-1">
        <h5 class="font-semibold text-sm">{interactable.DisplayName}</h5>
        <div class="badge badge-info badge-sm mt-1">{interactable.Category}</div>
      </div>
      {#if showDistances}
        <div class="badge badge-neutral badge-sm gap-1">
          <MapPin size={10} />
          {formatDistance(interactable.Distance)}
        </div>
      {/if}
    </div>

    <div class="mb-3">
      <div class="badge badge-sm gap-1 {interactable.IsActivated ? 'badge-neutral' : interactable.IsAvailable ? 'badge-success' : 'badge-error'}">
        {#if interactable.IsActivated}
          <EyeOff size={10} />
          Activated
        {:else if interactable.IsAvailable}
          <Eye size={10} />
          Available
        {:else}
          <Package size={10} />
          Unavailable
        {/if}
      </div>
    </div>

    <div class="text-xs opacity-60 mb-3 font-mono">
      {formatPosition(interactable.PositionX, interactable.PositionY, interactable.PositionZ)}
    </div>

    <button class="btn btn-outline btn-sm w-full" on:click={() => onTeleport(interactable.PositionX, interactable.PositionY, interactable.PositionZ, interactable.DisplayName)}>
      <Target size={12} />
      Teleport
    </button>
  </div>
</div>