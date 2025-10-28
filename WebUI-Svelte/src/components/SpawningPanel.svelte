<script>
  import TabbedPanel from './TabbedPanel.svelte';
  import MonsterSpawning from './MonsterSpawning.svelte';
  import InteractableSpawning from './InteractableSpawning.svelte';
  import TeleporterControls from './TeleporterControls.svelte';
  
  let activeTab = 0;
  
  const tabs = [
    { label: 'Monsters' },
    { label: 'Interactables' },
    { label: 'Teleporter' }
  ];
</script>

<div class="spawning-panel">
  <h3 class="panel-title">Spawning & Controls</h3>
  
  <TabbedPanel {tabs} bind:activeTab>
    <div slot="tab-0">
      <MonsterSpawning />
    </div>
    
    <div slot="tab-1">
      <InteractableSpawning />
    </div>
    
    <div slot="tab-2">
      <TeleporterControls />
    </div>
  </TabbedPanel>
</div>

<style>
  .spawning-panel {
    background: rgba(255, 255, 255, 0.05);
    border-radius: 12px;
    padding: 20px;
    border: 1px solid rgba(255, 255, 255, 0.1);
  }

  .panel-title {
    margin: 0 0 20px 0;
    color: #ffd700;
    font-size: 18px;
    font-weight: bold;
    text-align: center;
  }
</style>