<script>
  import { selectedPlayer } from '../../lib/stores.js';
  import Button from '../Button.svelte';
  import { api } from '../../lib/api.js';
  
  export let selectedCharacter = 'CommandoBody';
  export let statInputs = {};
  export let targetMode = 'selected';
  export let onCharacterChange = () => {};
  export let refreshGameState = () => {};

  // Debug logging
  console.log('BasicControls component loaded');

  const characters = [
    { value: 'CommandoBody', label: 'Commando' },
    { value: 'HuntressBody', label: 'Huntress' },
    { value: 'Bandit2Body', label: 'Bandit' },
    { value: 'ToolbotBody', label: 'MUL-T' },
    { value: 'EngiBody', label: 'Engineer' },
    { value: 'MageBody', label: 'Artificer' },
    { value: 'MercBody', label: 'Mercenary' },
    { value: 'TreebotBody', label: 'REX' },
    { value: 'LoaderBody', label: 'Loader' },
    { value: 'CrocoBody', label: 'Acrid' },
    { value: 'CaptainBody', label: 'Captain' }
  ];

  function getTargetPlayerId() {
    return targetMode === 'all' ? -1 : ($selectedPlayer?.PlayerId ?? -1);
  }

  async function changeCharacter() {
    try {
      await api.changeCharacter(selectedCharacter, getTargetPlayerId());
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to change character:', error);
    }
  }

  async function setLevel() {
    if (!statInputs.level) return;
    
    try {
      await api.setLevel(parseInt(statInputs.level), getTargetPlayerId());
      statInputs.level = '';
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to set level:', error);
    }
  }

  async function setHealth() {
    if (!statInputs.healthPercent) return;
    
    try {
      await api.setHealth(parseFloat(statInputs.healthPercent), getTargetPlayerId());
      statInputs.healthPercent = '';
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to set health:', error);
    }
  }

  async function setMoney() {
    if (!statInputs.money) return;
    
    try {
      await api.setMoney(parseInt(statInputs.money));
      statInputs.money = '';
    } catch (error) {
      console.error('Failed to set money:', error);
    }
  }

  function handleCharacterChange() {
    onCharacterChange();
  }
</script>

<!-- Basic Controls Component -->
<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">🎮 Basic Controls</h3>
    
    {#if $selectedPlayer}
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <!-- Character Selection -->
        <div class="form-control">
          <label class="label pb-1">
            <span class="label-text font-semibold">Character</span>
          </label>
          <div class="join w-full">
            <select class="select select-bordered join-item flex-1 min-w-0" bind:value={selectedCharacter} on:change={handleCharacterChange}>
              {#each characters as character}
                <option value={character.value}>{character.label}</option>
              {/each}
            </select>
            <button class="btn btn-primary join-item" on:click={changeCharacter}>
              Change
            </button>
          </div>
          {#if $selectedPlayer?.CurrentCharacter}
            <label class="label pt-1">
              <span class="label-text-alt opacity-70">Current: {characters.find(c => c.value === $selectedPlayer.CurrentCharacter)?.label || 'Unknown'}</span>
            </label>
          {/if}
        </div>

        <!-- Level -->
        <div class="form-control">
          <label class="label pb-1">
            <span class="label-text font-semibold">Level</span>
          </label>
          <div class="join w-full">
            <input 
              type="number" 
              class="input input-bordered join-item flex-1 min-w-0"
              bind:value={statInputs.level}
              placeholder={$selectedPlayer?.Level?.toFixed(0) || "2"} 
              min="1"
            />
            <button class="btn btn-primary join-item" on:click={setLevel}>
              Set
            </button>
          </div>
          {#if $selectedPlayer?.Level}
            <label class="label pt-1">
              <span class="label-text-alt opacity-70">Current: {$selectedPlayer.Level.toFixed(0)}</span>
            </label>
          {/if}
        </div>

        <!-- Health Percentage -->
        <div class="form-control">
          <label class="label pb-1">
            <span class="label-text font-semibold">Health %</span>
          </label>
          <div class="join w-full">
            <input 
              type="number" 
              class="input input-bordered join-item flex-1 min-w-0"
              bind:value={statInputs.healthPercent}
              placeholder={$selectedPlayer?.MaxHealth > 0 ? ((($selectedPlayer.Health || 0) / $selectedPlayer.MaxHealth) * 100).toFixed(0) : "51"} 
              min="1" 
              max="100"
            />
            <button class="btn btn-primary join-item" on:click={setHealth}>
              Set
            </button>
          </div>
          {#if $selectedPlayer?.MaxHealth > 0}
            <label class="label pt-1">
              <span class="label-text-alt opacity-70">Current: {((($selectedPlayer.Health || 0) / $selectedPlayer.MaxHealth) * 100).toFixed(0)}%</span>
            </label>
          {/if}
        </div>

        <!-- Money -->
        <div class="form-control">
          <label class="label pb-1">
            <span class="label-text font-semibold">Team Money</span>
          </label>
          <div class="join w-full">
            <input 
              type="number" 
              class="input input-bordered join-item flex-1 min-w-0"
              bind:value={statInputs.money}
              placeholder="Amount" 
              min="0"
            />
            <button class="btn btn-primary join-item" on:click={setMoney}>
              Set
            </button>
          </div>
          <label class="label pt-1">
            <span class="label-text-alt opacity-70">Affects all players</span>
          </label>
        </div>
      </div>
    {:else}
      <div class="alert alert-warning">
        <div>
          <h4 class="font-bold">No Player Selected</h4>
          <p>Select a player to access basic controls</p>
        </div>
      </div>
    {/if}
  </div>
</div>