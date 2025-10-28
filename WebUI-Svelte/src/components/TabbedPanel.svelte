<script>
  export let tabs = [];
  export let activeTab = 0;

  function setActiveTab(index) {
    activeTab = index;
  }
</script>

<div class="tabbed-panel">
  <div class="tab-header">
    {#each tabs as tab, index}
      <button 
        class="tab-button" 
        class:active={activeTab === index}
        on:click={() => setActiveTab(index)}
      >
        {tab.label}
      </button>
    {/each}
  </div>
  
  <div class="tab-content">
    {#if activeTab === 0}
      <slot name="tab-0" />
    {:else if activeTab === 1}
      <slot name="tab-1" />
    {:else if activeTab === 2}
      <slot name="tab-2" />
    {:else if activeTab === 3}
      <slot name="tab-3" />
    {:else if activeTab === 4}
      <slot name="tab-4" />
    {/if}
  </div>
</div>

<style>
  .tabbed-panel {
    background: rgba(255, 255, 255, 0.05);
    border-radius: 12px;
    overflow: hidden;
    border: 1px solid rgba(255, 255, 255, 0.1);
  }

  .tab-header {
    display: flex;
    background: rgba(0, 0, 0, 0.2);
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  .tab-button {
    flex: 1;
    padding: 12px 16px;
    border: none;
    background: transparent;
    color: rgba(255, 255, 255, 0.7);
    cursor: pointer;
    transition: all 0.2s ease;
    font-weight: 500;
    border-right: 1px solid rgba(255, 255, 255, 0.1);
  }

  .tab-button:last-child {
    border-right: none;
  }

  .tab-button:hover {
    background: rgba(255, 255, 255, 0.05);
    color: rgba(255, 255, 255, 0.9);
  }

  .tab-button.active {
    background: rgba(255, 215, 0, 0.1);
    color: #ffd700;
    border-bottom: 2px solid #ffd700;
  }

  .tab-content {
    padding: 20px;
  }
</style>