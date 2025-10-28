<script>
  import { api } from '../lib/api.js';
  import Button from './Button.svelte';
  
  let isProcessing = false;
  let statusMessage = '';

  async function chargeTeleporter() {
    if (isProcessing) return;
    
    isProcessing = true;
    statusMessage = 'Charging teleporter...';
    
    try {
      await api.sendCommand({
        Type: 'chargeteleporter',
        Data: {}
      });
      statusMessage = 'Teleporter charged!';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } catch (error) {
      console.error('Failed to charge teleporter:', error);
      statusMessage = 'Failed to charge teleporter.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }

  async function activateTeleporter() {
    if (isProcessing) return;
    
    isProcessing = true;
    statusMessage = 'Activating teleporter...';
    
    try {
      await api.sendCommand({
        Type: 'activateteleporter',
        Data: {}
      });
      statusMessage = 'Teleporter activated!';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } catch (error) {
      console.error('Failed to activate teleporter:', error);
      statusMessage = 'Failed to activate teleporter.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }

  async function skipTeleporterEvent() {
    if (isProcessing) return;
    
    isProcessing = true;
    statusMessage = 'Skipping teleporter event...';
    
    try {
      await api.sendCommand({
        Type: 'skipteleporterevent',
        Data: {}
      });
      statusMessage = 'Teleporter event skipped!';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } catch (error) {
      console.error('Failed to skip teleporter event:', error);
      statusMessage = 'Failed to skip teleporter event.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }

  async function spawnTeleporter() {
    if (isProcessing) return;
    
    isProcessing = true;
    statusMessage = 'Spawning teleporter...';
    
    try {
      await api.sendCommand({
        Type: 'spawnteleporter',
        Data: {}
      });
      statusMessage = 'Teleporter spawned!';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } catch (error) {
      console.error('Failed to spawn teleporter:', error);
      statusMessage = 'Failed to spawn teleporter.';
      setTimeout(() => {
        statusMessage = '';
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }
</script>

<div class="teleporter-controls">
  <div class="control-section">
    <h4>Teleporter Actions</h4>
    <div class="button-grid">
      <Button on:click={chargeTeleporter} disabled={isProcessing}>
        Charge Teleporter
      </Button>
      
      <Button on:click={activateTeleporter} disabled={isProcessing}>
        Activate Teleporter
      </Button>
      
      <Button on:click={skipTeleporterEvent} disabled={isProcessing}>
        Skip Event
      </Button>
      
      <Button on:click={spawnTeleporter} disabled={isProcessing}>
        Spawn Teleporter
      </Button>
    </div>
  </div>

  {#if statusMessage}
    <div class="status-message" class:success={statusMessage.includes('!')} class:error={statusMessage.includes('Failed')}>
      {statusMessage}
    </div>
  {/if}

  <div class="info-section">
    <h4>Teleporter Controls:</h4>
    <ul>
      <li><strong>Charge:</strong> Instantly charges the teleporter to 100%</li>
      <li><strong>Activate:</strong> Starts the teleporter charging sequence</li>
      <li><strong>Skip Event:</strong> Skips the current teleporter boss event</li>
      <li><strong>Spawn:</strong> Creates a new teleporter at your location</li>
    </ul>
  </div>

  <div class="warning-section">
    <h4>⚠️ Warning:</h4>
    <p>These controls directly manipulate the teleporter state. Use carefully as they may affect game progression and multiplayer synchronization.</p>
  </div>
</div>

<style>
  .teleporter-controls {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  .control-section h4 {
    margin: 0 0 15px 0;
    color: #ffd700;
    font-size: 16px;
  }

  .button-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 12px;
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

  .info-section, .warning-section {
    background: rgba(255, 255, 255, 0.05);
    padding: 15px;
    border-radius: 8px;
  }

  .info-section {
    border-left: 3px solid #ffd700;
  }

  .warning-section {
    border-left: 3px solid #ef4444;
  }

  .info-section h4, .warning-section h4 {
    margin: 0 0 10px 0;
    font-size: 14px;
  }

  .info-section h4 {
    color: #ffd700;
  }

  .warning-section h4 {
    color: #ef4444;
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

  .warning-section p {
    margin: 0;
    color: rgba(255, 255, 255, 0.8);
    font-size: 13px;
  }

  @keyframes fadeIn {
    from { opacity: 0; transform: translateY(-10px); }
    to { opacity: 1; transform: translateY(0); }
  }
</style>