<script>
  import { selectedPlayer, gameState } from "../lib/stores.js";
  import { api } from "../lib/api.js";

  import Button from "./Button.svelte";
  import {
    User,
    Users,
    Settings,
    RotateCcw,
    Activity,
    Package,
  } from "lucide-svelte";
  import { onMount } from "svelte";

  // Import our new components
  import StatsOverview from "./player/StatsOverview.svelte";
  import CharacterInfo from "./player/CharacterInfo.svelte";
  import BasicControls from "./player/BasicControls.svelte";
  import AdvancedStats from "./player/AdvancedStats.svelte";
  import PlayerActions from "./player/PlayerActions.svelte";
  import ItemManagement from "./player/ItemManagement.svelte";

  // UI State
  let activeTab = "overview";
  let targetMode = "selected"; // 'selected' or 'all'
  let isProcessing = false;

  // Item management state
  let itemFilter = "";
  let expandedItems = new Set();

  // Character and basic controls
  let selectedCharacter = "CommandoBody";

  // Stat editing state - these will be populated with current values
  let statInputs = {
    // Basic stats
    level: "",
    experience: "",
    healthPercent: "",
    money: "",

    // Combat stats
    baseDamage: "",
    armor: "",
    attackSpeed: "",
    critChance: "",

    // Movement stats
    moveSpeed: "",
    jumpPower: "",

    // Health stats
    maxHealth: "",
    maxShield: "",
    healthRegen: "",
  };

  // Dynamic item catalog
  let availableItems = [];

  // Character defaults
  let characterDefaults = [];
  let characterDefaultsLoaded = false;

  // Auto-load current values when player changes
  $: if ($selectedPlayer) {
    loadPlayerDefaults();
  }

  function loadPlayerDefaults() {
    if (!$selectedPlayer) return;

    // Update character selection
    selectedCharacter = $selectedPlayer.CurrentCharacter || "CommandoBody";

    // Load current stat values (only if inputs are empty to avoid overwriting user input)
    if (!statInputs.level)
      statInputs.level = $selectedPlayer.Level?.toFixed(0) || "";
    if (!statInputs.experience)
      statInputs.experience = $selectedPlayer.Experience?.toFixed(0) || "";
    if (!statInputs.healthPercent && $selectedPlayer.MaxHealth > 0) {
      statInputs.healthPercent = (
        (($selectedPlayer.Health || 0) / $selectedPlayer.MaxHealth) *
        100
      ).toFixed(0);
    }

    // Load advanced stats
    if (!statInputs.baseDamage)
      statInputs.baseDamage = $selectedPlayer.BaseDamage?.toFixed(1) || "";
    if (!statInputs.armor)
      statInputs.armor = $selectedPlayer.Armor?.toFixed(1) || "";
    if (!statInputs.attackSpeed)
      statInputs.attackSpeed = $selectedPlayer.AttackSpeed?.toFixed(2) || "";
    if (!statInputs.critChance)
      statInputs.critChance = $selectedPlayer.CritChance?.toFixed(1) || "";
    if (!statInputs.moveSpeed)
      statInputs.moveSpeed = $selectedPlayer.MoveSpeed?.toFixed(1) || "";
    if (!statInputs.jumpPower)
      statInputs.jumpPower = $selectedPlayer.JumpPower?.toFixed(1) || "";
    if (!statInputs.maxHealth)
      statInputs.maxHealth = $selectedPlayer.MaxHealth?.toFixed(0) || "";
    if (!statInputs.maxShield)
      statInputs.maxShield = $selectedPlayer.MaxShield?.toFixed(0) || "";
    if (!statInputs.healthRegen)
      statInputs.healthRegen = $selectedPlayer.HealthRegen?.toFixed(2) || "";
  }

  // Force load current values (for reset buttons)
  function forceLoadCurrentValues() {
    if (!$selectedPlayer) return;

    statInputs.level = $selectedPlayer.Level?.toFixed(0) || "";
    statInputs.experience = $selectedPlayer.Experience?.toFixed(0) || "";
    if ($selectedPlayer.MaxHealth > 0) {
      statInputs.healthPercent = (
        (($selectedPlayer.Health || 0) / $selectedPlayer.MaxHealth) *
        100
      ).toFixed(0);
    }

    statInputs.baseDamage = $selectedPlayer.BaseDamage?.toFixed(1) || "";
    statInputs.armor = $selectedPlayer.Armor?.toFixed(1) || "";
    statInputs.attackSpeed = $selectedPlayer.AttackSpeed?.toFixed(2) || "";
    statInputs.critChance = $selectedPlayer.CritChance?.toFixed(1) || "";
    statInputs.moveSpeed = $selectedPlayer.MoveSpeed?.toFixed(1) || "";
    statInputs.jumpPower = $selectedPlayer.JumpPower?.toFixed(1) || "";
    statInputs.maxHealth = $selectedPlayer.MaxHealth?.toFixed(0) || "";
    statInputs.maxShield = $selectedPlayer.MaxShield?.toFixed(0) || "";
    statInputs.healthRegen = $selectedPlayer.HealthRegen?.toFixed(2) || "";
  }

  // Load item catalog from the game
  async function loadItemCatalog() {
    try {
      // First try to get existing catalog
      let catalog = await api.getItemCatalog();

      // If empty, refresh it from the game
      if (!catalog || catalog.length === 0) {
        await api.refreshItemCatalog();
        // Wait a bit for the command to process
        await new Promise((resolve) => setTimeout(resolve, 500));
        catalog = await api.getItemCatalog();
      }

      if (catalog && catalog.length > 0) {
        // Group items by category
        const groupedItems = {};
        catalog.forEach((item) => {
          if (!groupedItems[item.Category]) {
            groupedItems[item.Category] = [];
          }
          groupedItems[item.Category].push({
            value: item.Value,
            label: item.Label,
            description: item.Description,
          });
        });

        // Convert to the format expected by SearchableItemSelect
        availableItems = Object.entries(groupedItems).map(
          ([category, items]) => ({
            category,
            items,
          }),
        );

        console.log(
          `Loaded ${catalog.length} items in ${availableItems.length} categories`,
        );
      } else {
        console.warn("No items found in catalog, using fallback");
        // Fallback to basic items if catalog fails
        availableItems = [
          {
            category: "Common Items",
            items: [
              { value: "Syringe", label: "Soldier's Syringe" },
              { value: "Bear", label: "Tougher Times" },
              { value: "Crowbar", label: "Crowbar" },
            ],
          },
        ];
      }
    } catch (error) {
      console.error("Failed to load item catalog:", error);
      // Fallback to basic items
      availableItems = [
        {
          category: "Common Items",
          items: [
            { value: "Syringe", label: "Soldier's Syringe" },
            { value: "Bear", label: "Tougher Times" },
            { value: "Crowbar", label: "Crowbar" },
          ],
        },
      ];
    }
  }

  // Load character defaults from the game
  async function loadCharacterDefaults() {
    try {
      // First try to get existing defaults
      let defaults = await api.getCharacterDefaults();

      // If empty, refresh them from the game
      if (!defaults || defaults.length === 0) {
        await api.refreshCharacterDefaults();
        // Wait a bit for the command to process
        await new Promise((resolve) => setTimeout(resolve, 500));
        defaults = await api.getCharacterDefaults();
      }

      if (defaults && defaults.length > 0) {
        characterDefaults = defaults;
        characterDefaultsLoaded = true;
        console.log(`Loaded ${defaults.length} character defaults`);
      } else {
        console.warn("No character defaults found");
        characterDefaults = [];
      }
    } catch (error) {
      console.error("Failed to load character defaults:", error);
      characterDefaults = [];
    }
  }

  // Get default stats for a character
  function getCharacterDefaults(characterName) {
    return (
      characterDefaults.find((char) => char.CharacterName === characterName) ||
      null
    );
  }

  // Load default stats for selected character
  function loadCharacterDefaultStats() {
    if (!selectedCharacter) return;

    const defaults = getCharacterDefaults(selectedCharacter);
    if (!defaults) {
      console.warn(`No defaults found for ${selectedCharacter}`);
      return;
    }

    // Load base stats (level 1)
    statInputs.baseDamage = defaults.BaseDamage?.toFixed(1) || "";
    statInputs.armor = defaults.BaseArmor?.toFixed(1) || "";
    statInputs.attackSpeed = defaults.BaseAttackSpeed?.toFixed(2) || "";
    statInputs.critChance = defaults.BaseCrit?.toFixed(1) || "";
    statInputs.moveSpeed = defaults.BaseMoveSpeed?.toFixed(1) || "";
    statInputs.jumpPower = defaults.BaseJumpPower?.toFixed(1) || "";
    statInputs.maxHealth = defaults.BaseMaxHealth?.toFixed(0) || "";
    statInputs.maxShield = defaults.BaseMaxShield?.toFixed(0) || "";
    statInputs.healthRegen = defaults.BaseRegen?.toFixed(2) || "";

    console.log(`Loaded default stats for ${defaults.DisplayName}`);
  }

  // Load catalog when component mounts
  onMount(() => {
    loadItemCatalog();
    loadCharacterDefaults();
  });

  async function refreshGameState() {
    try {
      isProcessing = true;
      await api.refreshGameState();
      const state = await api.getGameState();
      if (state) {
        gameState.set(state);
      }
    } catch (error) {
      console.error("Failed to refresh game state:", error);
    } finally {
      isProcessing = false;
    }
  }

  // Handle character change from BasicControls
  function handleCharacterChange() {
    loadCharacterDefaultStats();
  }

  // Item management functions
  $: filteredItems = Object.entries($selectedPlayer?.Items || {}).filter(
    ([itemName, count]) => {
      if (count <= 0) return false;
      if (!itemFilter.trim()) return true;
      return itemName.toLowerCase().includes(itemFilter.toLowerCase());
    },
  );

  $: suggestedItems = itemFilter.trim()
    ? availableItems
        .flatMap((category) =>
          category.items.filter((item) => {
            const hasItem = $selectedPlayer?.Items?.[item.value] > 0;
            const matchesSearch =
              item.label.toLowerCase().includes(itemFilter.toLowerCase()) ||
              item.value.toLowerCase().includes(itemFilter.toLowerCase());
            return !hasItem && matchesSearch;
          }),
        )
        .slice(0, 6)
    : [];

  function toggleItemControls(itemName) {
    if (expandedItems.has(itemName)) {
      expandedItems.delete(itemName);
    } else {
      expandedItems.add(itemName);
    }
    expandedItems = expandedItems;
  }

  async function addSuggestedItem(itemValue, count = 1) {
    if (!$selectedPlayer) return;

    try {
      await api.spawnItem(itemValue, count, $selectedPlayer.PlayerId);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error("Failed to add suggested item:", error);
    }
  }

  async function modifyItem(itemName, change) {
    if (!$selectedPlayer) return;

    try {
      await api.spawnItem(itemName, change, $selectedPlayer.PlayerId);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error("Failed to modify item:", error);
    }
  }

  async function removeItem(itemName) {
    if (!$selectedPlayer) return;

    const currentCount = $selectedPlayer.Items[itemName] || 0;
    if (currentCount > 0) {
      try {
        await api.spawnItem(itemName, -currentCount, $selectedPlayer.PlayerId);
        setTimeout(refreshGameState, 50);
      } catch (error) {
        console.error("Failed to remove item:", error);
      }
    }
  }

  function getItemDisplayName(itemName) {
    // Simple display name function - you can enhance this
    return itemName.replace(/([A-Z])/g, " $1").trim() || itemName;
  }
</script>

{#if $selectedPlayer}
  <div
    class="space-y-6"
    class:opacity-70={isProcessing}
    class:pointer-events-none={isProcessing}
  >
    <!-- Header Card -->
    <div class="card bg-base-200 shadow-xl">
      <div class="card-body">
        <div
          class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4"
        >
          <!-- Player Info -->
          <div class="flex items-center gap-4">
            <div class="avatar placeholder">
              <div class="bg-primary text-primary-content rounded-full w-16">
                <span class="text-2xl">🎮</span>
              </div>
            </div>
            <div>
              <h1 class="card-title text-2xl">{$selectedPlayer.PlayerName}</h1>
              <div class="flex items-center gap-2 text-sm opacity-70">
                <div class="badge badge-primary">
                  Level {$selectedPlayer.Level?.toFixed(0) || "0"}
                </div>
                <div
                  class="badge {$selectedPlayer.IsAlive
                    ? 'badge-success'
                    : 'badge-error'}"
                >
                  {$selectedPlayer.IsAlive ? "Alive" : "Dead"}
                </div>
              </div>
            </div>
          </div>

          <!-- Target Mode Selector -->
          <div class="flex flex-col items-end gap-2">
            <span class="text-sm font-medium">Apply Changes To:</span>
            <div class="join">
              <button
                class="btn btn-sm join-item {targetMode === 'selected'
                  ? 'btn-primary'
                  : 'btn-outline'}"
                on:click={() => (targetMode = "selected")}
              >
                <User size={14} />
                This Player
              </button>
              <button
                class="btn btn-sm join-item {targetMode === 'all'
                  ? 'btn-primary'
                  : 'btn-outline'}"
                on:click={() => (targetMode = "all")}
              >
                <Users size={14} />
                All Players
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="card bg-base-200 shadow-lg">
      <div class="card-body py-4">
        <div class="flex flex-wrap gap-2 justify-center">
          <button
            class="btn btn-sm btn-outline"
            on:click={forceLoadCurrentValues}
          >
            <RotateCcw size={14} />
            Load Current Values
          </button>
          <button
            class="btn btn-sm btn-outline"
            on:click={loadCharacterDefaultStats}
            disabled={!characterDefaultsLoaded}
          >
            <Settings size={14} />
            Load Defaults
          </button>
          <button
            class="btn btn-sm btn-outline"
            on:click={refreshGameState}
            disabled={isProcessing}
          >
            <Activity size={14} />
            {isProcessing ? "Refreshing..." : "Refresh"}
          </button>
        </div>
      </div>
    </div>

    <!-- Stats Overview -->
    <div class="card bg-base-200 shadow-lg">
      <div class="card-body">
        <h2 class="card-title">📊 Player Stats</h2>
        <StatsOverview />
      </div>
    </div>

    <!-- Player Actions -->
    <div class="card bg-base-200 shadow-lg">
      <div class="card-body">
        <h2 class="card-title">⚡ Quick Actions</h2>
        <PlayerActions {refreshGameState} />
      </div>
    </div>

    <!-- Main Content Tabs -->
    <div class="card bg-base-200 shadow-lg">
      <div class="card-body">
        <!-- Tab Navigation -->
        <div class="tabs tabs-boxed mb-6">
          <button
            class="tab {activeTab === 'overview' ? 'tab-active' : ''}"
            on:click={() => (activeTab = "overview")}
          >
            <Activity size={16} class="mr-2" />
            Stats & Controls
          </button>
          <button
            class="tab {activeTab === 'items' ? 'tab-active' : ''}"
            on:click={() => (activeTab = "items")}
          >
            <Package size={16} class="mr-2" />
            Items
          </button>
        </div>

        <!-- Tab Content Area -->
        <div class="min-h-[400px]">
          <div class="alert alert-info mb-4">
            <div>
              <h4 class="font-bold">Tab System Test</h4>
              <p>Active Tab: <strong>{activeTab}</strong></p>
              <p>
                Selected Player: <strong
                  >{$selectedPlayer?.PlayerName || "None"}</strong
                >
              </p>
              <p>This should always be visible when you click tabs</p>
            </div>
          </div>

          {#if activeTab === "overview"}
            <div class="card bg-success text-success-content">
              <div class="card-body">
                <h2 class="card-title">✅ Stats & Controls Tab Active</h2>
                <p>This is the Stats & Controls content area.</p>
                <p>
                  Player: {$selectedPlayer?.PlayerName || "No player selected"}
                </p>

                <!-- Simple test controls -->
                <div class="card bg-base-100 text-base-content mt-4">
                  <div class="card-body">
                    <h3 class="card-title">🎮 Test Controls</h3>
                    <div class="form-control">
                      <label class="label">
                        <span class="label-text">Test Input</span>
                      </label>
                      <input
                        type="text"
                        placeholder="Type something..."
                        class="input input-bordered"
                      />
                    </div>
                    <button class="btn btn-primary mt-2">Test Button</button>
                  </div>
                </div>
              </div>
            </div>
          {:else if activeTab === "items"}
            <!-- Item Management Interface -->
            <div class="space-y-6">
              <!-- Item Search Section -->
              <div
                class="card bg-gradient-to-r from-primary/10 to-secondary/10 shadow-lg"
              >
                <div class="card-body">
                  <h3 class="card-title text-lg mb-4">🔍 Item Search & Add</h3>
                  <div class="form-control">
                    <div class="input-group">
                      <span class="bg-base-200 px-4">🔍</span>
                      <input
                        type="text"
                        bind:value={itemFilter}
                        placeholder="Search for items to add (e.g., 'syringe', 'crowbar', 'bear')..."
                        class="input input-bordered flex-1 input-lg"
                      />
                      {#if itemFilter}
                        <button
                          class="btn btn-square btn-outline btn-lg"
                          on:click={() => (itemFilter = "")}
                          title="Clear search"
                        >
                          ×
                        </button>
                      {/if}
                    </div>
                    <label class="label">
                      <span class="label-text-alt"
                        >💡 Tip: Start typing to see item suggestions appear
                        below</span
                      >
                    </label>
                  </div>
                </div>
              </div>

              <!-- Item Suggestions -->
              {#if itemFilter && suggestedItems.length > 0}
                <div
                  class="card bg-success/10 border border-success/20 shadow-lg"
                >
                  <div class="card-body">
                    <h4 class="card-title text-success">
                      💡 Suggested Items ({suggestedItems.length})
                    </h4>
                    <p class="text-sm opacity-70 mb-4">
                      Items matching "{itemFilter}" that you can add
                    </p>

                    <div
                      class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3"
                    >
                      {#each suggestedItems as item}
                        <div
                          class="card bg-base-100 shadow-md hover:shadow-lg transition-all"
                        >
                          <div class="card-body p-4">
                            <div class="flex justify-between items-start mb-3">
                              <div class="flex-1">
                                <h5 class="font-semibold text-sm">
                                  {item.label}
                                </h5>
                                <p class="text-xs opacity-60">
                                  {availableItems.find((cat) =>
                                    cat.items.includes(item),
                                  )?.category || "Item"}
                                </p>
                              </div>
                            </div>
                            <div class="flex gap-2">
                              <button
                                class="btn btn-success btn-sm flex-1"
                                on:click={() => addSuggestedItem(item.value, 1)}
                              >
                                +1
                              </button>
                              <button
                                class="btn btn-success btn-sm flex-1"
                                on:click={() => addSuggestedItem(item.value, 5)}
                              >
                                +5
                              </button>
                              <button
                                class="btn btn-success btn-sm flex-1"
                                on:click={() =>
                                  addSuggestedItem(item.value, 10)}
                              >
                                +10
                              </button>
                            </div>
                          </div>
                        </div>
                      {/each}
                    </div>
                  </div>
                </div>
              {/if}

              <!-- Current Items Display -->
              <div class="card bg-base-100 shadow-lg">
                <div class="card-body">
                  <h4 class="card-title">📦 Current Items</h4>
                  <p class="text-sm opacity-70 mb-4">
                    Player: {$selectedPlayer?.PlayerName ||
                      "No player selected"}
                  </p>

                  {#if $selectedPlayer?.Items && Object.keys($selectedPlayer.Items).length > 0}
                    <div class="alert alert-info">
                      <div>
                        <h5 class="font-bold">Items Available</h5>
                        <p>
                          This player has {Object.keys(
                            $selectedPlayer.Items,
                          ).filter((key) => $selectedPlayer.Items[key] > 0)
                            .length} different items
                        </p>
                      </div>
                    </div>
                  {:else}
                    <div class="alert alert-warning">
                      <div>
                        <h5 class="font-bold">No Items</h5>
                        <p>
                          This player doesn't have any items yet. Use the search
                          above to add items!
                        </p>
                      </div>
                    </div>
                  {/if}
                </div>
              </div>
            </div>
          {:else}
            <div class="card bg-error text-error-content">
              <div class="card-body">
                <h2 class="card-title">❌ Unknown Tab</h2>
                <p>Active tab "{activeTab}" is not recognized.</p>
              </div>
            </div>
          {/if}
        </div>
      </div>
    </div>

    <!-- Processing Overlay -->
    {#if isProcessing}
      <div
        class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      >
        <div class="card bg-base-100 shadow-xl">
          <div class="card-body items-center text-center">
            <span class="loading loading-spinner loading-lg text-primary"
            ></span>
            <h3 class="card-title">Processing...</h3>
            <p>Please wait while we update the game state.</p>
          </div>
        </div>
      </div>
    {/if}
  </div>
{:else}
  <!-- No Player Selected -->
  <div class="hero min-h-[60vh]">
    <div class="hero-content text-center">
      <div class="max-w-md">
        <div class="avatar placeholder mb-6">
          <div class="bg-neutral text-neutral-content rounded-full w-24">
            <User size={48} />
          </div>
        </div>
        <h1 class="text-3xl font-bold">No Player Selected</h1>
        <p class="py-6">
          Select a player from the list above to manage their stats, items, and
          settings. Player management tools will appear here once you make a
          selection.
        </p>
      </div>
    </div>
  </div>
{/if}
