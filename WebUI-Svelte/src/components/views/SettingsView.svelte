<script>
  import DebugControls from '../DebugControls.svelte';
</script>

<section class="settings-view">
  <header class="view-header">
    <h1>⚙️ Settings & Debug</h1>
    <p>Configure debug options and development tools</p>
  </header>
  
  <div class="grid-auto">
    <article>
      <header>
        <h3>🔧 Debug Controls</h3>
      </header>
      <DebugControls />
    </article>
    
    <article>
      <header>
        <h3>🛠️ Development Settings</h3>
      </header>
      
      <div class="settings-section">
        <fieldset>
          <label>
            <input type="checkbox" role="switch" />
            Development Mode
          </label>
          <small>Enable additional debugging features and verbose logging</small>
        </fieldset>
        
        <fieldset>
          <label>
            <input type="checkbox" role="switch" checked />
            Auto-refresh Game State
          </label>
          <small>Automatically refresh game state every 500ms</small>
        </fieldset>
        
        <fieldset>
          <label>
            <input type="checkbox" role="switch" />
            Show Performance Metrics
          </label>
          <small>Display API response times and performance data</small>
        </fieldset>
      </div>
    </article>
  </div>
</section>

<style>
/* Settings View using Pico CSS */
.settings-view {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.view-header {
  text-align: center;
  margin-bottom: 1rem;
}

.view-header h1 {
  color: var(--pico-primary);
  margin: 0 0 0.5rem 0;
}

.view-header p {
  color: var(--pico-muted-color);
  margin: 0;
}

.settings-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.settings-section fieldset {
  margin: 0;
  padding: 1rem;
  background: var(--pico-form-element-background-color);
  border: 1px solid var(--pico-muted-border-color);
  border-radius: var(--pico-border-radius);
}

.settings-section fieldset label {
  font-weight: 600;
  color: var(--pico-color);
}

.settings-section fieldset small {
  color: var(--pico-muted-color);
  font-size: 0.8125rem;
  margin-top: 0.25rem;
  display: block;
}

</style>