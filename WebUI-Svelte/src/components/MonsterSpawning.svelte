<script>
  import { api } from '../lib/api.js';
  import Button from './Button.svelte';
  import SearchableSelect from './SearchableSelect.svelte';
  
  let selectedMonster = '';
  let spawnCount = 1;
  let spawnDistance = 10;
  let isSpawning = false;
  let statusMessage = '';

  // Common RoR2 monsters - this could be expanded or fetched from the game
  const monsters = [
    { value: 'Beetle', label: 'Beetle', category: 'Stage 1' },
    { value: 'Wisp', label: 'Wisp', category: 'Stage 1' },
    { value: 'Lemurian', label: 'Lemurian', category: 'Stage 1' },
    { value: 'Golem', label: 'Stone Golem', category: 'Stage 1' },
    { value: 'Jellyfish', label: 'Jellyfish', category: 'Stage 2' },
    { value: 'GreaterWisp', label: 'Greater Wisp', category: 'Stage 2' },
    { value: 'Bison', label: 'Bison', category: 'Stage 2' },
    { value: 'LemurianBruiser', label: 'Elder Lemurian', category: 'Stage 2' },
    { value: 'ClayBoss', label: 'Clay Dunestrider', category: 'Boss' },
    { value: 'Titan', label: 'Stone Titan', category: 'Boss' },
    { value: 'Vagrant', label: 'Wandering Vagrant', category: 'Boss' },
    { value: 'MagmaWorm', label: 'Magma Worm', category: 'Boss' },
    { value: 'ImpBoss', label: 'Imp Overlord', category: 'Boss' },
    { value: 'GravekeeperBoss', label: 'Grovetender', category: 'Boss' }
  ];

  async function spawnMonster() {
    if (!selectedMonster || isSpawning) return;
    
    isSpawning = true;
    statusMessage = 'Spawning monster...';
    
    try {
      await api.sendCommand({
        Type: 'spawnmonster',
        Data: { 
          monsterName: selectedMonster,
          count: spawnCount,
          distance: spawnDistance
        }
      });
      statusMessage = `Spawned ${spawnCount}x ${selectedMonster}!`;
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } catch (error) {
      console.error('Failed to spawn monster:', error);
      statusMessage = 'Failed to spawn monster. Check console for details.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } finally {
      isSpawning = false;
    }
  }

  function handleMonsterChange(event) {
    selectedMonster = event.detail;
  }
</script>

<div class="monster-spawning">
  <div class="form-group">
    <label for="monster-select">Select Monster</label>
    <div id="monster-select">
      <SearchableSelect 
        items={monsters}
        bind:selectedValue={selectedMonster}
        on:change={handleMonsterChange}
        placeholder="Search for a monster..."
      />
    </div>
  </div>

  <div class="form-row">
    <div class="form-group">
      <label for="spawn-count">Count</label>
      <input 
        id="spawn-count"
        type="number" 
        bind:value={spawnCount} 
        min="1" 
        max="50"
        class="number-input"
      />
    </div>

    <div class="form-group">
      <label for="spawn-distance">Distance</label>
      <input 
        id="spawn-distance"
        type="number" 
        bind:value={spawnDistance} 
        min="1" 
        max="100"
        class="number-input"
      />
    </div>
  </div>

  <Button on:click={spawnMonster} disabled={isSpawning || !selectedMonster}>
    {isSpawning ? 'Spawning...' : 'Spawn Monster'}
  </Button>

  {#if statusMessage}
    <div class="status-message" class:success={statusMessage.includes('Spawned')} class:error={statusMessage.includes('Failed')}>
      {statusMessage}
    </div>
  {/if}
</div>

<style>
  .monster-spawning {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .form-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 15px;
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

  @keyframes fadeIn {
    from { opacity: 0; transform: translateY(-10px); }
    to { opacity: 1; transform: translateY(0); }
  }
</style>