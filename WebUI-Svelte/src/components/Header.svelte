<script>
  import { isConnected } from '../lib/stores.js';
  import { Wifi, WifiOff } from 'lucide-svelte';
</script>

<header>
  <h1>🌧️ Risk of Rain 2 Dev Tool</h1>
  <div class="connection-status">
    <span class="status-indicator" class:connected={$isConnected}>
      {#if $isConnected}
        <Wifi size={16} />
      {:else}
        <WifiOff size={16} />
      {/if}
    </span>
    <span class="status-text">
      {$isConnected ? 'Connected' : 'Disconnected'}
    </span>
  </div>
  <p class="info-text">Direct API connection to mod (Port 8080)</p>
</header>

<style>
  header {
    text-align: center;
    margin-bottom: 30px;
    padding: 24px;
    background: var(--bg-secondary);
    border-radius: 12px;
    border: 1px solid var(--border-primary);
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.3);
  }

  h1 {
    font-size: 2.5em;
    margin-bottom: 10px;
    color: var(--text-primary);
    font-weight: 700;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
  }

  .connection-status {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 10px;
    font-size: 1.1em;
    margin-bottom: 10px;
  }

  .status-indicator {
    display: flex;
    align-items: center;
    color: var(--danger);
    animation: pulse 2s infinite;
  }

  .status-indicator.connected {
    color: var(--success);
    animation: none;
  }

  .status-text {
    font-weight: 500;
    color: var(--text-primary);
  }

  .info-text {
    font-size: 0.9em;
    color: var(--text-secondary);
    font-style: italic;
  }

  @keyframes pulse {
    0% { opacity: 1; }
    50% { opacity: 0.5; }
    100% { opacity: 1; }
  }

  @media (max-width: 768px) {
    h1 {
      font-size: 2em;
    }
  }
</style>