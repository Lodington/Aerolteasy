<script>
  import { createEventDispatcher } from 'svelte';
  import { Search, ChevronDown } from 'lucide-svelte';

  export let value = '';
  export let items = [];
  export let placeholder = 'Search for an item...';

  const dispatch = createEventDispatcher();
  
  let isOpen = false;
  let searchTerm = '';
  let searchInput;
  
  // Flatten items and filter based on search
  $: flatItems = items.flatMap(category => 
    category.items.map(item => ({ 
      ...item, 
      category: category.category 
    }))
  );
  
  $: filteredItems = flatItems.filter(item => 
    item.label.toLowerCase().includes(searchTerm.toLowerCase()) ||
    item.value.toLowerCase().includes(searchTerm.toLowerCase()) ||
    item.category.toLowerCase().includes(searchTerm.toLowerCase())
  );

  $: selectedItem = flatItems.find(item => item.value === value);

  function selectItem(item) {
    value = item.value;
    dispatch('change', { value: item.value });
    isOpen = false;
    searchTerm = '';
  }

  function toggleDropdown() {
    isOpen = !isOpen;
    if (isOpen) {
      searchTerm = '';
      setTimeout(() => searchInput?.focus(), 10);
    }
  }

  function handleKeydown(event) {
    if (!isOpen) return;
    
    if (event.key === 'Escape') {
      isOpen = false;
    } else if (event.key === 'Enter' && filteredItems.length > 0) {
      event.preventDefault();
      selectItem(filteredItems[0]);
    } else if (event.key === 'ArrowDown' && filteredItems.length > 0) {
      event.preventDefault();
      // Could add arrow key navigation here in the future
    }
  }

  function handleClickOutside(event) {
    if (isOpen && !event.target.closest('.searchable-select')) {
      isOpen = false;
    }
  }
</script>

<svelte:window on:keydown={handleKeydown} on:click={handleClickOutside} />

<div class="searchable-select" class:open={isOpen}>
  <button class="select-button" on:click={toggleDropdown} type="button">
    <div class="selected-item">
      {#if selectedItem}
        <span class="item-name">{selectedItem.label}</span>
        <span class="item-category">({selectedItem.category})</span>
      {:else}
        <span class="placeholder">{placeholder}</span>
      {/if}
    </div>
    <div class="chevron" class:rotated={isOpen}>
      <ChevronDown size={16} />
    </div>
  </button>

  {#if isOpen}
    <div class="dropdown">
      <div class="search-container">
        <div class="search-icon">
          <Search size={16} />
        </div>
        <input 
          bind:this={searchInput}
          type="text" 
          placeholder="Type to search..." 
          bind:value={searchTerm}
          class="search-input"
        />
      </div>
      
      <div class="items-list">
        {#each filteredItems as item (item.value)}
          <button 
            class="item-option" 
            on:click={() => selectItem(item)}
            type="button"
          >
            <div class="item-details">
              <span class="item-label">{item.label}</span>
              <span class="item-category-small">{item.category}</span>
            </div>
          </button>
        {/each}
        
        {#if filteredItems.length === 0}
          <div class="no-results">
            {searchTerm ? `No items found for "${searchTerm}"` : 'No items available'}
          </div>
        {/if}
      </div>
    </div>
  {/if}
</div>

<style>
  .searchable-select {
    position: relative;
    width: 100%;
  }

  .select-button {
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 10px 12px;
    background: var(--bg-tertiary);
    border: 1px solid var(--border-primary);
    border-radius: 8px;
    color: var(--text-primary);
    cursor: pointer;
    transition: all 0.2s ease;
    font-family: inherit;
    font-size: 14px;
  }

  .select-button:hover {
    border-color: var(--border-secondary);
  }

  .select-button:focus {
    outline: none;
    border-color: var(--accent-primary);
    box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.1);
  }

  .selected-item {
    display: flex;
    align-items: center;
    gap: 8px;
    flex: 1;
    text-align: left;
  }

  .item-name {
    font-weight: 500;
  }

  .item-category {
    font-size: 0.85em;
    color: var(--text-secondary);
  }

  .placeholder {
    color: var(--text-muted);
  }

  .chevron {
    transition: transform 0.2s ease;
    color: var(--text-secondary);
  }

  .chevron.rotated {
    transform: rotate(180deg);
  }

  .dropdown {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    z-index: 1000;
    background: var(--bg-secondary);
    border: 1px solid var(--border-primary);
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
    margin-top: 4px;
    max-height: 320px;
    overflow: hidden;
  }

  .search-container {
    position: relative;
    padding: 12px;
    border-bottom: 1px solid var(--border-primary);
  }

  .search-icon {
    position: absolute;
    left: 20px;
    top: 50%;
    transform: translateY(-50%);
    color: var(--text-muted);
    pointer-events: none;
  }

  .search-input {
    width: 100%;
    padding: 8px 12px 8px 36px;
    background: var(--bg-tertiary);
    border: 1px solid var(--border-primary);
    border-radius: 6px;
    color: var(--text-primary);
    font-family: inherit;
    font-size: 14px;
  }

  .search-input:focus {
    outline: none;
    border-color: var(--accent-primary);
    box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.1);
  }

  .search-input::placeholder {
    color: var(--text-muted);
  }

  .items-list {
    max-height: 250px;
    overflow-y: auto;
  }

  .item-option {
    width: 100%;
    display: flex;
    align-items: center;
    padding: 10px 12px;
    background: transparent;
    border: none;
    color: var(--text-primary);
    cursor: pointer;
    transition: background 0.2s ease;
    font-family: inherit;
    text-align: left;
  }

  .item-option:hover {
    background: var(--bg-hover);
  }

  .item-details {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }

  .item-label {
    font-size: 0.9em;
    font-weight: 500;
  }

  .item-category-small {
    font-size: 0.75em;
    color: var(--text-muted);
  }

  .no-results {
    padding: 16px;
    text-align: center;
    color: var(--text-muted);
    font-style: italic;
    font-size: 0.9em;
  }
</style>