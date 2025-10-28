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
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Base Damage</span>
        </label>
        <div class="flex flex-wrap gap-2 mb-3">
          {#if $selectedPlayer?.BaseDamage}
            <div class="badge stat-badge-current badge-sm">Current: {$selectedPlayer.BaseDamage.toFixed(1)}</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseDamage}
            <div class="badge stat-badge-default badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseDamage.toFixed(1)}</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 stat-input"
            bind:value={statInputs.baseDamage}
            placeholder="New damage" 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item stat-button" on:click={() => setPlayerStat('damage', 'baseDamage')}>
            Set
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Armor</span>
        </label>
        <div class="flex flex-wrap gap-2 mb-3">
          {#if $selectedPlayer?.Armor}
            <div class="badge stat-badge-current badge-sm">Current: {$selectedPlayer.Armor.toFixed(1)}</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseArmor}
            <div class="badge stat-badge-default badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseArmor.toFixed(1)}</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1 stat-input"
            bind:value={statInputs.armor}
            placeholder="New armor" 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item stat-button" on:click={() => setPlayerStat('armor', 'armor')}>
            Set
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Attack Speed</span>
        </label>
        <div class="flex flex-wrap gap-1 mb-2">
          {#if $selectedPlayer?.AttackSpeed}
            <div class="badge stat-badge-current badge-sm">Current: {$selectedPlayer.AttackSpeed.toFixed(2)}</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseAttackSpeed}
            <div class="badge stat-badge-default badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseAttackSpeed.toFixed(2)}</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1"
            bind:value={statInputs.attackSpeed}
            placeholder="New attack speed" 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('attackspeed', 'attackSpeed')}>
            Set
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Crit Chance %</span>
        </label>
        <div class="flex flex-wrap gap-1 mb-2">
          {#if $selectedPlayer?.CritChance}
            <div class="badge badge-success badge-sm">Current: {$selectedPlayer.CritChance.toFixed(1)}%</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseCrit}
            <div class="badge badge-info badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseCrit.toFixed(1)}%</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1"
            bind:value={statInputs.critChance}
            placeholder="New crit %" 
            min="0"
            max="100"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('critchance', 'critChance')}>
            Set
          </button>
        </div>
      </div>
    </div>
  </div>
</div>

<!-- Movement & Health Stats -->
<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🏃 Movement & Health</h3>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Move Speed</span>
        </label>
        <div class="flex flex-wrap gap-1 mb-2">
          {#if $selectedPlayer?.MoveSpeed}
            <div class="badge badge-success badge-sm">Current: {$selectedPlayer.MoveSpeed.toFixed(1)}</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseMoveSpeed}
            <div class="badge badge-info badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseMoveSpeed.toFixed(1)}</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1"
            bind:value={statInputs.moveSpeed}
            placeholder="New move speed" 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('movespeed', 'moveSpeed')}>
            Set
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Jump Power</span>
        </label>
        <div class="flex flex-wrap gap-1 mb-2">
          {#if $selectedPlayer?.JumpPower}
            <div class="badge badge-success badge-sm">Current: {$selectedPlayer.JumpPower.toFixed(1)}</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseJumpPower}
            <div class="badge badge-info badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseJumpPower.toFixed(1)}</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1"
            bind:value={statInputs.jumpPower}
            placeholder="New jump power" 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('jumppower', 'jumpPower')}>
            Set
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Max Health</span>
        </label>
        <div class="flex flex-wrap gap-1 mb-2">
          {#if $selectedPlayer?.MaxHealth}
            <div class="badge badge-success badge-sm">Current: {$selectedPlayer.MaxHealth.toFixed(0)}</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseMaxHealth}
            <div class="badge badge-info badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseMaxHealth.toFixed(0)}</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1"
            bind:value={statInputs.maxHealth}
            placeholder="New max health" 
            min="1"
            step="1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('maxhealth', 'maxHealth')}>
            Set
          </button>
        </div>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium">Health Regen</span>
        </label>
        <div class="flex flex-wrap gap-1 mb-2">
          {#if $selectedPlayer?.HealthRegen}
            <div class="badge badge-success badge-sm">Current: {$selectedPlayer.HealthRegen.toFixed(2)}</div>
          {/if}
          {#if getCharacterDefaults(selectedCharacter)?.BaseRegen}
            <div class="badge badge-info badge-sm">Default: {getCharacterDefaults(selectedCharacter).BaseRegen.toFixed(2)}</div>
          {/if}
        </div>
        <div class="join">
          <input 
            type="number" 
            class="input input-bordered join-item flex-1"
            bind:value={statInputs.healthRegen}
            placeholder="New health regen" 
            min="0"
            step="0.1"
          />
          <button class="btn btn-primary join-item" on:click={() => setPlayerStat('regen', 'healthRegen')}>
            Set
          </button>
        </div>
      </div>
    </div>
  </div>
</div>

<style>
  /* Enhanced styling for advanced stats */
  .stat-badge-current {
    background: linear-gradient(135deg, hsl(var(--su)), hsl(var(--su) / 0.8));
    color: hsl(var(--suc));
    border: none;
  }

  .stat-badge-default {
    background: linear-gradient(135deg, hsl(var(--in)), hsl(var(--in) / 0.8));
    color: hsl(var(--inc));
    border: none;
  }

  .stat-input {
    transition: all 0.3s ease;
  }

  .stat-input:focus {
    transform: scale(1.02);
    box-shadow: 0 0 0 3px hsl(var(--p) / 0.2);
  }

  .stat-button {
    transition: all 0.2s ease;
  }

  .stat-button:hover {
    transform: translateY(-1px);
    box-shadow: 0 4px 12px hsl(var(--p) / 0.3);
  }
</style>