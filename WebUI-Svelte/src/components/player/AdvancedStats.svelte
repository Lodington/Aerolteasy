<script>
  import { selectedPlayer } from '../../lib/stores.js';
  import Button from '../Button.svelte';
  import { api } from '../../lib/api.js';
  
  export let statInputs = {};
  export let targetMode = 'selected';
  export let characterDefaults = [];
  export let selectedCharacter = 'CommandoBody';
  export let refreshGameState = () => {};

  // Get default stats for a character
  function getCharacterDefaults(characterName) {
    return characterDefaults.find(char => char.CharacterName === characterName) || null;
  }

  function getTargetPlayerId() {
    return targetMode === 'all' ? -1 : ($selectedPlayer?.PlayerId ?? -1);
  }

  // Advanced stat setting functions
  async function setPlayerStat(statType, inputKey) {
    const value = statInputs[inputKey];
    if (!value) return;
    
    try {
      await api.setPlayerStat(statType, parseFloat(value), getTargetPlayerId());
      // Clear the input after successful set
      statInputs[inputKey] = '';
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error(`Failed to set ${statType}:`, error);
    }
  }
</script>

<!-- Combat Stats -->
<div class="card bg-base-100 shadow-lg mb-6">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">⚔️ Combat Stats</h3>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Base Damage</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.baseDamage}
            placeholder={$selectedPlayer?.BaseDamage?.toFixed(1) || "13.4"} 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('damage', 'baseDamage')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.BaseDamage?.toFixed(1) || "N/A"}
          </span>
        </label>
      </div>

      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Armor</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.armor}
            placeholder={$selectedPlayer?.Armor?.toFixed(1) || "11.0"} 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('armor', 'armor')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.Armor?.toFixed(1) || "N/A"}
          </span>
        </label>
      </div>

      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Attack Speed</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.attackSpeed}
            placeholder={$selectedPlayer?.AttackSpeed?.toFixed(2) || "2.01"} 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('attackspeed', 'attackSpeed')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.AttackSpeed?.toFixed(2) || "N/A"}
          </span>
        </label>
      </div>

      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Crit Chance %</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.critChance}
            placeholder={$selectedPlayer?.CritChance?.toFixed(1) || "19.9"} 
            min="0"
            max="100"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('critchance', 'critChance')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.CritChance?.toFixed(1) || "N/A"}%
          </span>
        </label>
      </div>
    </div>
  </div>
</div>

<!-- Movement & Health Stats -->
<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🏃 Movement & Health</h3>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Move Speed</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.moveSpeed}
            placeholder={$selectedPlayer?.MoveSpeed?.toFixed(1) || "7.0"} 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('movespeed', 'moveSpeed')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.MoveSpeed?.toFixed(1) || "N/A"}
          </span>
        </label>
      </div>

      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Jump Power</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.jumpPower}
            placeholder={$selectedPlayer?.JumpPower?.toFixed(1) || "15.0"} 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('jumppower', 'jumpPower')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.JumpPower?.toFixed(1) || "N/A"}
          </span>
        </label>
      </div>

      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Max Health</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.maxHealth}
            placeholder={$selectedPlayer?.MaxHealth?.toFixed(0) || "110"} 
            min="1"
            step="1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('maxhealth', 'maxHealth')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.MaxHealth?.toFixed(0) || "N/A"}
          </span>
        </label>
      </div>

      <div class="form-control">
        <label class="label pb-1">
          <span class="label-text font-semibold">Health Regen</span>
        </label>
        <div class="join w-full">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 min-w-0"
            bind:value={statInputs.healthRegen}
            placeholder={$selectedPlayer?.HealthRegen?.toFixed(2) || "1.00"} 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('regen', 'healthRegen')}>
            Set
          </button>
        </div>
        <label class="label pt-1">
          <span class="label-text-alt opacity-70">
            Current: {$selectedPlayer?.HealthRegen?.toFixed(2) || "N/A"}
          </span>
        </label>
      </div>
    </div>
  </div>
</div>

