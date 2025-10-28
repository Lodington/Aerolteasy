<script>
  import { api } from '../lib/api.js';
  import Button from './Button.svelte';
  import SearchableSelect from './SearchableSelect.svelte';
  
  let selectedInteractable = '';
  let spawnDistance = 5;
  let isSpawning = false;
  let statusMessage = '';

  // Common RoR2 interactables
  const interactables = [
    { value: 'Chest1', label: 'Small Chest', category: 'Chests' },
    { value: 'Chest2', label: 'Large Chest', category: 'Chests' },
    { value: 'GoldChest', label: 'Legendary Chest', category: 'Chests' },
    { value: 'EquipmentBarrel', label: 'Equipment Barrel', category: 'Chests' },
    { value: 'Barrel1', label: 'Barrel', category: 'Containers' },
    { value: 'Pot', label: 'Pot', category: 'Containers' },
    { value: 'Lockbox', label: 'Lockbox', category: 'Containers' },
    { value: 'Scrapper', label: 'Scrapper', category: 'Machines' },
    { value: 'Duplicator', label: '3D Printer', category: 'Machines' },
    { value: 'DuplicatorLarge', label: 'Large 3D Printer', category: 'Machines' },
    { value: 'DuplicatorMilitary', label: 'Equipment 3D Printer', category: 'Machines' },
    { value: 'ShrineBoss', label: 'Shrine of the Mountain', category: 'Shrines' },
    { value: 'ShrineChance', label: 'Shrine of Chance', category: 'Shrines' },
    { value: 'ShrineCombat', label: 'Shrine of Combat', category: 'Shrines' },
    { value: 'ShrineHealing', label: 'Shrine of the Woods', category: 'Shrines' },
    { value: 'ShrineRestack', label: 'Shrine of Order', category: 'Shrines' },
    { value: 'ShrineCleanse', label: 'Cleansing Pool', category: 'Shrines' },
    { value: 'MultiShop', label: 'Multishop Terminal', category: 'Shops' },
    { value: 'TripleShop', label: 'Triple Shop', category: 'Shops' },
    { value: 'TripleShopLarge', label: 'Large Triple Shop', category: 'Shops' },
    { value: 'TripleShopEquipment', label: 'Equipment Triple Shop', category: 'Shops' }
  ];

  async function spawnInteractable() {
    if (!selectedInteractable || isSpawning) return;
    
    isSpawning = true;
    statusMessage = 'Spawning interactable...';
    
    try {
      await api.sendCommand({
        Type: 'spawninteractable',
        Data: { 
          interactableName: selectedInteractable,
          distance: spawnDistance
        }
      });
      statusMessage = `Spawned ${selectedInteractable}!`;
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } catch (error) {
      console.error('Failed to spawn interactable:', error);
      statusMessage = 'Failed to spawn interactable. Check console for details.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } finally {
      isSpawning = false;
    }
  }

  function handleInteractableChange(event) {
    selectedInteractable = event.detail;
  }
</script>

<div class="interactable-spawning">
  <div class="form-group">
    <label for="interactable-select">Select Interactable</label>
    <div id="interactable-select">
      <SearchableSelect 
        items={interactables}
        bind:selectedValue={selectedInteractable}
        on:change={handleInteractableChange}
        placeholder="Search for an interactable..."
      />
    </div>
  </div>

  <div class="form-group">
    <label for="spawn-distance">Distance</label>
    <input 
      id="spawn-distance"
      type="number" 
      bind:value={spawnDistance} 
      min="1" 
      max="50"
      class="number-input"
    />
  </div>

  <Button on:click={spawnInteractable} disabled={isSpawning || !selectedInteractable}>
    {isSpawning ? 'Spawning...' : 'Spawn Interactable'}
  </Button>

  {#if statusMessage}
    <div class="status-message" class:success={statusMessage.includes('Spawned')} class:error={statusMessage.includes('Failed')}>
      {statusMessage}
    </div>
  {/if}

  <div class="info-section">
    <h4>Tips:</h4>
    <ul>
      <li>Distance determines how far from your character the object spawns</li>
      <li>Some interactables may require specific conditions to function properly</li>
      <li>Shrines and machines will be fully functional when spawned</li>
    </ul>
  </div>
</div>

<style>
  .interactable-spawning {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  label {
    font-weight: bold;
    color: #ffd700;
    font-size: 14px;
  }

  .number-input {
    padding: 12px;
    border: none;
    border-radius: 8px;
    background: rgba(255, 255, 255, 0.1);
    color: white;
    font-size: 14px;
  }

  .number-input:focus {
    outline: 2px solid #ffd700;
    outline-offset: 2px;
    background: rgba(255, 255, 255, 0.15);
  }

  .status-message {
    padding: 8px 12px;
    border-radius: 6px;
    font-size: 14px;
    text-align: center;
    animation: fadeIn 0.3s ease-out;
  }

  .status-message.success {
    background: rgba(34, 197, 94, 0.2);
    color: #22c55e;
    border: 1px solid rgba(34, 197, 94, 0.3);
  }

  .status-message.error {
    background: rgba(239, 68, 68, 0.2);
    color: #ef4444;
    border: 1px solid rgba(239, 68, 68, 0.3);
  }

  .info-section {
    background: rgba(255, 255, 255, 0.05);
    padding: 15px;
    border-radius: 8px;
    border-left: 3px solid #ffd700;
  }

  .info-section h4 {
    margin: 0 0 10px 0;
    color: #ffd700;
    font-size: 14px;
  }

  .info-section ul {
    margin: 0;
    padding-left: 20px;
    color: rgba(255, 255, 255, 0.8);
  }

  .info-section li {
    font-size: 13px;
    margin-bottom: 5px;
  }

  @keyframes fadeIn {
    from { opacity: 0; transform: translateY(-10px); }
    to { opacity: 1; transform: translateY(0); }
  }
</style>