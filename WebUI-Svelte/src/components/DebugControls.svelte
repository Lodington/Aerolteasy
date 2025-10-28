<script>
  import { api } from '../lib/api.js';

  import Button from './Button.svelte';
  
  let verboseLogging = false;
  let isUpdating = false;

  async function toggleVerboseLogging() {
    if (isUpdating) return;
    
    isUpdating = true;
    const newValue = !verboseLogging;
    
    try {
      await api.sendCommand({
        Type: 'setverboselogging',
        Data: { enabled: newValue }
      });
      verboseLogging = newValue;
    } catch (error) {
      console.error('Failed to toggle verbose logging:', error);
    } finally {
      isUpdating = false;
    }
  }

  async function refreshGameState() {
    try {
      await api.sendCommand({
        Type: 'refreshstate',
        Data: {}
      });
    } catch (error) {
      console.error('Failed to refresh game state:', error);
    }
  }

  async function debugAllItems() {
    try {
      await api.sendCommand({
        Type: 'debugitems',
        Data: {}
      });
    } catch (error) {
      console.error('Failed to debug items:', error);
    }
  }

  async function debugAllInteractables() {
    try {
      await api.sendCommand({
        Type: 'debuginteractables',
        Data: {}
      });
    } catch (error) {
      console.error('Failed to debug interactables:', error);
    }
  }

  async function debugCharacterIcons() {
    try {
      await api.sendCommand({
        Type: 'debugcharactericons',
        Data: {}
      });
    } catch (error) {
      console.error('Failed to debug character icons:', error);
    }
  }

  async function debugMonsters() {
    try {
      await api.sendCommand({
        Type: 'debugmonsters',
        Data: {}
      });
    } catch (error) {
      console.error('Failed to debug monsters:', error);
    }
  }

  async function debugInteractableCards() {
    try {
      await api.sendCommand({
        Type: 'debuginteractablecards',
        Data: {}
      });
    } catch (error) {
      console.error('Failed to debug interactable cards:', error);
    }
  }

  async function generateMockData() {
    try {
      await api.sendCommand({
        Type: 'mockgamestate',
        Data: {}
      });
      
      // Wait a moment for the command to process, then refresh to get the mock data
      setTimeout(async () => {
        try {
          const state = await api.getGameState();
          if (state && state.Players && state.Players.length > 0) {
            // The overlay should automatically hide due to the reactive statement in App.svelte
            console.log('Mock data loaded via UI button');
          }
        } catch (err) {
          console.error('Failed to refresh after mock data generation:', err);
        }
      }, 100);
      
    } catch (error) {
      console.error('Failed to generate mock data:', error);
    }
  }

  async function clearMockData() {
    try {
      await api.sendCommand({
        Type: 'clearmockdata',
        Data: {}
      });
    } catch (error) {
      console.error('Failed to clear mock data:', error);
    }
  }
</script>

<div class="space-y-6">
  <!-- Settings -->
  <div class="card bg-base-100 shadow-md">
    <div class="card-body">
      <h3 class="card-title text-lg">Debug Settings</h3>
      <div class="form-control">
        <label class="label cursor-pointer">
          <span class="label-text">Verbose Logging</span>
          <input 
            type="checkbox" 
            class="toggle toggle-primary"
            bind:checked={verboseLogging}
            on:change={toggleVerboseLogging}
            disabled={isUpdating}
          />
        </label>
        <div class="label">
          <span class="label-text-alt">Enable detailed logging for debugging (may impact performance)</span>
        </div>
      </div>
    </div>
  </div>

  <!-- Mock Data Controls -->
  <div class="card bg-base-100 shadow-md">
    <div class="card-body">
      <h3 class="card-title text-lg">🧪 UI Testing</h3>
      <p class="text-sm opacity-70 mb-4">Generate mock data to test the UI without being in a game</p>
      <div class="flex gap-2">
        <Button variant="success" on:click={generateMockData}>Generate Mock Data</Button>
        <Button variant="warning" on:click={clearMockData}>Clear Mock Data</Button>
      </div>
    </div>
  </div>

  <!-- Debug Commands -->
  <div class="card bg-base-100 shadow-md">
    <div class="card-body">
      <h3 class="card-title text-lg">🔍 Debug Commands</h3>
      <p class="text-sm opacity-70 mb-4">Log various game data to console for debugging</p>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-2">
        <Button variant="secondary" size="sm" on:click={refreshGameState}>Refresh Game State</Button>
        <Button variant="secondary" size="sm" on:click={debugAllItems}>Log All Items</Button>
        <Button variant="secondary" size="sm" on:click={debugAllInteractables}>Log All Interactables</Button>
        <Button variant="secondary" size="sm" on:click={debugCharacterIcons}>Log Character Icons</Button>
        <Button variant="secondary" size="sm" on:click={debugMonsters}>Log All Monsters</Button>
        <Button variant="secondary" size="sm" on:click={debugInteractableCards}>Log Interactable Cards</Button>
      </div>
    </div>
  </div>
</div>

<!-- DaisyUI handles all the styling -->