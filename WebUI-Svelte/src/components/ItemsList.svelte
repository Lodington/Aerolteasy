<script>
  import { selectedPlayer, getItemDisplayName } from '../lib/stores.js';
  import Panel from './Panel.svelte';
  import { Package } from 'lucide-svelte';
  
  $: itemEntries = Object.entries($selectedPlayer?.Items || {});
  $: hasItems = itemEntries.length > 0;
  $: totalItems = itemEntries.reduce((sum, [, count]) => sum + count, 0);
</script>

<Panel title="Player Items">
  {#if $selectedPlayer}
    <div class="items-header">
      <h3>{$selectedPlayer.PlayerName}'s Items</h3>
      <span class="total-items">
        <Package size={16} />
        {totalItems} Total
      </span>
    </div>
    
    <div class="items-list">
      {#if hasItems}
        {#each itemEntries as [itemName, count]}
          <div class="item-entry">
            <span class="item-name">{getItemDisplayName(itemName)}</span>
            <span class="item-count">{count}</span>
          </div>
        {/each}
      {:else}
        <p class="no-items">No items found</p>
      {/if}
    </div>
  {:else}
    <div class="no-selection">
      <Package size={48} />
      <p>Select a player to view their items</p>
    </div>
  {/if}
</Panel>

<style>
  .items-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 15px;
    padding-bottom: 10px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  .items-header h3 {
    margin: 0;
    font-size: 1em;
    color: #ffffff;
  }

  .total-items {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 0.9em;
    color: #ffd700;
  }

  .items-list {
    max-height: 300px;
    overflow-y: auto;
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .item-entry {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px;
    background: rgba(255, 255, 255, 0.05);
    border-radius: 8px;
    border-left: 4px solid #ffd700;
    transition: background 0.3s ease;
  }

  .item-entry:hover {
    background: rgba(255, 255, 255, 0.08);
  }

  .item-name {
    font-weight: bold;
  }

  .item-count {
    background: rgba(255, 215, 0, 0.2);
    padding: 4px 8px;
    border-radius: 12px;
    font-size: 0.9em;
    min-width: 24px;
    text-align: center;
    font-weight: bold;
  }

  .no-items {
    text-align: center;
    color: rgba(255, 255, 255, 0.6);
    font-style: italic;
    padding: 20px;
  }

  .no-selection {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 40px 20px;
    color: rgba(255, 255, 255, 0.6);
    text-align: center;
  }

  .no-selection p {
    margin: 12px 0;
    font-size: 1.1em;
  }
</style>