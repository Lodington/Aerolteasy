<script>
  import { selectedPlayer } from '../../lib/stores.js';
  
  export let characterDefaults = [];
  export let selectedCharacter = 'CommandoBody';
  
  // Get default stats for a character
  function getCharacterDefaults(characterName) {
    return characterDefaults.find(char => char.CharacterName === characterName) || null;
  }

  // Debug logging
  $: console.log('CharacterInfo - selectedCharacter:', selectedCharacter);
  $: console.log('CharacterInfo - characterDefaults:', characterDefaults);
  $: console.log('CharacterInfo - found defaults:', getCharacterDefaults(selectedCharacter));
</script>

<!-- Always show something for debugging -->
<div class="card bg-base-100 shadow-lg mb-4">
  <div class="card-body">
    <h3 class="card-title">📊 Character Information</h3>
    
    {#if getCharacterDefaults(selectedCharacter)}
      <div class="alert alert-info">
        <div>
          <h4 class="font-bold">{getCharacterDefaults(selectedCharacter).DisplayName}</h4>
          <p class="text-sm opacity-70">Base stats for level 1 character</p>
        </div>
      </div>
      
      <div class="stats stats-vertical lg:stats-horizontal shadow">
        <div class="stat">
          <div class="stat-title">Damage</div>
          <div class="stat-value text-primary">{getCharacterDefaults(selectedCharacter).BaseDamage?.toFixed(1)}</div>
        </div>
        <div class="stat">
          <div class="stat-title">Health</div>
          <div class="stat-value text-secondary">{getCharacterDefaults(selectedCharacter).BaseMaxHealth?.toFixed(0)}</div>
        </div>
        <div class="stat">
          <div class="stat-title">Speed</div>
          <div class="stat-value text-accent">{getCharacterDefaults(selectedCharacter).BaseMoveSpeed?.toFixed(1)}</div>
        </div>
        <div class="stat">
          <div class="stat-title">Armor</div>
          <div class="stat-value text-info">{getCharacterDefaults(selectedCharacter).BaseArmor?.toFixed(1)}</div>
        </div>
      </div>
    {:else}
      <div class="alert alert-warning">
        <div>
          <h4 class="font-bold">No Character Defaults Available</h4>
          <p class="text-sm">Character defaults: {characterDefaults.length} loaded</p>
          <p class="text-sm">Selected character: {selectedCharacter}</p>
          <button class="btn btn-sm btn-outline mt-2" on:click={() => console.log('All defaults:', characterDefaults)}>
            Debug: Log All Defaults
          </button>
        </div>
      </div>
    {/if}
  </div>
</div>

<!-- DaisyUI handles all styling -->