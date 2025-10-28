<script>
  import { api } from "../../lib/api.js";
  import InteractableSelection from "../interactable/InteractableSelection.svelte";
  import InteractableSpawnSettings from "../interactable/InteractableSpawnSettings.svelte";

  // Interactable search and spawning
  let interactableSearch = '';
  let selectedInteractable = null;
  let spawnDistance = 5;
  let isSpawning = false;
  let statusMessage = '';

  // Available interactables organized by category
  const interactables = [
    { 
      category: 'Chests & Containers', 
      interactables: [
        { value: 'Chest1', label: 'Small Chest', description: 'Contains common items (white)', emoji: '📦', cost: '$25' },
        { value: 'Chest2', label: 'Large Chest', description: 'Contains uncommon items (green)', emoji: '📋', cost: '$50' },
        { value: 'GoldChest', label: 'Legendary Chest', description: 'Contains legendary items (red)', emoji: '🏆', cost: '$400' },
        { value: 'EquipmentBarrel', label: 'Equipment Barrel', description: 'Contains equipment items (orange)', emoji: '🛢️', cost: '$25' },
        { value: 'Barrel1', label: 'Barrel', description: 'Breakable container with random loot', emoji: '🪣', cost: 'Free' },
        { value: 'Pot', label: 'Pot', description: 'Breakable container with small items', emoji: '🏺', cost: 'Free' },
        { value: 'Lockbox', label: 'Lockbox', description: 'Secure container with guaranteed loot', emoji: '🔒', cost: '$30' }
      ]
    },
    { 
      category: 'Machines & Printers', 
      interactables: [
        { value: 'Scrapper', label: 'Scrapper', description: 'Convert items into scrap material', emoji: '♻️', cost: 'Items' },
        { value: 'Duplicator', label: '3D Printer (White)', description: 'Duplicate common items', emoji: '🖨️', cost: 'Items' },
        { value: 'DuplicatorLarge', label: '3D Printer (Green)', description: 'Duplicate uncommon items', emoji: '🖨️', cost: 'Items' },
        { value: 'DuplicatorMilitary', label: '3D Printer (Orange)', description: 'Duplicate equipment items', emoji: '🖨️', cost: 'Items' },
        { value: 'DuplicatorWild', label: 'Cauldron (Red)', description: 'Duplicate legendary items', emoji: '🍯', cost: 'Items' },
        { value: 'FreeChestTerminal', label: 'Shipping Request Form', description: 'Free item delivery terminal', emoji: '📋', cost: 'Free' }
      ]
    },
    { 
      category: 'Shrines', 
      interactables: [
        { value: 'ShrineBoss', label: 'Shrine of the Mountain', description: 'Spawn additional bosses for rewards', emoji: '⛰️', cost: '$50+' },
        { value: 'ShrineChance', label: 'Shrine of Chance', description: 'Gamble money for items', emoji: '🎰', cost: '$50+' },
        { value: 'ShrineCombat', label: 'Shrine of Combat', description: 'Spawn enemies for money', emoji: '⚔️', cost: '$100+' },
        { value: 'ShrineHealing', label: 'Shrine of the Woods', description: 'Heal for money', emoji: '🌿', cost: '$25+' },
        { value: 'ShrineRestack', label: 'Shrine of Order', description: 'Reroll all items of same type', emoji: '🔄', cost: '$100' },
        { value: 'ShrineCleanse', label: 'Cleansing Pool', description: 'Remove debuffs and curses', emoji: '💧', cost: '$25' },
        { value: 'ShrineGoldshoresAccess', label: 'Gold Shrine', description: 'Gilded Coast access shrine', emoji: '🏛️', cost: '$50' }
      ]
    },
    { 
      category: 'Shops & Terminals', 
      interactables: [
        { value: 'MultiShop', label: 'Multishop Terminal', description: 'Choose from multiple item options', emoji: '🛒', cost: '$25-50' },
        { value: 'TripleShop', label: 'Triple Shop (White)', description: 'Three common item choices', emoji: '🏪', cost: '$25' },
        { value: 'TripleShopLarge', label: 'Triple Shop (Green)', description: 'Three uncommon item choices', emoji: '🏪', cost: '$50' },
        { value: 'TripleShopEquipment', label: 'Triple Shop (Orange)', description: 'Three equipment choices', emoji: '🏪', cost: '$25' },
        { value: 'CategoryChestDamage', label: 'Damage Items Chest', description: 'Guaranteed damage items', emoji: '💥', cost: '$50' },
        { value: 'CategoryChestHealing', label: 'Healing Items Chest', description: 'Guaranteed healing items', emoji: '💚', cost: '$50' },
        { value: 'CategoryChestUtility', label: 'Utility Items Chest', description: 'Guaranteed utility items', emoji: '🔧', cost: '$50' }
      ]
    },
    { 
      category: 'Special & Lunar', 
      interactables: [
        { value: 'LunarChest', label: 'Lunar Pod', description: 'Lunar items for lunar coins', emoji: '🌙', cost: '1 Lunar' },
        { value: 'ShrineBoss_Lunar', label: 'Lunar Shrine', description: 'Lunar item gamble', emoji: '🌕', cost: '2 Lunar' },
        { value: 'ShrineRestack_Lunar', label: 'Lunar Reroller', description: 'Reroll items with lunar coins', emoji: '🔮', cost: '1 Lunar' },
        { value: 'NewtStatue', label: 'Newt Altar', description: 'Access to Bazaar Between Time', emoji: '🗿', cost: '1 Lunar' },
        { value: 'VoidChest', label: 'Void Cradle', description: 'Void items that corrupt others', emoji: '🕳️', cost: 'Items' },
        { value: 'VoidTriple', label: 'Void Triple', description: 'Three void item choices', emoji: '⚫', cost: 'Items' }
      ]
    }
  ];



  async function spawnInteractable() {
    if (!selectedInteractable || isSpawning) return;
    
    isSpawning = true;
    statusMessage = 'Spawning interactable...';
    
    try {
      await api.sendCommand({
        Type: 'spawninteractable',
        Data: { 
          interactableName: selectedInteractable.value,
          distance: spawnDistance
        }
      });

      statusMessage = `Spawned ${selectedInteractable.label}!`;
      
      setTimeout(() => {
        statusMessage = '';
      }, 4000);
    } catch (error) {
      console.error('Failed to spawn interactable:', error);
      statusMessage = 'Failed to spawn interactable. Check console for details.';
      setTimeout(() => {
        statusMessage = '';
      }, 4000);
    } finally {
      isSpawning = false;
    }
  }
</script>

<div class="container mx-auto p-4 sm:p-6 space-y-4 sm:space-y-6">
  <div class="text-center mb-6 sm:mb-8">
    <h1 class="text-2xl sm:text-3xl font-bold mb-2">📦 Interactable Management</h1>
    <p class="text-sm sm:text-base text-base-content/70 px-4">
      Search and spawn chests, shrines, machines, and other interactables
    </p>
  </div>

  <div class="flex flex-col xl:grid xl:grid-cols-2 gap-6">
    <!-- Interactable Selection Component -->
    <div class="order-1">
      <InteractableSelection 
        bind:interactableSearch
        bind:selectedInteractable
        {interactables}
      />
    </div>

    <!-- Interactable Spawn Settings Component -->
    <div class="order-2 xl:order-2">
      <InteractableSpawnSettings
        {selectedInteractable}
        bind:spawnDistance
        {isSpawning}
        {statusMessage}
        onSpawnInteractable={spawnInteractable}
      />
    </div>
  </div>
</div>