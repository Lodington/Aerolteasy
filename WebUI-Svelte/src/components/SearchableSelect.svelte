<script>
  import { createEventDispatcher } from 'svelte';
  
  export let items = [];
  export let selectedValue = '';
  export let placeholder = 'Search...';
  export let valueKey = 'value';
  export let labelKey = 'label';
  export let categoryKey = 'category';
  
  const dispatch = createEventDispatcher();
  
  let searchTerm = '';
  let isOpen = false;
  let filteredItems = [];
  let highlightedIndex = -1;
  let searchInput;
  let dropdown;

  $: {
    if (searchTerm.trim() === '') {
      filteredItems = items;
    } else {
      filteredItems = items.filter(item =>
        item[labelKey].toLowerCase().includes(searchTerm.toLowerCase()) ||
        item[valueKey].toLowerCase().includes(searchTerm.toLowerCase()) ||
        (item[categoryKey] && item[categoryKey].toLowerCase().includes(searchTerm.toLowerCase()))
      );
    }
    highlightedIndex = -1;
  }

  $: selectedLabel = items.find(item => item[valueKey] === selectedValue)?.[labelKey] || '';

  function openDropdown() {
    isOpen = true;
    searchTerm = '';
    setTimeout(() => searchInput?.focus(), 0);
  }

  function closeDropdown() {
    isOpen = false;
    highlightedIndex = -1;
  }

  function selectItem(item) {
    selectedValue = item[valueKey];
    dispatch('change', item[valueKey]);
    closeDropdown();
  }

  function handleKeydown(event) {
    if (!isOpen) {
      if (event.key === 'Enter' || event.key === ' ') {
        event.preventDefault();
        openDropdown();
      }
      return;
    }

    switch (event.key) {
      case 'Escape':
        closeDropdown();
        break;
      case 'ArrowDown':
        event.preventDefault();
        highlightedIndex = Math.min(highlightedIndex + 1, filteredItems.length - 1);
        scrollToHighlighted();
        break;
      case 'ArrowUp':
        event.preventDefault();
        highlightedIndex = Math.max(highlightedIndex - 1, 0);
        scrollToHighlighted();
        break;
      case 'Enter':
        event.preventDefault();
        if (highlightedIndex >= 0 && filteredItems[highlightedIndex]) {
          selectItem(filteredItems[highlightedIndex]);
        }
        break;
    }
  }

  function scrollToHighlighted() {
    if (highlightedIndex >= 0 && dropdown) {
      const highlightedElement = dropdown.children[highlightedIndex];
      if (highlightedElement) {
        highlightedElement.scrollIntoView({ block: 'nearest' });
      }
    }
  }

  function handleClickOutside(event) {
    if (!event.target.closest('.searchable-select')) {
      closeDropdown();
    }
  }
</script>

<svelte:window on:click={handleClickOutside} />

<div class="searchable-select">
  <button class="select-button" on:click={openDropdown} on:keydown={handleKeydown} type="button">
    <span class="selected-text">
      {selectedLabel || placeholder}
    </span>
    <span class="arrow" class:open={isOpen}>▼</span>
  </button>

  {#if isOpen}
    <div class="dropdown">
      <div class="search-container">
        <input
          bind:this={searchInput}
          bind:value={searchTerm}
          on:keydown={handleKeydown}
          placeholder="Search..."
          class="search-input"
        />
      </div>
      
      <div class="options" bind:this={dropdown}>
        {#each filteredItems as item, index}
          <button
            class="option"
            class:highlighted={index === highlightedIndex}
            on:click={() => selectItem(item)}
            on:mouseenter={() => highlightedIndex = index}
            type="button"
          >
            <div class="item-info">
              <span class="item-name">{item[labelKey]}</span>
              {#if item[categoryKey]}
                <span class="item-category">{item[categoryKey]}</span>
              {/if}
            </div>
          </button>
        {/each}
        
        {#if filteredItems.length === 0}
          <div class="no-results">No items found</div>
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
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    padding: 12px;
    border: none;
    border-radius: 8px;
    background: rgba(255, 255, 255, 0.1);
    color: white;
    cursor: pointer;
    transition: background-color 0.2s;
  }

  .select-button:hover {
    background: rgba(255, 255, 255, 0.15);
  }

  .select-button:focus {
    outline: 2px solid #ffd700;
    outline-offset: 2px;
  }

  .selected-text {
    flex: 1;
    text-align: left;
  }

  .arrow {
    transition: transform 0.2s;
    font-size: 12px;
  }

  .arrow.open {
    transform: rotate(180deg);
  }

  .dropdown {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    z-index: 1000;
    background: #1a1a2e;
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
    max-height: 300px;
    overflow: hidden;
  }

  .search-container {
    padding: 8px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  .search-input {
    width: 100%;
    padding: 8px;
    border: none;
    border-radius: 4px;
    background: rgba(255, 255, 255, 0.1);
    color: white;
    font-size: 14px;
  }

  .search-input:focus {
    outline: 1px solid #ffd700;
    background: rgba(255, 255, 255, 0.15);
  }

  .search-input::placeholder {
    color: rgba(255, 255, 255, 0.6);
  }

  .options {
    max-height: 240px;
    overflow-y: auto;
  }

  .option {
    width: 100%;
    padding: 12px;
    border: none;
    background: transparent;
    color: inherit;
    cursor: pointer;
    transition: background-color 0.2s;
    border-bottom: 1px solid rgba(255, 255, 255, 0.05);
    text-align: left;
  }

  .option:hover,
  .option.highlighted {
    background: rgba(255, 215, 0, 0.1);
  }

  .option:last-child {
    border-bottom: none;
  }

  .item-info {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }

  .item-name {
    font-weight: 500;
    color: white;
  }

  .item-category {
    font-size: 12px;
    color: rgba(255, 255, 255, 0.7);
  }

  .no-results {
    padding: 12px;
    text-align: center;
    color: rgba(255, 255, 255, 0.6);
    font-style: italic;
  }

  /* Scrollbar styling */
  .options::-webkit-scrollbar {
    width: 6px;
  }

  .options::-webkit-scrollbar-track {
    background: rgba(255, 255, 255, 0.1);
  }

  .options::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.3);
    border-radius: 3px;
  }

  .options::-webkit-scrollbar-thumb:hover {
    background: rgba(255, 255, 255, 0.5);
  }
</style>