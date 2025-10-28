<script>
  import { createEventDispatcher } from 'svelte';
  
  export let selectedStage = '';
  export let placeholder = 'Search stages...';
  
  const dispatch = createEventDispatcher();
  
  let searchTerm = '';
  let isOpen = false;
  let filteredStages = [];
  let highlightedIndex = -1;
  let searchInput;
  let dropdown;

  const stages = [
    { 
      category: 'Stage 1 - Early Game', 
      stages: [
        { value: 'golemplains', label: 'Titanic Plains', description: 'Grassy plains with stone golems', emoji: '🌾', difficulty: 'Easy' },
        { value: 'blackbeach', label: 'Distant Roost', description: 'Coastal cliffs with flying enemies', emoji: '🏖️', difficulty: 'Easy' },
        { value: 'snowyforest', label: 'Siphoned Forest', description: 'Snowy forest with void corruption', emoji: '❄️', difficulty: 'Easy' }
      ]
    },
    { 
      category: 'Stage 2 - Mid Game', 
      stages: [
        { value: 'goolake', label: 'Abandoned Aqueduct', description: 'Ancient aqueduct with tar pools', emoji: '🏛️', difficulty: 'Medium' },
        { value: 'foggyswamp', label: 'Wetland Aspect', description: 'Misty swampland with mushrooms', emoji: '🌫️', difficulty: 'Medium' },
        { value: 'frozenwall', label: 'Rallypoint Delta', description: 'Frozen military outpost', emoji: '🧊', difficulty: 'Medium' }
      ]
    },
    { 
      category: 'Stage 3 - Advanced', 
      stages: [
        { value: 'wispgraveyard', label: 'Scorched Acres', description: 'Burned wasteland with fire enemies', emoji: '🔥', difficulty: 'Hard' },
        { value: 'sulfurpools', label: 'Sulfur Pools', description: 'Toxic pools with acid hazards', emoji: '☠️', difficulty: 'Hard' }
      ]
    },
    { 
      category: 'Stage 4 - Late Game', 
      stages: [
        { value: 'dampcavesimple', label: 'Abyssal Depths', description: 'Deep underground caverns', emoji: '🕳️', difficulty: 'Very Hard' },
        { value: 'shipgraveyard', label: "Siren's Call", description: 'Shipwreck with siren enemies', emoji: '🚢', difficulty: 'Very Hard' }
      ]
    },
    { 
      category: 'Stage 5 - Final Approach', 
      stages: [
        { value: 'skymeadow', label: 'Sky Meadow', description: 'Floating islands in the sky', emoji: '☁️', difficulty: 'Extreme' }
      ]
    },
    { 
      category: 'Final Stage', 
      stages: [
        { value: 'moon2', label: 'Commencement', description: 'The final battle on the moon', emoji: '🌙', difficulty: 'Ultimate' }
      ]
    },
    { 
      category: 'Special Stages', 
      stages: [
        { value: 'goldshores', label: 'Gilded Coast', description: 'Golden beach with Aurelionite', emoji: '🏆', difficulty: 'Special' },
        { value: 'mysteryspace', label: 'A Moment, Fractured', description: 'Mysterious void space', emoji: '🌌', difficulty: 'Special' },
        { value: 'bazaar', label: 'Bazaar Between Time', description: 'Lunar shop dimension', emoji: '🛒', difficulty: 'Peaceful' },
        { value: 'arena', label: 'Void Fields', description: 'Arena combat challenges', emoji: '⚔️', difficulty: 'Challenge' }
      ]
    }
  ];

  // Flatten stages for easier searching
  const allStages = stages.flatMap(category => 
    category.stages.map(stage => ({
      ...stage,
      category: category.category
    }))
  );

  $: {
    if (searchTerm.trim() === '') {
      filteredStages = allStages.slice(0, 12); // Limit initial results
    } else {
      const searchLower = searchTerm.toLowerCase();
      filteredStages = allStages.filter(stage =>
        stage.label.toLowerCase().includes(searchLower) ||
        stage.value.toLowerCase().includes(searchLower) ||
        stage.description.toLowerCase().includes(searchLower) ||
        stage.difficulty.toLowerCase().includes(searchLower) ||
        stage.category.toLowerCase().includes(searchLower)
      ).slice(0, 12); // Limit search results
    }
    highlightedIndex = -1;
  }

  $: selectedStageData = allStages.find(stage => stage.value === selectedStage);
  $: selectedStageLabel = selectedStageData ? `${selectedStageData.emoji} ${selectedStageData.label}` : '';

  function openDropdown() {
    isOpen = true;
    searchTerm = '';
    setTimeout(() => searchInput?.focus(), 0);
  }

  function closeDropdown() {
    isOpen = false;
    highlightedIndex = -1;
  }

  function selectStage(stage) {
    selectedStage = stage.value;
    dispatch('change', stage.value);
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
        highlightedIndex = Math.min(highlightedIndex + 1, filteredStages.length - 1);
        scrollToHighlighted();
        break;
      case 'ArrowUp':
        event.preventDefault();
        highlightedIndex = Math.max(highlightedIndex - 1, 0);
        scrollToHighlighted();
        break;
      case 'Enter':
        event.preventDefault();
        if (highlightedIndex >= 0 && filteredStages[highlightedIndex]) {
          selectStage(filteredStages[highlightedIndex]);
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
    if (!event.target.closest('.searchable-stage-select')) {
      closeDropdown();
    }
  }
</script>

<svelte:window on:click={handleClickOutside} />

<div class="searchable-stage-select">
  <button class="select-button" on:click={openDropdown} on:keydown={handleKeydown} type="button">
    <span class="selected-text">
      {selectedStageLabel || placeholder}
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
          placeholder="Search stages..."
          class="search-input"
        />
      </div>
      
      <div class="options" bind:this={dropdown}>
        {#if searchTerm.trim() === ''}
          <div class="search-hint">
            <p>🗺️ Try searching for:</p>
            <div class="search-examples">
              <button class="example-search" on:click={() => searchTerm = 'plains'} type="button">Plains</button>
              <button class="example-search" on:click={() => searchTerm = 'roost'} type="button">Roost</button>
              <button class="example-search" on:click={() => searchTerm = 'stage 1'} type="button">Stage 1</button>
              <button class="example-search" on:click={() => searchTerm = 'final'} type="button">Final</button>
              <button class="example-search" on:click={() => searchTerm = 'special'} type="button">Special</button>
            </div>
          </div>
        {/if}
        
        {#each filteredStages as stage, index}
          <button
            class="option stage-card"
            class:highlighted={index === highlightedIndex}
            on:click={() => selectStage(stage)}
            on:mouseenter={() => highlightedIndex = index}
            type="button"
          >
            <div class="stage-info">
              <div class="stage-emoji">{stage.emoji}</div>
              <div class="stage-details">
                <div class="stage-name">{stage.label}</div>
                <div class="stage-category">{stage.category}</div>
                <div class="stage-description">{stage.description}</div>
                <div class="stage-difficulty">Difficulty: {stage.difficulty}</div>
              </div>
            </div>
          </button>
        {/each}
        
        {#if filteredStages.length === 0 && searchTerm.trim() !== ''}
          <div class="no-results">
            <p>No stages found matching "{searchTerm}"</p>
            <button class="clear-filter" on:click={() => searchTerm = ''} type="button">Clear Search</button>
          </div>
        {/if}
      </div>
    </div>
  {/if}
</div>

<style>
  .searchable-stage-select {
    position: relative;
    width: 100%;
  }

  .select-button {
    display: flex;
    justify-content: space-between;
    align-items: center;
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
    background: var(--bg-secondary, #1a1a2e);
    border: 1px solid var(--border-primary, rgba(255, 255, 255, 0.2));
    border-radius: 12px;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.4);
    max-height: 400px;
    overflow: hidden;
  }

  .search-container {
    padding: 8px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  .search-input {
    width: 100%;
    padding: 10px 12px;
    border: 2px solid var(--border-primary, rgba(255, 255, 255, 0.2));
    border-radius: 8px;
    background: var(--bg-tertiary, rgba(255, 255, 255, 0.1));
    color: var(--text-primary, white);
    font-size: 14px;
    transition: all 0.2s ease;
  }

  .search-input:focus {
    outline: none;
    border-color: var(--accent-primary, #ffd700);
    background: var(--bg-hover, rgba(255, 255, 255, 0.15));
    box-shadow: 0 0 0 3px rgba(255, 215, 0, 0.1);
  }

  .search-input::placeholder {
    color: var(--text-muted, rgba(255, 255, 255, 0.6));
  }

  .options {
    max-height: 340px;
    overflow-y: auto;
  }

  .option {
    width: 100%;
    padding: 12px;
    border: none;
    background: transparent;
    color: inherit;
    cursor: pointer;
    transition: all 0.2s ease;
    border-bottom: 1px solid rgba(255, 255, 255, 0.05);
    text-align: left;
  }

  .option:hover,
  .option.highlighted {
    background: var(--bg-hover, rgba(255, 215, 0, 0.1));
    transform: translateX(4px);
  }

  .option:last-child {
    border-bottom: none;
  }

  .stage-card .stage-info {
    display: flex;
    align-items: flex-start;
    gap: 12px;
  }

  .stage-emoji {
    font-size: 24px;
    min-width: 32px;
    text-align: center;
  }

  .stage-details {
    flex: 1;
    min-width: 0;
  }

  .stage-name {
    font-weight: 600;
    color: var(--text-primary, white);
    font-size: 1em;
    margin-bottom: 2px;
  }

  .stage-category {
    font-size: 0.75em;
    color: var(--accent-primary, #ffd700);
    margin-bottom: 4px;
  }

  .stage-description {
    font-size: 0.8em;
    color: var(--text-secondary, rgba(255, 255, 255, 0.8));
    line-height: 1.3;
    margin-bottom: 4px;
  }

  .stage-difficulty {
    font-size: 0.75em;
    color: var(--text-muted, rgba(255, 255, 255, 0.6));
    font-weight: 500;
  }

  .no-results {
    padding: 16px;
    text-align: center;
    color: var(--text-muted, rgba(255, 255, 255, 0.6));
  }

  .no-results p {
    margin: 0 0 8px 0;
    font-style: italic;
  }

  .clear-filter {
    padding: 4px 8px;
    background: var(--bg-tertiary, rgba(255, 255, 255, 0.1));
    border: 1px solid var(--border-primary, rgba(255, 255, 255, 0.2));
    border-radius: 6px;
    color: var(--text-secondary, rgba(255, 255, 255, 0.7));
    cursor: pointer;
    transition: all 0.2s ease;
    font-size: 0.8em;
  }

  .clear-filter:hover {
    background: var(--bg-hover, rgba(255, 255, 255, 0.15));
    color: var(--text-primary, white);
  }

  .search-hint {
    padding: 16px;
    text-align: center;
    color: var(--text-secondary, rgba(255, 255, 255, 0.7));
  }

  .search-hint p {
    margin: 0 0 12px 0;
    font-size: 0.9em;
  }

  .search-examples {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
    justify-content: center;
  }

  .example-search {
    padding: 4px 8px;
    background: var(--bg-tertiary, rgba(255, 255, 255, 0.1));
    border: 1px solid var(--border-primary, rgba(255, 255, 255, 0.2));
    border-radius: 12px;
    color: var(--text-secondary, rgba(255, 255, 255, 0.7));
    font-size: 0.75em;
    cursor: pointer;
    transition: all 0.2s ease;
  }

  .example-search:hover {
    background: var(--accent-primary, #ffd700);
    color: var(--bg-primary, #1a1a2e);
    border-color: var(--accent-primary, #ffd700);
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