<script>
  import { selectedPlayer, getItemDisplayName } from '../../lib/stores.js';
  import Button from '../Button.svelte';
  import { Package, Zap } from 'lucide-svelte';
  import { api } from '../../lib/api.js';

  export let availableItems = [];
  export let refreshGameState = () => {};

  // Debug logging
  console.log('ItemManagement component loaded');

  // Item management state
  let itemFilter = '';
  let expandedItems = new Set();
  let editingItem = null;
  let editValue = '';

  // Filtered items for display
  $: filteredItems = Object.entries($selectedPlayer?.Items || {}).filter(([itemName, count]) => {
    if (count <= 0) return false;
    if (!itemFilter.trim()) return true;
    return getItemDisplayName(itemName).toLowerCase().includes(itemFilter.toLowerCase());
  });

  // Get suggested items that match search but player doesn't have
  $: suggestedItems = itemFilter.trim() ? 
    availableItems.flatMap(category => 
      category.items.filter(item => {
        const hasItem = $selectedPlayer?.Items?.[item.value] > 0;
        const matchesSearch = item.label.toLowerCase().includes(itemFilter.toLowerCase()) ||
                            item.value.toLowerCase().includes(itemFilter.toLowerCase());
        return !hasItem && matchesSearch;
      })
    ).slice(0, 6) : // Limit to 6 suggestions
    [];

  // Toggle individual item controls
  function toggleItemControls(itemName) {
    if (expandedItems.has(itemName)) {
      expandedItems.delete(itemName);
    } else {
      expandedItems.add(itemName);
    }
    expandedItems = expandedItems; // Trigger reactivity
  }

  // Quick add suggested item
  async function addSuggestedItem(itemValue, count = 1) {
    if (!$selectedPlayer) return;
    
    try {
      await api.spawnItem(itemValue, count, $selectedPlayer.PlayerId);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to add suggested item:', error);
    }
  }

  async function modifyItem(itemName, change) {
    if (!$selectedPlayer) return;
    
    try {
      await api.spawnItem(itemName, change, $selectedPlayer.PlayerId);
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to modify item:', error);
    }
  }

  async function removeItem(itemName) {
    if (!$selectedPlayer) return;
    
    const currentCount = $selectedPlayer.Items[itemName] || 0;
    if (currentCount > 0) {
      try {
        await api.spawnItem(itemName, -currentCount, $selectedPlayer.PlayerId);
        setTimeout(refreshGameState, 50);
      } catch (error) {
        console.error('Failed to remove item:', error);
      }
    }
  }

  function startEditingCount(itemName, currentCount, event) {
    event.stopPropagation();
    editingItem = itemName;
    editValue = currentCount.toString();
    // Focus the input after it renders
    setTimeout(() => {
      const input = document.querySelector('.count-edit-input');
      if (input) {
        input.focus();
        input.select();
      }
    }, 10);
  }

  function cancelEdit() {
    editingItem = null;
    editValue = '';
  }

  async function saveEditedCount(itemName, oldCount) {
    const newCount = parseInt(editValue);
    
    if (isNaN(newCount) || newCount < 0) {
      cancelEdit();
      return;
    }

    const difference = newCount - oldCount;
    
    if (difference !== 0) {
      try {
        await api.spawnItem(itemName, difference, $selectedPlayer.PlayerId);
        setTimeout(refreshGameState, 50);
      } catch (error) {
        console.error('Failed to update item count:', error);
      }
    }
    
    cancelEdit();
  }

  function handleEditKeydown(event, itemName, oldCount) {
    if (event.key === 'Enter') {
      saveEditedCount(itemName, oldCount);
    } else if (event.key === 'Escape') {
      cancelEdit();
    }
  }


</script>

{#if $selectedPlayer}
<div class="space-y-6">
  <!-- Enhanced Search Section -->
  <div class="card bg-gradient-to-r from-primary/10 to-secondary/10 shadow-lg">
    <div class="card-body">
      <h3 class="card-title text-lg mb-4">🔍 Item Search & Management</h3>
      <div class="form-control">
        <div class="join w-full">
          <span class="btn btn-square join-item pointer-events-none">🔍</span>
          <input 
            type="text" 
            bind:value={itemFilter}
            placeholder="Search for items to add..."
            class="input input-bordered join-item flex-1"
          />
          {#if itemFilter}
            <button class="btn btn-square join-item" on:click={() => itemFilter = ''} title="Clear search">
              ×
            </button>
          {/if}
        </div>
        <label class="label">
          <span class="label-text-alt">💡 Tip: Search by item name to find and add new items</span>
        </label>
      </div>
    </div>
  </div>

  <!-- Suggested Items Section - Right below search bar -->
  {#if suggestedItems.length > 0}
    <div class="suggested-items-section">
      <div class="suggested-header">
        <h5>💡 Suggested Items ({suggestedItems.length})</h5>
        <p class="suggested-hint">Items matching "{itemFilter}" that you can add</p>
      </div>
      
      <div class="suggested-items-grid">
        {#each suggestedItems as item}
          <div class="suggested-item-card">
            <div class="suggested-item-info">
              <div class="suggested-item-name" title={item.label}>
                {item.label}
              </div>
              <div class="suggested-item-category">
                {availableItems.find(cat => cat.items.includes(item))?.category || 'Item'}
              </div>
            </div>
            <div class="suggested-item-actions">
              <button class="quick-add-btn" on:click={() => addSuggestedItem(item.value, 1)} title="Add 1">
                +1
              </button>
              <button class="quick-add-btn" on:click={() => addSuggestedItem(item.value, 5)} title="Add 5">
                +5
              </button>
            </div>
          </div>
        {/each}
      </div>
    </div>
  {/if}

  <!-- Current Items Section -->
  {#if $selectedPlayer.Items && Object.keys($selectedPlayer.Items).length > 0}
    <div class="card bg-base-100 shadow-lg">
      <div class="card-body">
        <div class="flex justify-between items-center mb-4">
          <div>
            <h4 class="card-title">📦 Current Items ({filteredItems.length}{itemFilter ? ` of ${Object.keys($selectedPlayer.Items).filter(key => $selectedPlayer.Items[key] > 0).length}` : ''})</h4>
            <p class="text-sm opacity-70">
              Click on any item to show controls
              {#if suggestedItems.length > 0}
                • {suggestedItems.length} suggested items above
              {/if}
            </p>
          </div>
          <button class="btn btn-outline btn-sm" on:click={refreshGameState}>
            <Zap size={14} />
            Refresh
          </button>
        </div>
    


        <!-- Current Items Grid -->
        {#if filteredItems.length > 0}
          <div class="items-friendly-grid">
        {#each filteredItems as [itemName, count]}
          <div class="item-card-friendly" class:expanded={expandedItems.has(itemName)}>
            <div 
              class="item-header" 
              on:click={() => toggleItemControls(itemName)} 
              on:keydown={(e) => e.key === 'Enter' && toggleItemControls(itemName)}
              role="button" 
              tabindex="0"
            >
              <div class="item-name-display" title={getItemDisplayName(itemName)}>
                {getItemDisplayName(itemName)}
              </div>
              {#if editingItem === itemName}
                <div class="count-edit-wrapper" on:click|stopPropagation>
                  <input 
                    type="number" 
                    class="count-edit-input"
                    bind:value={editValue}
                    on:blur={() => saveEditedCount(itemName, count)}
                    on:keydown={(e) => handleEditKeydown(e, itemName, count)}
                    min="0"
                  />
                </div>
              {:else}
                <div 
                  class="item-count-display editable" 
                  on:click={(e) => startEditingCount(itemName, count, e)}
                  title="Click to edit count"
                >
                  ×{count}
                </div>
              {/if}
              <div class="expand-indicator" class:rotated={expandedItems.has(itemName)}>
                ▼
              </div>
            </div>
            
            {#if expandedItems.has(itemName)}
              <div class="item-controls-friendly">
                <div class="quantity-controls">
                  <button class="control-btn decrease" on:click|stopPropagation={() => modifyItem(itemName, -1)} title="Remove 1">
                    <span>−</span>
                  </button>
                  <span class="current-count">{count}</span>
                  <button class="control-btn increase" on:click|stopPropagation={() => modifyItem(itemName, 1)} title="Add 1">
                    <span>+</span>
                  </button>
                </div>
                <button class="control-btn remove-all" on:click|stopPropagation={() => removeItem(itemName)} title="Remove all">
                  Remove All
                </button>
              </div>
            {/if}
            </div>
          {/each}
        </div>
      {:else if Object.keys($selectedPlayer.Items).filter(key => $selectedPlayer.Items[key] > 0).length === 0 && !itemFilter}
        <!-- No items at all -->
        <div class="alert alert-info">
          <div>
            <h4 class="font-bold">No Items Yet</h4>
            <p>This player has no items yet. Use the search above to find and add items!</p>
          </div>
        </div>
      {/if}
      
      {#if filteredItems.length === 0 && itemFilter && suggestedItems.length === 0}
        <div class="alert alert-warning">
          <div>
            <h4 class="font-bold">No Results</h4>
            <p>No items match "{itemFilter}"</p>
            <button class="btn btn-sm btn-outline mt-2" on:click={() => itemFilter = ''}>Clear Filter</button>
          </div>
        </div>
      {/if}
      </div>
    </div>

  {:else}
    <div class="card bg-base-100 shadow-lg">
      <div class="card-body text-center">
        <div class="text-6xl mb-4">📦</div>
        <h3 class="text-xl font-bold mb-2">No Items Found</h3>
        <p class="opacity-70">This player doesn't have any items yet. Use the search above to find and add items!</p>
      </div>
    </div>
  {/if}

</div>
{:else}
  <div class="card bg-base-100 shadow-lg">
    <div class="card-body text-center">
      <div class="text-6xl mb-4">📦</div>
      <h3 class="text-xl font-bold mb-2">No Player Selected</h3>
      <p class="opacity-70">Select a player to manage their items</p>
    </div>
  </div>
{/if}

<style>
  .items-section {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  .items-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 16px;
  }

  .items-title h4 {
    margin: 0 0 0.25rem 0;
    color: var(--pico-color);
    font-size: 1.1em;
  }

  .items-hint {
    margin: 0;
    font-size: 0.75rem;
    color: var(--pico-muted-color);
    font-style: italic;
  }

  .items-actions {
    display: flex;
    gap: 8px;
  }

  /* Enhanced styling for item management */
  .items-friendly-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: 12px;
    max-height: 600px;
    overflow-y: auto;
    padding: 8px;
    border-radius: 8px;
    background: rgba(0, 0, 0, 0.02);
  }

  .item-card-friendly {
    background: linear-gradient(135deg, hsl(var(--b1)), hsl(var(--b2)));
    border-radius: 12px;
    border: 2px solid transparent;
    padding: 16px;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    cursor: pointer;
    position: relative;
    overflow: hidden;
  }

  .item-card-friendly::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(135deg, transparent, rgba(255, 255, 255, 0.1));
    opacity: 0;
    transition: opacity 0.3s ease;
    pointer-events: none;
  }

  .item-card-friendly:hover {
    border-color: hsl(var(--p));
    transform: translateY(-4px);
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
  }

  .item-card-friendly:hover::before {
    opacity: 1;
  }

  .item-card-friendly.expanded {
    background: linear-gradient(135deg, hsl(var(--p) / 0.1), hsl(var(--s) / 0.1));
    border-color: hsl(var(--p));
    box-shadow: 0 4px 20px hsl(var(--p) / 0.3);
  }

  .item-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    cursor: pointer;
    user-select: none;
    position: relative;
    z-index: 1;
  }

  .item-name-display {
    font-weight: 600;
    font-size: 14px;
    line-height: 1.3;
    flex: 1;
    margin-right: 12px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    color: hsl(var(--bc));
  }

  .item-count-display {
    background: linear-gradient(135deg, hsl(var(--wa)), hsl(var(--wa) / 0.8));
    color: hsl(var(--wac));
    padding: 4px 12px;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 700;
    min-width: 40px;
    text-align: center;
    box-shadow: 0 2px 8px hsl(var(--wa) / 0.3);
    flex-shrink: 0;
    margin-right: 8px;
    transition: all 0.2s ease;
  }

  .item-count-display.editable {
    cursor: pointer;
  }

  .item-count-display.editable:hover {
    background: linear-gradient(135deg, hsl(var(--p)), hsl(var(--p) / 0.8));
    transform: scale(1.05);
    box-shadow: 0 3px 12px hsl(var(--p) / 0.4);
  }

  .count-edit-wrapper {
    flex-shrink: 0;
    margin-right: 8px;
  }

  .count-edit-input {
    width: 60px;
    padding: 4px 8px;
    border-radius: 8px;
    border: 2px solid hsl(var(--p));
    background: hsl(var(--b1));
    color: hsl(var(--bc));
    font-size: 12px;
    font-weight: 700;
    text-align: center;
    outline: none;
    box-shadow: 0 0 0 3px hsl(var(--p) / 0.2);
  }

  .count-edit-input:focus {
    border-color: hsl(var(--p));
    box-shadow: 0 0 0 4px hsl(var(--p) / 0.3);
  }

  .expand-indicator {
    color: hsl(var(--bc) / 0.6);
    font-size: 12px;
    transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    flex-shrink: 0;
  }

  .expand-indicator.rotated {
    transform: rotate(180deg);
  }

  .item-controls-friendly {
    display: flex;
    flex-direction: column;
    gap: 8px;
    position: relative;
    z-index: 1;
  }

  .quantity-controls {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 12px;
    background: hsl(var(--b2));
    padding: 8px;
    border-radius: 8px;
    border: 1px solid hsl(var(--b3));
  }

  .control-btn {
    border: 2px solid hsl(var(--b3));
    border-radius: 8px;
    cursor: pointer;
    font-weight: 600;
    transition: all 0.2s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    font-family: inherit;
    background: hsl(var(--b1));
    color: hsl(var(--bc));
  }

  .control-btn.decrease,
  .control-btn.increase {
    width: 36px;
    height: 36px;
    font-size: 18px;
    font-weight: bold;
  }

  .control-btn.decrease {
    background: linear-gradient(135deg, hsl(var(--wa) / 0.1), hsl(var(--wa) / 0.05));
    color: hsl(var(--wa));
    border-color: hsl(var(--wa) / 0.3);
  }

  .control-btn.decrease:hover {
    background: linear-gradient(135deg, hsl(var(--wa) / 0.2), hsl(var(--wa) / 0.1));
    border-color: hsl(var(--wa));
    transform: scale(1.1);
  }

  .control-btn.increase {
    background: linear-gradient(135deg, hsl(var(--su) / 0.1), hsl(var(--su) / 0.05));
    color: hsl(var(--su));
    border-color: hsl(var(--su) / 0.3);
  }

  .control-btn.increase:hover {
    background: linear-gradient(135deg, hsl(var(--su) / 0.2), hsl(var(--su) / 0.1));
    border-color: hsl(var(--su));
    transform: scale(1.1);
  }

  .current-count {
    font-size: 16px;
    font-weight: 700;
    color: hsl(var(--bc));
    min-width: 30px;
    text-align: center;
  }

  .control-btn.remove-all {
    padding: 8px 16px;
    background: linear-gradient(135deg, hsl(var(--er) / 0.1), hsl(var(--er) / 0.05));
    color: hsl(var(--er));
    border: 2px solid hsl(var(--er) / 0.3);
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    font-weight: 600;
  }

  .control-btn.remove-all:hover {
    background: linear-gradient(135deg, hsl(var(--er) / 0.2), hsl(var(--er) / 0.1));
    border-color: hsl(var(--er));
    transform: translateY(-2px);
  }

  .suggested-items-section {
    margin-bottom: 24px;
    padding: 20px;
    background: linear-gradient(135deg, hsl(var(--p) / 0.1), hsl(var(--s) / 0.1));
    border: 2px solid hsl(var(--p) / 0.2);
    border-radius: 16px;
    animation: slideIn 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  }

  .suggested-items-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
    gap: 12px;
  }

  .suggested-item-card {
    background: hsl(var(--b1));
    border: 2px solid hsl(var(--su) / 0.3);
    border-radius: 12px;
    padding: 16px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    box-shadow: 0 2px 8px hsl(var(--su) / 0.1);
  }

  .suggested-item-card:hover {
    background: hsl(var(--su) / 0.05);
    border-color: hsl(var(--su));
    transform: translateY(-3px);
    box-shadow: 0 6px 20px hsl(var(--su) / 0.2);
  }

  .suggested-item-name {
    font-size: 13px;
    font-weight: 600;
    color: hsl(var(--bc));
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    margin-bottom: 4px;
  }

  .suggested-item-category {
    font-size: 10px;
    color: hsl(var(--su));
    text-transform: uppercase;
    letter-spacing: 0.5px;
    font-weight: 500;
  }

  .quick-add-btn {
    padding: 6px 12px;
    background: linear-gradient(135deg, hsl(var(--su) / 0.1), hsl(var(--su) / 0.05));
    color: hsl(var(--su));
    border: 2px solid hsl(var(--su) / 0.3);
    border-radius: 8px;
    font-size: 11px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;
    min-width: 40px;
  }

  .quick-add-btn:hover {
    background: linear-gradient(135deg, hsl(var(--su) / 0.2), hsl(var(--su) / 0.1));
    border-color: hsl(var(--su));
    transform: scale(1.05);
  }

  @keyframes slideIn {
    from {
      opacity: 0;
      transform: translateY(-20px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  .items-friendly-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
    gap: 8px;
    max-height: 500px;
    overflow-y: auto;
    padding: 6px;
  }

  .item-card-friendly {
    background: var(--pico-card-background-color);
    border-radius: var(--pico-border-radius);
    border: 1px solid var(--pico-card-border-color);
    padding: 0.75rem;
    transition: all 0.2s ease;
    box-shadow: var(--pico-card-box-shadow);
    cursor: pointer;
  }

  .item-card-friendly:hover {
    border-color: var(--pico-muted-border-color);
    background: var(--pico-card-sectioning-background-color);
    transform: translateY(-1px);
    box-shadow: 0 3px 8px rgba(0, 0, 0, 0.12);
  }

  .item-card-friendly.expanded {
    background: rgba(59, 130, 246, 0.08);
    border-color: rgba(59, 130, 246, 0.3);
    box-shadow: 0 2px 8px rgba(59, 130, 246, 0.15);
  }

  .item-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 8px;
    cursor: pointer;
    user-select: none;
  }

  .item-header:focus {
    outline: 2px solid var(--accent-primary);
    outline-offset: 2px;
    border-radius: 4px;
  }

  .item-name-display {
    font-weight: 600;
    color: var(--pico-color);
    font-size: 0.8125rem;
    line-height: 1.2;
    flex: 1;
    margin-right: 0.5rem;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .item-count-display {
    background: linear-gradient(135deg, var(--gold), #f59e0b);
    color: var(--bg-primary);
    padding: 3px 8px;
    border-radius: 12px;
    font-size: 12px;
    font-weight: 700;
    min-width: 30px;
    text-align: center;
    box-shadow: 0 1px 3px rgba(251, 191, 36, 0.3);
    flex-shrink: 0;
    margin-right: 8px;
  }

  .expand-indicator {
    color: var(--pico-muted-color);
    font-size: 0.625rem;
    transition: transform 0.2s ease;
    flex-shrink: 0;
  }

  .expand-indicator.rotated {
    transform: rotate(180deg);
  }

  .item-controls-friendly {
    display: flex;
    flex-direction: column;
    gap: 6px;
  }

  .quantity-controls {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    background: rgba(255, 255, 255, 0.05);
    padding: 6px;
    border-radius: 6px;
  }

  .control-btn {
    border: 1px solid var(--pico-muted-border-color);
    border-radius: var(--pico-border-radius);
    cursor: pointer;
    font-weight: 600;
    transition: all 0.2s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    font-family: inherit;
    background: var(--pico-form-element-background-color);
    color: var(--pico-color);
  }

  .control-btn.decrease,
  .control-btn.increase {
    width: 1.75rem;
    height: 1.75rem;
    font-size: 1rem;
    font-weight: bold;
  }

  .control-btn.decrease {
    background: rgba(245, 158, 11, 0.1);
    color: #f59e0b;
    border-color: rgba(245, 158, 11, 0.3);
  }

  .control-btn.decrease:hover {
    background: rgba(245, 158, 11, 0.2);
    border-color: #f59e0b;
    transform: scale(1.05);
  }

  .control-btn.increase {
    background: rgba(16, 185, 129, 0.1);
    color: #10b981;
    border-color: rgba(16, 185, 129, 0.3);
  }

  .control-btn.increase:hover {
    background: rgba(16, 185, 129, 0.2);
    border-color: #10b981;
    transform: scale(1.05);
  }

  .current-count {
    font-size: 0.875rem;
    font-weight: 700;
    color: var(--pico-color);
    min-width: 1.5625rem;
    text-align: center;
  }

  .control-btn.remove-all {
    padding: 0.375rem 0.75rem;
    background: rgba(239, 68, 68, 0.1);
    color: #ef4444;
    border: 1px solid rgba(239, 68, 68, 0.3);
    font-size: 0.6875rem;
    text-transform: uppercase;
    letter-spacing: 0.01875rem;
  }

  .control-btn.remove-all:hover {
    background: rgba(239, 68, 68, 0.2);
    border-color: #ef4444;
    transform: translateY(-1px);
  }

  .no-filter-results {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 20px;
    color: var(--text-muted);
    text-align: center;
  }

  .no-filter-results p {
    margin: 0 0 10px 0;
    font-size: 14px;
  }

  .clear-filter {
    padding: 0.375rem 0.75rem;
    background: var(--pico-primary);
    color: var(--pico-primary-inverse);
    border: 1px solid var(--pico-primary);
    border-radius: var(--pico-border-radius);
    cursor: pointer;
    font-size: 0.75rem;
    transition: all 0.2s ease;
  }

  .clear-filter:hover {
    background: var(--pico-primary-hover);
    border-color: var(--pico-primary-hover);
  }

  .suggested-items-section {
    margin-bottom: 1.25rem;
    padding: 1rem;
    background: var(--pico-primary-background);
    border: 1px solid rgba(59, 130, 246, 0.2);
    border-radius: var(--pico-border-radius);
    animation: slideIn 0.3s ease-out;
  }

  .suggested-header {
    margin-bottom: 12px;
  }

  .suggested-header h5 {
    margin: 0 0 0.25rem 0;
    color: var(--pico-primary);
    font-size: 0.9375rem;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 0.375rem;
  }

  .suggested-hint {
    margin: 0 0 0.75rem 0;
    font-size: 0.75rem;
    color: var(--pico-primary);
    font-style: italic;
    opacity: 0.8;
  }

  .suggested-items-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 8px;
  }

  .suggested-item-card {
    background: var(--pico-card-background-color);
    border: 1px solid rgba(16, 185, 129, 0.3);
    border-radius: var(--pico-border-radius);
    padding: 0.75rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    transition: all 0.2s ease;
    box-shadow: 0 1px 3px rgba(16, 185, 129, 0.1);
  }

  .suggested-item-card:hover {
    background: rgba(16, 185, 129, 0.05);
    border-color: #10b981;
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(16, 185, 129, 0.2);
  }

  .suggested-item-info {
    flex: 1;
    min-width: 0;
  }

  .suggested-item-name {
    font-size: 0.75rem;
    font-weight: 500;
    color: var(--pico-color);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    margin-bottom: 0.125rem;
  }

  .suggested-item-category {
    font-size: 10px;
    color: var(--success);
    text-transform: uppercase;
    letter-spacing: 0.3px;
  }

  .suggested-item-actions {
    display: flex;
    gap: 4px;
    margin-left: 8px;
  }

  .quick-add-btn {
    padding: 0.25rem 0.5rem;
    background: rgba(16, 185, 129, 0.1);
    color: #10b981;
    border: 1px solid rgba(16, 185, 129, 0.3);
    border-radius: var(--pico-border-radius);
    font-size: 0.625rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;
    min-width: 1.75rem;
  }

  .quick-add-btn:hover {
    background: rgba(16, 185, 129, 0.2);
    border-color: #10b981;
    transform: scale(1.05);
  }

  .no-items-friendly {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 40px 20px;
    text-align: center;
    background: rgba(255, 255, 255, 0.02);
    border-radius: 12px;
    border: 2px dashed rgba(255, 255, 255, 0.1);
  }

  .no-items-icon {
    color: var(--text-muted);
    margin-bottom: 16px;
    opacity: 0.7;
  }

  .no-items-friendly h3 {
    margin: 0 0 0.5rem 0;
    color: var(--pico-color);
    font-size: 1.2em;
    font-weight: 600;
  }

  .no-items-friendly p {
    margin: 0 0 1.25rem 0;
    color: var(--pico-muted-color);
    font-size: 0.9em;
    line-height: 1.4;
    max-width: 18.75rem;
  }

  .no-items-message {
    text-align: center;
    padding: 20px;
    color: var(--text-muted);
    background: rgba(255, 255, 255, 0.02);
    border-radius: 8px;
    border: 1px dashed rgba(255, 255, 255, 0.1);
    margin: 12px 0;
  }

  .no-items-message p {
    margin: 0;
    font-size: 14px;
    font-style: italic;
  }

  /* Animation for suggested items */
  @keyframes slideIn {
    from {
      opacity: 0;
      transform: translateY(-10px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }
</style>