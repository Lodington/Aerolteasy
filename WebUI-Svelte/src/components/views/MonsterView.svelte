<script>
  import { api } from "../../lib/api.js";
  import { onMount } from "svelte";
  import MonsterSelection from "../monster/MonsterSelection.svelte";
  import MonsterSpawnSettings from "../monster/MonsterSpawnSettings.svelte";
  import MonsterManagement from "../monster/MonsterManagement.svelte";

  // Monster spawning state
  let monsterSearch = "";
  let selectedMonster = null;
  let spawnCount = 1;
  let spawnDistance = 10;
  let selectedTeam = "Monster";
  let isSpawning = false;
  let statusMessage = "";

  // Monster management state
  let selectedItem = "";
  let itemCount = 1;
  let selectedBuff = "";
  let buffStacks = 1;
  let buffDuration = -1; // -1 means permanent
  let isGivingItem = false;
  let isGivingBuff = false;
  let managementStatusMessage = "";

  // Spawn configuration state
  let spawnItemFilter = "";
  let spawnBuffFilter = "";
  let spawnItems = []; // Items to give to spawned monsters
  let spawnBuffs = []; // Buffs to give to spawned monsters

  // Available items (simplified list for monsters)
  let availableItems = [];

  // Available buffs (common buffs that work well on monsters)
  const availableBuffs = [
    {
      value: "AttackSpeedOnCrit",
      label: "Predatory Instincts",
      description: "Attack speed boost",
    },
    {
      value: "ArmorBoost",
      label: "Armor Boost",
      description: "Increased armor",
    },
    {
      value: "BeetleJuice",
      label: "Beetle Juice",
      description: "Damage boost",
    },
    {
      value: "CloakSpeed",
      label: "Cloak Speed",
      description: "Movement speed boost",
    },
    {
      value: "EnergizedOnEquipmentUse",
      label: "Energized",
      description: "Equipment cooldown reduction",
    },
    {
      value: "FullCrit",
      label: "Full Crit",
      description: "100% critical chance",
    },
    {
      value: "HiddenInvincibility",
      label: "Hidden Invincibility",
      description: "Temporary invincibility",
    },
    {
      value: "PowerBuff",
      label: "Power Buff",
      description: "Increased damage",
    },
    { value: "TeamWarCry", label: "War Cry", description: "Team damage boost" },
    {
      value: "Warbanner",
      label: "Warbanner",
      description: "Area damage boost",
    },
    {
      value: "EliteSecretSpeed",
      label: "Elite Speed",
      description: "Elite movement speed bonus",
    },
    {
      value: "EliteHaunted",
      label: "Elite Haunted",
      description: "Ghostly elite powers",
    },
    {
      value: "EliteFire",
      label: "Elite Fire",
      description: "Blazing elite abilities",
    },
    {
      value: "EliteIce",
      label: "Elite Ice",
      description: "Freezing elite powers",
    },
    {
      value: "EliteLightning",
      label: "Elite Lightning",
      description: "Electric elite abilities",
    },
    {
      value: "ElitePoison",
      label: "Elite Poison",
      description: "Toxic elite powers",
    },
    {
      value: "Immune",
      label: "Immune",
      description: "Immunity to damage",
    },
    {
      value: "Intangible",
      label: "Intangible",
      description: "Reduced damage taken",
    },
    {
      value: "NoCooldowns",
      label: "No Cooldowns",
      description: "Removes all ability cooldowns",
    },
    {
      value: "Overheat",
      label: "Overheat",
      description: "Increased attack speed and damage",
    },
  ];

  // Available monsters organized by category
  const monsters = [
    {
      category: "Stage 1 Enemies",
      monsters: [
        {
          value: "Beetle",
          label: "Beetle",
          description: "Basic melee enemy",
          emoji: "🪲",
        },
        {
          value: "BeetleGuard",
          label: "Beetle Guard",
          description: "Armored beetle",
          emoji: "🛡️",
        },
        {
          value: "Wisp",
          label: "Wisp",
          description: "Flying energy enemy",
          emoji: "👻",
        },
        {
          value: "Lemurian",
          label: "Lemurian",
          description: "Agile lizard",
          emoji: "🦎",
        },
        {
          value: "Golem",
          label: "Stone Golem",
          description: "Heavy tank",
          emoji: "🗿",
        },
      ],
    },
    {
      category: "Bosses",
      monsters: [
        {
          value: "ClayBoss",
          label: "Clay Dunestrider",
          description: "Massive clay boss",
          emoji: "🏔️",
        },
        {
          value: "Titan",
          label: "Stone Titan",
          description: "Giant golem boss",
          emoji: "🗿",
        },
        {
          value: "Vagrant",
          label: "Wandering Vagrant",
          description: "Floating energy boss",
          emoji: "💫",
        },
      ],
    },
  ];

  // Available teams
  const availableTeams = [
    { value: "Monster", label: "Monster Team", emoji: "👹" },
    { value: "Player", label: "Player Team", emoji: "🤝" },
    { value: "Neutral", label: "Neutral Team", emoji: "🤖" },
  ];

  // Search results
  $: monsterResults = monsterSearch.trim()
    ? monsters
        .flatMap((category) =>
          category.monsters
            .filter((monster) => {
              const searchLower = monsterSearch.toLowerCase();
              return (
                monster.label.toLowerCase().includes(searchLower) ||
                monster.value.toLowerCase().includes(searchLower)
              );
            })
            .map((monster) => ({ ...monster, category: category.category })),
        )
        .slice(0, 8)
    : [];

  // Spawn item suggestions (items matching search that aren't already added)
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

  // Spawn buff suggestions (buffs matching search that aren't already added)
  $: suggestedSpawnBuffs = spawnBuffFilter.trim()
    ? availableBuffs
        .filter((buff) => {
          const hasBuff = spawnBuffs.some(
            (spawnBuff) => spawnBuff.value === buff.value,
          );
          const matchesSearch =
            buff.label.toLowerCase().includes(spawnBuffFilter.toLowerCase()) ||
            buff.value.toLowerCase().includes(spawnBuffFilter.toLowerCase()) ||
            buff.description
              .toLowerCase()
              .includes(spawnBuffFilter.toLowerCase());
          return !hasBuff && matchesSearch;
        })
        .slice(0, 6)
    : [];

  function selectMonster(monster) {
    selectedMonster = monster;
    monsterSearch = "";
  }

  function clearMonster() {
    selectedMonster = null;
  }

  // Add item to spawn configuration
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
    spawnItemFilter = ""; // Clear search after adding
  }

  // Remove item from spawn configuration
  function removeSpawnItem(itemValue) {
    spawnItems = spawnItems.filter((item) => item.value !== itemValue);
  }

  // Update spawn item count
  function updateSpawnItemCount(itemValue, newCount) {
    if (newCount <= 0) {
      removeSpawnItem(itemValue);
    } else {
      const item = spawnItems.find((item) => item.value === itemValue);
      if (item) {
        item.count = newCount;
        spawnItems = [...spawnItems]; // Trigger reactivity
      }
    }
  }

  // Add buff to spawn configuration
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
    spawnBuffFilter = ""; // Clear search after adding
  }

  // Remove buff from spawn configuration
  function removeSpawnBuff(buffValue) {
    spawnBuffs = spawnBuffs.filter((buff) => buff.value !== buffValue);
  }

  // Update spawn buff configuration
  function updateSpawnBuff(buffValue, stacks, duration) {
    const buff = spawnBuffs.find((buff) => buff.value === buffValue);
    if (buff) {
      buff.stacks = stacks;
      buff.duration = duration;
      spawnBuffs = [...spawnBuffs]; // Trigger reactivity
    }
  }

  async function spawnMonster() {
    if (!selectedMonster || isSpawning) return;

    isSpawning = true;
    statusMessage = "Spawning monster...";

    try {
      // First spawn the monster
      await api.sendCommand({
        Type: "spawnmonster",
        Data: {
          monsterName: selectedMonster.value,
          count: spawnCount,
          distance: spawnDistance,
          team: selectedTeam,
        },
      });

      // If we have items or buffs configured, apply them to all monsters
      if (spawnItems.length > 0 || spawnBuffs.length > 0) {
        statusMessage = "Spawning monster and applying configuration...";

        // Wait a moment for monsters to spawn
        await new Promise((resolve) => setTimeout(resolve, 500));

        // Give items to monsters
        for (const item of spawnItems) {
          try {
            await api.sendCommand({
              Type: "givemonsteritem",
              Data: {
                itemName: item.value,
                count: item.count,
              },
            });
          } catch (error) {
            console.warn(`Failed to give item ${item.label}:`, error);
          }
        }

        // Give buffs to monsters
        for (const buff of spawnBuffs) {
          try {
            await api.sendCommand({
              Type: "givemonsterbuff",
              Data: {
                buffName: buff.value,
                stacks: buff.stacks,
                duration: buff.duration,
              },
            });
          } catch (error) {
            console.warn(`Failed to give buff ${buff.label}:`, error);
          }
        }
      }

      const configText =
        spawnItems.length > 0 || spawnBuffs.length > 0
          ? ` with ${spawnItems.length} items and ${spawnBuffs.length} buffs`
          : "";
      statusMessage = `Spawned ${spawnCount}x ${selectedMonster.label}${configText}!`;
      setTimeout(() => (statusMessage = ""), 3000);
    } catch (error) {
      console.error("Failed to spawn monster:", error);
      statusMessage = "Failed to spawn monster";
      setTimeout(() => (statusMessage = ""), 3000);
    } finally {
      isSpawning = false;
    }
  }

  // Load available items on mount
  onMount(async () => {
    await loadItemCatalog();
  });

  // Load item catalog from the game
  async function loadItemCatalog() {
    try {
      let catalog = await api.getItemCatalog();

      if (!catalog || catalog.length === 0) {
        await api.refreshItemCatalog();
        await new Promise((resolve) => setTimeout(resolve, 500));
        catalog = await api.getItemCatalog();
      }

      if (catalog && catalog.length > 0) {
        // Filter to useful items for monsters and group by category
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

        availableItems = Object.entries(groupedItems).map(
          ([category, items]) => ({
            category,
            items,
          }),
        );
      } else {
        // Fallback to basic items
        availableItems = [
          {
            category: "Common Items",
            items: [
              {
                value: "Syringe",
                label: "Soldier's Syringe",
                description: "Attack speed",
              },
              {
                value: "Bear",
                label: "Tougher Times",
                description: "Block chance",
              },
              {
                value: "Crowbar",
                label: "Crowbar",
                description: "Damage to full health enemies",
              },
              {
                value: "Lens",
                label: "Lens-Maker's Glasses",
                description: "Critical chance",
              },
              {
                value: "Hoof",
                label: "Paul's Goat Hoof",
                description: "Movement speed",
              },
            ],
          },
          {
            category: "Uncommon Items",
            items: [
              {
                value: "Whip",
                label: "Elemental Band",
                description: "Elemental damage",
              },
              {
                value: "Bandolier",
                label: "Bandolier",
                description: "Ammo and health on kill",
              },
              {
                value: "ATG",
                label: "AtG Missile Mk. 1",
                description: "Missile on hit",
              },
            ],
          },
        ];
      }
    } catch (error) {
      console.error("Failed to load item catalog:", error);
    }
  }

  // Give item to all monsters
  async function giveItemToMonsters() {
    if (!selectedItem || isGivingItem) return;

    isGivingItem = true;
    managementStatusMessage = "Giving items to monsters...";

    try {
      await api.sendCommand({
        Type: "givemonsteritem",
        Data: {
          itemName: selectedItem,
          count: itemCount,
        },
      });

      const itemLabel =
        availableItems
          .flatMap((cat) => cat.items)
          .find((item) => item.value === selectedItem)?.label || selectedItem;

      managementStatusMessage = `Gave ${itemCount}x ${itemLabel} to all monsters!`;
      setTimeout(() => (managementStatusMessage = ""), 3000);
    } catch (error) {
      console.error("Failed to give item to monsters:", error);
      managementStatusMessage = "Failed to give item to monsters";
      setTimeout(() => (managementStatusMessage = ""), 3000);
    } finally {
      isGivingItem = false;
    }
  }

  // Give buff to all monsters
  async function giveBuffToMonsters() {
    if (!selectedBuff || isGivingBuff) return;

    isGivingBuff = true;
    managementStatusMessage = "Giving buffs to monsters...";

    try {
      await api.sendCommand({
        Type: "givemonsterbuff",
        Data: {
          buffName: selectedBuff,
          stacks: buffStacks,
          duration: buffDuration,
        },
      });

      const buffLabel =
        availableBuffs.find((buff) => buff.value === selectedBuff)?.label ||
        selectedBuff;
      const durationText =
        buffDuration > 0 ? ` for ${buffDuration}s` : " (permanent)";

      managementStatusMessage = `Gave ${buffStacks}x ${buffLabel}${durationText} to all monsters!`;
      setTimeout(() => (managementStatusMessage = ""), 3000);
    } catch (error) {
      console.error("Failed to give buff to monsters:", error);
      managementStatusMessage = "Failed to give buff to monsters";
      setTimeout(() => (managementStatusMessage = ""), 3000);
    } finally {
      isGivingBuff = false;
    }
  }
</script>

<div class="container mx-auto p-6 space-y-6">
  <div class="text-center mb-8">
    <h1 class="text-3xl font-bold mb-2">👹 Monster Management</h1>
    <p class="text-base-content/70">
      Spawn new monsters and manage existing ones with items and buffs
    </p>
  </div>

  <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
    <!-- Monster Selection Component -->
    <MonsterSelection bind:monsterSearch bind:selectedMonster {monsters} />

    <!-- Monster Spawn Settings Component -->
    <MonsterSpawnSettings
      {selectedMonster}
      bind:spawnCount
      bind:spawnDistance
      bind:selectedTeam
      {availableTeams}
      {isSpawning}
      {statusMessage}
      onSpawnMonster={spawnMonster}
      bind:spawnItemFilter
      bind:spawnBuffFilter
      bind:spawnItems
      bind:spawnBuffs
      {availableItems}
      {availableBuffs}
    />
  </div>

  <!-- Monster Management Component -->
  <MonsterManagement
    bind:selectedItem
    bind:itemCount
    bind:selectedBuff
    bind:buffStacks
    bind:buffDuration
    {isGivingItem}
    {isGivingBuff}
    {managementStatusMessage}
    {availableItems}
    {availableBuffs}
    onGiveItemToMonsters={giveItemToMonsters}
    onGiveBuffToMonsters={giveBuffToMonsters}
  />
</div>
