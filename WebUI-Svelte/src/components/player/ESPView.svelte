<script>
  import { selectedPlayer, gameState } from "../../lib/stores.js";
  import { api } from "../../lib/api.js";
  import { Search, MapPin, Eye, EyeOff, Package, Target } from "lucide-svelte";
  import Button from "../Button.svelte";

  // Import ESP components
  import ESPControls from "../esp/ESPControls.svelte";
  import ESPFilters from "../esp/ESPFilters.svelte";
  import ESPOverlayControls from "../esp/ESPOverlayControls.svelte";
  import ESPDebug from "../esp/ESPDebug.svelte";
  import MonstersSection from "../esp/MonstersSection.svelte";
  import InteractablesSection from "../esp/InteractablesSection.svelte";
  import ItemsSection from "../esp/ItemsSection.svelte";

  export let refreshGameState = () => {};

  // ESP Toggle States
  let showMonsters = true;
  let showInteractables = true;
  let showItems = true;
  let showDistances = true;
  let showHealth = true;
  let showElites = true;

  // Filter States
  let monsterFilter = "";
  let interactableFilter = "";
  let itemFilter = "";
  let maxDistance = 100; // meters
  let selectedCategory = "all";
  let selectedItemTier = "all";

  // Sort options
  let sortBy = "distance"; // 'distance', 'name', 'health'
  let sortOrder = "asc";

  // Teleport options
  let teleportTarget = "selected"; // 'selected' or 'all'
  let teleportOffset = 2.5; // Height offset above target

  // ESP Overlay options (disabled by default)
  let showMonsterOverlays = false;
  let showInteractableOverlays = false;
  let showItemOverlays = false;
  let overlayDistance = 50; // meters
  let overlayScale = 1.0;
  let showOverlayDistances = true;
  let showOverlayHealth = true;

  // Get monsters from game state
  $: monsters = $gameState?.Monsters || [];
  $: interactables = $gameState?.Interactables || [];
  $: items = $gameState?.Items || [];

  // Debug logging
  $: {
    console.log("ESP Debug - Game State:", $gameState);
    console.log("ESP Debug - Monsters:", monsters);
    console.log("ESP Debug - Interactables:", interactables);
    console.log("ESP Debug - Items:", items);
  }

  // Filter and sort monsters
  $: filteredMonsters = monsters
    .filter((monster) => {
      if (!showMonsters) return false;
      if (!showElites && monster.IsElite) return false;
      if (monster.Distance > maxDistance) return false;
      if (
        monsterFilter &&
        !monster.DisplayName.toLowerCase().includes(monsterFilter.toLowerCase())
      )
        return false;
      return true;
    })
    .sort((a, b) => {
      let comparison = 0;
      switch (sortBy) {
        case "distance":
          comparison = a.Distance - b.Distance;
          break;
        case "name":
          comparison = a.DisplayName.localeCompare(b.DisplayName);
          break;
        case "health":
          comparison = a.Health / a.MaxHealth - b.Health / b.MaxHealth;
          break;
      }
      return sortOrder === "asc" ? comparison : -comparison;
    });

  // Filter and sort interactables
  $: filteredInteractables = interactables
    .filter((interactable) => {
      if (!showInteractables) return false;
      if (interactable.Distance > maxDistance) return false;
      if (
        selectedCategory !== "all" &&
        interactable.Category !== selectedCategory
      )
        return false;
      if (
        interactableFilter &&
        !interactable.DisplayName.toLowerCase().includes(
          interactableFilter.toLowerCase(),
        )
      )
        return false;
      return true;
    })
    .sort((a, b) => {
      let comparison = 0;
      switch (sortBy) {
        case "distance":
          comparison = a.Distance - b.Distance;
          break;
        case "name":
          comparison = a.DisplayName.localeCompare(b.DisplayName);
          break;
      }
      return sortOrder === "asc" ? comparison : -comparison;
    });

  // Filter and sort items
  $: filteredItems = items
    .filter((item) => {
      if (!showItems) return false;
      if (item.Distance > maxDistance) return false;
      if (selectedItemTier !== "all" && item.ItemTier !== selectedItemTier)
        return false;
      if (
        itemFilter &&
        !item.DisplayName.toLowerCase().includes(itemFilter.toLowerCase())
      )
        return false;
      return true;
    })
    .sort((a, b) => {
      let comparison = 0;
      switch (sortBy) {
        case "distance":
          comparison = a.Distance - b.Distance;
          break;
        case "name":
          comparison = a.DisplayName.localeCompare(b.DisplayName);
          break;
      }
      return sortOrder === "asc" ? comparison : -comparison;
    });

  // Get unique categories and item tiers
  $: categories = [...new Set(interactables.map((i) => i.Category))].filter(
    Boolean,
  );
  $: itemTiers = [...new Set(items.map((i) => i.ItemTier))].filter(Boolean);

  // Get health percentage
  function getHealthPercentage(monster) {
    return monster.MaxHealth > 0
      ? (monster.Health / monster.MaxHealth) * 100
      : 0;
  }

  // Get health color
  function getHealthColor(percentage) {
    if (percentage > 75) return "#4ade80";
    if (percentage > 50) return "#fbbf24";
    if (percentage > 25) return "#fb923c";
    return "#ef4444";
  }

  // Get elite color
  function getEliteColor(eliteType) {
    switch (eliteType?.toLowerCase()) {
      case "fire":
        return "#ef4444";
      case "ice":
        return "#3b82f6";
      case "lightning":
        return "#eab308";
      case "poison":
        return "#22c55e";
      case "celestine":
        return "#a855f7";
      case "malachite":
        return "#10b981";
      case "perfected":
        return "#f59e0b";
      default:
        return "#6b7280";
    }
  }

  // Format distance
  function formatDistance(distance) {
    return distance < 1000
      ? `${distance.toFixed(1)}m`
      : `${(distance / 1000).toFixed(2)}km`;
  }

  // Format position
  function formatPosition(x, y, z) {
    return `(${x.toFixed(1)}, ${y.toFixed(1)}, ${z.toFixed(1)})`;
  }

  // Toggle functions
  function toggleSort(newSortBy) {
    if (sortBy === newSortBy) {
      sortOrder = sortOrder === "asc" ? "desc" : "asc";
    } else {
      sortBy = newSortBy;
      sortOrder = "asc";
    }
  }

  // Teleport to entity position
  async function teleportToPosition(x, y, z, entityName = "position") {
    try {
      // Determine target player ID based on teleport target setting
      const targetPlayerId =
        teleportTarget === "all" ? -1 : $selectedPlayer?.PlayerId || -1;

      await api.teleportPlayer(x, y, z, targetPlayerId, teleportOffset);

      // Show success message
      const targetText =
        teleportTarget === "all"
          ? "all players"
          : $selectedPlayer?.PlayerName || "player";
      console.log(
        `Successfully teleported ${targetText} to ${entityName} at (${x.toFixed(1)}, ${y.toFixed(1)}, ${z.toFixed(1)})`,
      );

      // Optional: Show a toast notification (if you have a toast system)
      // showToast(`Teleported ${targetText} to ${entityName}`, 'success');
    } catch (error) {
      console.error("Failed to teleport:", error);
      // Optional: Show error toast
      // showToast('Teleport failed', 'error');
    }
  }

  // Toggle ESP overlay labels
  async function toggleOverlay(overlayType) {
    try {
      let currentState, newState;

      switch (overlayType) {
        case "monsters":
          currentState = showMonsterOverlays;
          showMonsterOverlays = !showMonsterOverlays;
          newState = showMonsterOverlays;
          break;
        case "interactables":
          currentState = showInteractableOverlays;
          showInteractableOverlays = !showInteractableOverlays;
          newState = showInteractableOverlays;
          break;
        case "items":
          currentState = showItemOverlays;
          showItemOverlays = !showItemOverlays;
          newState = showItemOverlays;
          break;
      }

      await api.toggleESPOverlay(overlayType, newState);
      console.log(
        `ESP ${overlayType} overlay ${newState ? "enabled" : "disabled"}`,
      );
    } catch (error) {
      console.error("Failed to toggle ESP overlay:", error);
      // Revert state on error
      switch (overlayType) {
        case "monsters":
          showMonsterOverlays = !showMonsterOverlays;
          break;
        case "interactables":
          showInteractableOverlays = !showInteractableOverlays;
          break;
        case "items":
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
        showHealth: showOverlayHealth,
      });
      console.log("ESP overlay configuration updated");
    } catch (error) {
      console.error("Failed to update ESP overlay config:", error);
    }
  }
</script>

<div class="space-y-6">
  <!-- ESP Controls -->
  <ESPControls
    bind:showMonsters
    bind:showInteractables
    bind:showItems
    bind:showDistances
    bind:showHealth
    bind:showElites
    {filteredMonsters}
    {filteredInteractables}
    {filteredItems}
  />

  <!-- Filters & Sorting -->
  <ESPFilters
    bind:maxDistance
    bind:sortBy
    bind:sortOrder
    bind:teleportTarget
    bind:teleportOffset
    {refreshGameState}
  />

  <!-- ESP Overlay Controls -->
  <ESPOverlayControls
    bind:showMonsterOverlays
    bind:showInteractableOverlays
    bind:showItemOverlays
    bind:overlayDistance
    bind:overlayScale
    bind:showOverlayDistances
    bind:showOverlayHealth
  />

  <!-- Debug Information -->
  <ESPDebug
    gameState={$gameState}
    {monsters}
    {interactables}
    {items}
    {filteredMonsters}
    {filteredInteractables}
    {filteredItems}
    {maxDistance}
  />

  <!-- ESP Content -->
  <div class="space-y-6">
    <!-- Monsters Section -->
    <MonstersSection
      {showMonsters}
      {filteredMonsters}
      {showDistances}
      {showHealth}
      bind:monsterFilter
      onTeleport={teleportToPosition}
    />

    <!-- Interactables Section -->
    <InteractablesSection
      {showInteractables}
      {filteredInteractables}
      {categories}
      bind:selectedCategory
      bind:interactableFilter
      {showDistances}
      onTeleport={teleportToPosition}
    />

    <!-- Items Section -->
    <ItemsSection
      {showItems}
      {filteredItems}
      {itemTiers}
      bind:selectedItemTier
      bind:itemFilter
      {showDistances}
      onTeleport={teleportToPosition}
    />
  </div>
</div>

<!-- DaisyUI Enhanced ESP View - Modular components with modern styling -->
