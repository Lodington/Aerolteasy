<script>
  import { Crown, MapPin, Heart, Target } from 'lucide-svelte';

  export let monster;
  export let showDistances = true;
  export let showHealth = true;
  export let onTeleport = () => {};

  function getHealthPercentage(monster) {
    return monster.MaxHealth > 0 ? (monster.Health / monster.MaxHealth) * 100 : 0;
  }

  function formatDistance(distance) {
    return distance < 1000 ? `${distance.toFixed(1)}m` : `${(distance / 1000).toFixed(2)}km`;
  }

  function formatPosition(x, y, z) {
    return `(${x.toFixed(1)}, ${y.toFixed(1)}, ${z.toFixed(1)})`;
  }
</script>

<div class="card bg-base-200 shadow-md hover:shadow-lg transition-all {monster.IsElite ? 'border-2 border-warning' : ''}">
  <div class="card-body p-4">
    <div class="flex justify-between items-start mb-3">
      <div class="flex-1">
        <h5 class="font-semibold text-sm">{monster.DisplayName}</h5>
        {#if monster.IsElite}
          <div class="badge badge-warning badge-sm gap-1 mt-1">
            <Crown size={10} />
            {monster.EliteType || 'Elite'}
          </div>
        {/if}
      </div>
      {#if showDistances}
        <div class="badge badge-neutral badge-sm gap-1">
          <MapPin size={10} />
          {formatDistance(monster.Distance)}
        </div>
      {/if}
    </div>

    {#if showHealth}
      <div class="mb-3">
        <div class="flex items-center gap-2 mb-2 text-sm">
          <Heart size={12} class="text-error" />
          <span>{Math.round(monster.Health)}/{Math.round(monster.MaxHealth)}</span>
          <span class="opacity-60">({getHealthPercentage(monster).toFixed(1)}%)</span>
        </div>
        <progress 
          class="progress progress-error w-full h-2" 
          value={getHealthPercentage(monster)} 
          max="100"
        ></progress>
      </div>
    {/if}

    <div class="text-xs opacity-60 mb-3 font-mono">
      {formatPosition(monster.PositionX, monster.PositionY, monster.PositionZ)}
    </div>

    <button class="btn btn-outline btn-sm w-full" on:click={() => onTeleport(monster.PositionX, monster.PositionY, monster.PositionZ, monster.DisplayName)}>
      <Target size={12} />
      Teleport
    </button>
  </div>
</div>