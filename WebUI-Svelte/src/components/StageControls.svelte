<script>
  import { api } from '../lib/api.js';
  import Panel from './Panel.svelte';
  import Button from './Button.svelte';
  import SearchableStageSelect from './SearchableStageSelect.svelte';
  
  let selectedStage = 'golemplains';
  let isChangingStage = false;
  let statusMessage = '';

  async function changeStage() {
    if (!selectedStage || isChangingStage) return;
    
    isChangingStage = true;
    statusMessage = 'Changing stage...';
    
    try {
      await api.sendCommand({
        Type: 'changestage',
        Data: { stageName: selectedStage }
      });
      statusMessage = 'Stage change initiated! Check the game.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } catch (error) {
      console.error('Failed to change stage:', error);
      statusMessage = 'Failed to change stage. Check console for details.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } finally {
      isChangingStage = false;
    }
  }

  function handleStageChange(event) {
    selectedStage = event.detail;
  }
</script>

<Panel title="Stage Controls">
  <div class="stage-controls">
    <label for="stage-select">Select Stage</label>
    <div id="stage-select">
      <SearchableStageSelect 
        bind:selectedStage 
        on:change={handleStageChange}
        placeholder="Search for a stage..."
      />
    </div>
    
    <Button on:click={changeStage} disabled={isChangingStage || !selectedStage}>
      {isChangingStage ? 'Changing...' : 'Change Stage'}
    </Button>
    
    {#if statusMessage}
      <div class="status-message" class:success={statusMessage.includes('initiated')} class:error={statusMessage.includes('Failed')}>
        {statusMessage}
      </div>
    {/if}
  </div>
</Panel>

<style>
  .stage-controls {
    display: flex;
    flex-direction: column;
    gap: 15px;
  }

  label {
    font-weight: bold;
    color: #ffd700;
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