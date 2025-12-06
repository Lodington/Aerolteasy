<script>
  import { Target, Package, Zap } from "lucide-svelte";

  export let selectedMonster = null;
  export let spawnCount = 1;
  export let spawnDistance = 10;
  export let selectedTeam = "Monster";
  export let availableTeams = [];
  export let isSpawning = false;
  export let statusMessage = "";
  export let onSpawnMonster = () => {};

  // Spawn configuration
  export let spawnItemFilter = "";
  export let spawnBuffFilter = "";
  export let spawnItems = [];
  export let spawnBuffs = [];
  export let availableItems = [];
  export let availableBuffs = [];

  // Reactive suggestions
  $: suggestedSpawnItems = spawnItemFilter.trim()
    ? availableItems
        .flatMap((category) =>
          category.items.filter((item) => {
            const hasItem = spawnItems.some(
              (spawnItem) => spawnItem.value === item.value,
            );
            const matchesSearch =
              item.label
                .toLowerCase()
                .includes(spawnItemFilter.toLowerCase()) ||
              item.value.toLowerCase().includes(spawnItemFilter.toLowerCase());
            return !hasItem && matchesSearch;
          }),
        )
        .slice(0, 6)
    : [];

  $: suggestedSpawnBuffs = spawnBuffFilter.trim()
    ? availableBuffs.filter((buff) => {
        const hasBuff = spawnBuffs.some(
          (spawnBuff) => spawnBuff.value === buff.value,
        );
        const matchesSearch =
          buff.label.toLowerCase().includes(spawnBuffFilter.toLowerCase()) ||
          buff.value.toLowerCase().includes(spawnBuffFilter.toLowerCase()) ||
          buff.description.toLowerCase().includes(spawnBuffFilter.toLowerCase());
        return !hasBuff && matchesSearch;
      }).slice(0, 6)
    : [];

  // Functions
  function addSpawnItem(itemValue, count = 1) {
    const existingItem = spawnItems.find((item) => item.value === itemValue);
    if (existingItem) {
      existingItem.count += count;
    } else {
      const itemData = availableItems
        .flatMap((cat) => cat.items)
        .find((item) => item.value === itemValue);
      if (itemData) {
        spawnItems = [...spawnItems, { ...itemData, count }];
      }
    }
    spawnItemFilter = "";
  }

  function removeSpawnItem(itemValue) {
    spawnItems = spawnItems.filter((item) => item.value !== itemValue);
  }

  function updateSpawnItemCount(itemValue, newCount) {
    if (newCount <= 0) {
      removeSpawnItem(itemValue);
    } else {
      const item = spawnItems.find((item) => item.value === itemValue);
      if (item) {
        item.count = newCount;
        spawnItems = [...spawnItems];
      }
    }
  }

  function addSpawnBuff(buffValue, stacks = 1, duration = -1) {
    const existingBuff = spawnBuffs.find((buff) => buff.value === buffValue);
    if (existingBuff) {
      existingBuff.stacks += stacks;
    } else {
      const buffData = availableBuffs.find((buff) => buff.value === buffValue);
      if (buffData) {
        spawnBuffs = [...spawnBuffs, { ...buffData, stacks, duration }];
      }
    }
    spawnBuffFilter = "";
  }

  function removeSpawnBuff(buffValue) {
    spawnBuffs = spawnBuffs.filter((buff) => buff.value !== buffValue);
  }

  function updateSpawnBuff(buffValue, stacks, duration) {
    const buff = spawnBuffs.find((buff) => buff.value === buffValue);
    if (buff) {
      buff.stacks = stacks;
      buff.duration = duration;
      spawnBuffs = [...spawnBuffs];
    }
  }

  function clearAllConfiguration() {
    spawnItems = [];
    spawnBuffs = [];
    spawnItemFilter = "";
    spawnBuffFilter = "";
  }
</script>

{#if selectedMonster}
  <div class="card bg-base-200 shadow-lg">
    <div class="card-body">
      <h3 class="card-title text-lg mb-4">⚙️ Spawn Settings</h3>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-4">
        <div class="form-control">
          <label class="label" for="spawn-count">
            <span class="label-text">Count</span>
          </label>
          <input
            id="spawn-count"
            type="number"
            bind:value={spawnCount}
            min="1"
            max="50"
            class="input input-bordered"
          />
        </div>

        <div class="form-control">
          <label class="label" for="spawn-distance">
            <span class="label-text">Distance (m)</span>
          </label>
          <input
            id="spawn-distance"
            type="number"
            bind:value={spawnDistance}
            min="1"
            max="100"
            class="input input-bordered"
          />
        </div>
      </div>

      <div class="form-control mb-6">
        <label class="label" for="team-select">
          <span class="label-text">Team</span>
        </label>
        <select
          id="team-select"
          bind:value={selectedTeam}
          class="select select-bordered"
        >
          {#each availableTeams as team}
            <option value={team.value}>{team.emoji} {team.label}</option>
          {/each}
        </select>
      </div>

      <!-- Spawn Configuration Section -->
      <div class="divider">
        <div class="flex items-center justify-between">
          <span class="text-sm">Spawn Configuration</span>
          {#if spawnItems.length > 0 || spawnBuffs.length > 0}
            <button class="btn btn-ghost btn-xs" on:click={clearAllConfiguration}>
              Clear All
            </button>
          {/if}
        </div>
      </div>

      <!-- Item Configuration -->
      <div class="space-y-4">
        <h5 class="font-semibold flex items-center gap-2">
          <Package size={16} />
          Items for Spawned Monsters
        </h5>

        <!-- Item Search -->
        <div class="form-control">
          <div class="input-group">
            <span class="bg-base-200 px-4">🔍</span>
            <input
              type="text"
              bind:value={spawnItemFilter}
              placeholder="Search items to add..."
              class="input input-bordered flex-1"
            />
            {#if spawnItemFilter}
              <button
                class="btn btn-square btn-outline"
                on:click={() => (spawnItemFilter = "")}
                title="Clear search"
              >
                ×
              </button>
            {/if}
          </div>
        </div>

        <!-- Suggested Items -->
        {#if suggestedSpawnItems.length > 0}
          <div class="bg-base-200 rounded-lg p-3">
            <p class="text-xs text-base-content/70 mb-2">
              💡 Suggested items matching "{spawnItemFilter}"
            </p>
            <div class="flex flex-wrap gap-2">
              {#each suggestedSpawnItems as item}
                <button
                  class="btn btn-outline btn-xs"
                  on:click={() => addSpawnItem(item.value, 1)}
                  title={item.description}
                >
                  +1 {item.label}
                </button>
              {/each}
            </div>
          </div>
        {/if}

        <!-- Current Spawn Items -->
        {#if spawnItems.length > 0}
          <div class="space-y-2">
            <p class="text-sm font-medium">Items to give ({spawnItems.length})</p>
            {#each spawnItems as item}
              <div class="flex items-center justify-between bg-base-200 rounded p-2">
                <span class="text-sm">{item.label}</span>
                <div class="flex items-center gap-2">
                  <button
                    class="btn btn-xs btn-outline"
                    on:click={() => updateSpawnItemCount(item.value, item.count - 1)}
                  >
                    -
                  </button>
                  <span class="text-sm min-w-[2rem] text-center">×{item.count}</span>
                  <button
                    class="btn btn-xs btn-outline"
                    on:click={() => updateSpawnItemCount(item.value, item.count + 1)}
                  >
                    +
                  </button>
                  <button
                    class="btn btn-xs btn-error"
                    on:click={() => removeSpawnItem(item.value)}
                  >
                    ×
                  </button>
                </div>
              </div>
            {/each}
          </div>
        {/if}

        <!-- Buff Configuration -->
        <h5 class="font-semibold flex items-center gap-2 mt-6">
          <Zap size={16} />
          Buffs for Spawned Monsters
        </h5>

        <!-- Buff Search -->
        <div class="form-control">
          <div class="input-group">
            <span class="bg-base-200 px-4">⚡</span>
            <input
              type="text"
              bind:value={spawnBuffFilter}
              placeholder="Search buffs to add..."
              class="input input-bordered flex-1"
            />
            {#if spawnBuffFilter}
              <button
                class="btn btn-square btn-outline"
                on:click={() => (spawnBuffFilter = "")}
                title="Clear search"
              >
                ×
              </button>
            {/if}
          </div>
        </div>

        <!-- Suggested Buffs -->
        {#if suggestedSpawnBuffs.length > 0}
          <div class="bg-base-200 rounded-lg p-3">
            <p class="text-xs text-base-content/70 mb-2">
              ⚡ Suggested buffs matching "{spawnBuffFilter}"
            </p>
            <div class="flex flex-wrap gap-2">
              {#each suggestedSpawnBuffs as buff}
                <button
                  class="btn btn-outline btn-xs"
                  on:click={() => addSpawnBuff(buff.value, 1, -1)}
                  title={buff.description}
                >
                  + {buff.label}
                </button>
              {/each}
            </div>
          </div>
        {/if}

        <!-- Quick Buff Buttons -->
        <div class="flex flex-wrap gap-2">
          {#each availableBuffs.slice(0, 4) as buff}
            <button
              class="btn btn-outline btn-sm"
              on:click={() => addSpawnBuff(buff.value, 1, -1)}
              title={buff.description}
              disabled={spawnBuffs.some((b) => b.value === buff.value)}
            >
              + {buff.label}
            </button>
          {/each}
        </div>

        <!-- Current Spawn Buffs -->
        {#if spawnBuffs.length > 0}
          <div class="space-y-2">
            <p class="text-sm font-medium">Buffs to give ({spawnBuffs.length})</p>
            {#each spawnBuffs as buff}
              <div class="bg-base-200 rounded p-2">
                <div class="flex items-center justify-between mb-2">
                  <span class="text-sm font-medium">{buff.label}</span>
                  <button
                    class="btn btn-xs btn-error"
                    on:click={() => removeSpawnBuff(buff.value)}
                  >
                    ×
                  </button>
                </div>
                <div class="grid grid-cols-2 gap-2">
                  <div class="form-control">
                    <label class="label py-1" for="spawn-buff-stacks-{buff.value}">
                      <span class="label-text text-xs">Stacks</span>
                    </label>
                    <input
                      id="spawn-buff-stacks-{buff.value}"
                      type="number"
                      min="1"
                      max="100"
                      value={buff.stacks}
                      on:input={(e) =>
                        updateSpawnBuff(
                          buff.value,
                          parseInt(e.target.value) || 1,
                          buff.duration,
                        )}
                      class="input input-bordered input-sm"
                    />
                  </div>
                  <div class="form-control">
                    <label class="label py-1" for="spawn-buff-duration-{buff.value}">
                      <span class="label-text text-xs">Duration (s)</span>
                    </label>
                    <input
                      id="spawn-buff-duration-{buff.value}"
                      type="number"
                      min="-1"
                      max="3600"
                      value={buff.duration}
                      on:input={(e) =>
                        updateSpawnBuff(
                          buff.value,
                          buff.stacks,
                          parseInt(e.target.value) || -1,
                        )}
                      class="input input-bordered input-sm"
                      placeholder="-1 = permanent"
                    />
                  </div>
                </div>
              </div>
            {/each}
          </div>
        {/if}
      </div>

      <button
        class="btn btn-primary w-full mt-6"
        on:click={onSpawnMonster}
        disabled={!selectedMonster || isSpawning}
      >
        <Target size={16} />
        {isSpawning
          ? "Spawning..."
          : `Spawn ${spawnCount}x ${selectedMonster.label}${spawnItems.length > 0 || spawnBuffs.length > 0 ? " (Configured)" : ""}`}
      </button>

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
    </div>
  </div>
{/if}