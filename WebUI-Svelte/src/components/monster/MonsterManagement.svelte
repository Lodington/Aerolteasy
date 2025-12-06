<script>
  import { Package, Zap, Users } from "lucide-svelte";

  export let selectedItem = "";
  export let itemCount = 1;
  export let selectedBuff = "";
  export let buffStacks = 1;
  export let buffDuration = -1;
  export let isGivingItem = false;
  export let isGivingBuff = false;
  export let managementStatusMessage = "";
  export let availableItems = [];
  export let availableBuffs = [];
  export let onGiveItemToMonsters = () => {};
  export let onGiveBuffToMonsters = () => {};
</script>

<!-- Monster Management Section -->
<div class="divider">
  <div class="flex items-center gap-2">
    <Users size={20} />
    <span class="text-lg font-semibold">Monster Management</span>
  </div>
</div>

<div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
  <!-- Give Items to Monsters -->
  <div class="card bg-base-200 shadow-lg">
    <div class="card-body">
      <h3 class="card-title text-lg mb-4">
        <Package size={20} />
        Give Items to All Monsters
      </h3>
      <p class="text-sm text-base-content/70 mb-4">
        Give items to all existing monsters on the map
      </p>

      <div class="space-y-4">
        <div class="form-control">
          <label class="label" for="monster-item-select">
            <span class="label-text">Select Item</span>
          </label>
          <select id="monster-item-select" bind:value={selectedItem} class="select select-bordered">
            <option value="">Choose an item...</option>
            {#each availableItems as category}
              <optgroup label={category.category}>
                {#each category.items as item}
                  <option value={item.value}>{item.label}</option>
                {/each}
              </optgroup>
            {/each}
          </select>
        </div>

        <div class="form-control">
          <label class="label" for="monster-item-count">
            <span class="label-text">Count</span>
          </label>
          <input
            id="monster-item-count"
            type="number"
            bind:value={itemCount}
            min="1"
            max="100"
            class="input input-bordered"
          />
        </div>

        <button
          class="btn btn-primary w-full"
          on:click={onGiveItemToMonsters}
          disabled={!selectedItem || isGivingItem}
        >
          <Package size={16} />
          {isGivingItem ? "Giving Items..." : `Give ${itemCount}x Item to All Monsters`}
        </button>
      </div>
    </div>
  </div>

  <!-- Give Buffs to Monsters -->
  <div class="card bg-base-200 shadow-lg">
    <div class="card-body">
      <h3 class="card-title text-lg mb-4">
        <Zap size={20} />
        Give Buffs to All Monsters
      </h3>
      <p class="text-sm text-base-content/70 mb-4">
        Apply buffs to all existing monsters on the map
      </p>

      <div class="space-y-4">
        <div class="form-control">
          <label class="label" for="monster-buff-select">
            <span class="label-text">Select Buff</span>
          </label>
          <select id="monster-buff-select" bind:value={selectedBuff} class="select select-bordered">
            <option value="">Choose a buff...</option>
            {#each availableBuffs as buff}
              <option value={buff.value} title={buff.description}>
                {buff.label}
              </option>
            {/each}
          </select>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="form-control">
            <label class="label" for="monster-buff-stacks">
              <span class="label-text">Stacks</span>
            </label>
            <input
              id="monster-buff-stacks"
              type="number"
              bind:value={buffStacks}
              min="1"
              max="100"
              class="input input-bordered"
            />
          </div>

          <div class="form-control">
            <label class="label" for="monster-buff-duration">
              <span class="label-text">Duration (s)</span>
              <span class="label-text-alt">-1 = permanent</span>
            </label>
            <input
              id="monster-buff-duration"
              type="number"
              bind:value={buffDuration}
              min="-1"
              max="3600"
              class="input input-bordered"
            />
          </div>
        </div>

        <button
          class="btn btn-secondary w-full"
          on:click={onGiveBuffToMonsters}
          disabled={!selectedBuff || isGivingBuff}
        >
          <Zap size={16} />
          {isGivingBuff ? "Giving Buffs..." : `Give ${buffStacks}x Buff to All Monsters`}
        </button>
      </div>
    </div>
  </div>
</div>

<!-- Management Status Message -->
{#if managementStatusMessage}
  <div
    class="alert"
    class:alert-success={!managementStatusMessage.includes("Failed")}
    class:alert-error={managementStatusMessage.includes("Failed")}
  >
    <div>
      {managementStatusMessage}
    </div>
  </div>
{/if}