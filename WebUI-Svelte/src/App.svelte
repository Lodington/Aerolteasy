<script>
  import { onMount, onDestroy } from "svelte";
  import { gameState, isConnected, selectedPlayerId, networkStatus, toast, showToast } from "./lib/stores.js";
  import { api } from "./lib/api.js";
  import { networkApi } from "./lib/networkApi.js";

  import Navigation from "./components/Navigation.svelte";
  import GameStatePanel from "./components/GameStatePanel.svelte";
  import RunRequiredOverlay from "./components/RunRequiredOverlay.svelte";
  import Toast from "./components/Toast.svelte";
  import NetworkStatusIndicator from "./components/NetworkStatusIndicator.svelte";
  
  // View components
  import PlayerView from "./components/views/PlayerView.svelte";
  import MonsterView from "./components/views/MonsterView.svelte";
  import InteractableView from "./components/views/InteractableView.svelte";
  import TeleporterView from "./components/views/TeleporterView.svelte";
  import WorldView from "./components/views/WorldView.svelte";
  import NetworkView from "./components/views/NetworkView.svelte";
  import SettingsView from "./components/views/SettingsView.svelte";

  let pollInterval;
  let currentView = 'players';
  let previousRunState = false;
  let justEndedRun = false;
  let mockModeActive = false;

  onMount(() => {
    startPolling();
    setupBrowserConsoleCommands();
  });

  onDestroy(() => {
    if (pollInterval) {
      clearInterval(pollInterval);
    }
  });

  function startPolling() {
    pollInterval = setInterval(async () => {
      // Skip polling if we're in mock mode
      if (mockModeActive) {
        console.log('⏸️ Polling skipped - Mock mode active');
        return;
      }
      
      try {
        const connected = await api.checkStatus();
        isConnected.set(connected);

        if (connected) {
          const state = await api.getGameState();
          if (state) {
            gameState.set(state);
          }

          // Poll network status
          const netStatus = await networkApi.getNetworkStatus();
          if (netStatus) {
            networkStatus.set(netStatus);
          }
        }
      } catch (error) {
        console.error("Polling error:", error);
        isConnected.set(false);
      }
    }, 1000); // Reduced from 500ms to 1000ms to reduce load
  }

  function handleViewChange(event) {
    currentView = event.detail;
  }

  // Enhanced overlay detection - show overlay if:
  // 1. Not in run OR
  // 2. No players exist OR  
  // 3. Not connected to game
  // BUT: Never show overlay in mock mode
  $: showOverlay = !mockModeActive && (
                   !$gameState.IsInRun || 
                   !$gameState.Players || 
                   $gameState.Players.length === 0 || 
                   !$isConnected
                   );

  // Watch for run state changes and reset UI when run ends
  $: {
    // More robust run state detection
    const currentRunState = $gameState.IsInRun && 
                           $gameState.Players && 
                           $gameState.Players.length > 0 && 
                           $isConnected;
    
    // If we were in a run and now we're not, reset the UI (but not in mock mode)
    if (previousRunState && !currentRunState && !mockModeActive) {
      resetUIState();
    }
    
    // If we just entered a run, ensure we have a selected player
    if (!previousRunState && currentRunState && $gameState.Players?.length > 0) {
      // Show welcome toast
      showToast('Run started - Dev tools are now available!', 'success');
      
      // Auto-select first player if none selected
      if (!$selectedPlayerId) {
        selectedPlayerId.set($gameState.Players[0].PlayerId);
      }
    }
    
    previousRunState = currentRunState;
  }

  function resetUIState() {
    console.log('Run ended - resetting UI state');
    
    // Show toast notification
    showToast('Run ended - Interface has been reset', 'warning');
    
    // Reset selected player
    selectedPlayerId.set(null);
    
    // Reset to default view
    currentView = 'players';
    
    // Show "just ended run" state for a few seconds
    justEndedRun = true;
    setTimeout(() => {
      justEndedRun = false;
    }, 5000); // Show for 5 seconds then return to normal message
    
    // Could add more state resets here as needed
    // For example: clear any cached data, reset form states, etc.
  }

  // Browser console functions for development
  function setupBrowserConsoleCommands() {
    // Generate mock data function
    window.generateMockData = () => {
      console.log('🎮 Generating mock game data...');
      
      const mockGameState = {
        IsInRun: true,
        CurrentStage: "golemplains",
        StageDisplayName: "Titanic Plains",
        GameTime: 1234.56,
        DifficultyCoefficient: 2.75,
        TotalEnemiesAlive: 12,
        TeamMoney: 1250,
        Players: generateMockPlayers(),
        Teleporter: {
          IsActive: true,
          IsCharged: false,
          ChargeProgress: 0.65
        },
        Interactables: generateMockInteractables(),
        Monsters: generateMockMonsters()
      };

      // Enable mock mode to prevent polling from overriding our data
      mockModeActive = true;
      console.log('🔧 Mock mode enabled:', mockModeActive);
      
      gameState.set(mockGameState);
      isConnected.set(true);
      
      // Auto-select the first player for immediate testing
      if (mockGameState.Players.length > 0) {
        selectedPlayerId.set(mockGameState.Players[0].PlayerId);
      }
      
      console.log('✅ Mock data generated!');
      console.log(`📊 Created ${mockGameState.Players.length} players, ${mockGameState.Interactables.length} interactables, ${mockGameState.Monsters.length} monsters`);
      console.log('🎯 Auto-selected first player for testing');
      console.log('⏸️ Polling disabled to preserve mock data');
      
      // Show toast
      showToast('Mock data generated - UI ready for testing!', 'success');
    };

    // Clear mock data function
    window.clearMockData = () => {
      console.log('🧹 Clearing mock data...');
      
      gameState.set({
        IsInRun: false,
        Players: [],
        Interactables: [],
        Monsters: [],
        Teleporter: null,
        CurrentStage: null,
        GameTime: 0,
        DifficultyCoefficient: 0,
        TotalEnemiesAlive: 0,
        TeamMoney: 0
      });
      
      selectedPlayerId.set(null);
      isConnected.set(false);
      
      // Re-enable polling to get real game state
      mockModeActive = false;
      
      console.log('✅ Mock data cleared!');
      console.log('▶️ Polling re-enabled for real game state');
      
      // Show toast
      showToast('Mock data cleared - Polling resumed', 'info');
    };

    // Force restart polling (useful for debugging)
    window.restartPolling = () => {
      console.log('🔄 Restarting polling...');
      if (pollInterval) {
        clearInterval(pollInterval);
      }
      startPolling();
      console.log('✅ Polling restarted');
    };

    // Help function
    window.devHelp = () => {
      console.log(`
🎮 RoR2 Dev Tool - Browser Console Commands:

📊 generateMockData()  - Generate fake game data for UI testing (disables polling)
🧹 clearMockData()     - Clear mock data and resume normal polling
🔍 mockStatus()        - Check current mock mode and polling status
🔄 restartPolling()    - Force restart the polling interval
❓ devHelp()           - Show this help message

Example usage:
> generateMockData()   // Creates 4 fake players, hides overlay, stops polling
> clearMockData()      // Removes fake data, shows overlay, resumes polling

Mock Mode Features:
- Automatically hides the "Run Required" overlay
- Auto-selects first player for immediate testing
- Disables polling to prevent data from being overwritten
- Perfect for UI development and demonstrations

Current Status: ${mockModeActive ? '🧪 Mock Mode Active' : '🔴 Normal Mode'}
      `);
    };

    // Add status check function
    window.mockStatus = () => {
      console.log(`Mock Mode: ${mockModeActive ? '🧪 ACTIVE' : '🔴 INACTIVE'}`);
      console.log(`Polling: ${mockModeActive ? '⏸️ PAUSED' : '▶️ RUNNING'}`);
      return { mockMode: mockModeActive, polling: !mockModeActive };
    };

    console.log('🎮 RoR2 Dev Tool loaded! Type devHelp() for available commands.');
  }

  function generateMockPlayers() {
    const characterBodies = ["CommandoBody", "HuntressBody", "Bandit2Body", "ToolbotBody"];
    const playerNames = ["TestPlayer1", "DevUser", "MockPlayer", "UITester"];
    const players = [];

    for (let i = 0; i < 4; i++) {
      const isAlive = i < 3; // Make one player dead
      const health = isAlive ? Math.random() * 50 + 50 : 0;
      const maxHealth = 100 + (i * 25);

      players.push({
        PlayerId: i + 1,
        PlayerName: playerNames[i],
        CurrentCharacter: characterBodies[i],
        CharacterIcon: getCharacterIcon(characterBodies[i]),
        Level: Math.floor(Math.random() * 14) + 1,
        Experience: Math.random() * 1000,
        Health: health,
        MaxHealth: maxHealth,
        Shield: isAlive ? Math.random() * 25 : 0,
        MaxShield: 25,
        HealthRegen: Math.random() * 4 + 1,
        IsAlive: isAlive,
        GodModeEnabled: i === 0, // First player has god mode
        BaseDamage: Math.random() * 13 + 12,
        Armor: Math.random() * 20,
        AttackSpeed: Math.random() * 1.5 + 1,
        CritChance: Math.random() * 20 + 5,
        MoveSpeed: Math.random() * 5 + 7,
        JumpPower: Math.random() * 5 + 15,
        Items: generateMockItems(i)
      });
    }

    return players;
  }

  function generateMockItems(playerIndex) {
    const items = {};
    const commonItems = ["Syringe", "Bear", "Crowbar", "Mushroom", "Hoof"];
    const uncommonItems = ["Whip", "ATG", "Ukulele", "Infusion", "Bandolier"];
    const legendaryItems = ["Dagger", "Behemoth", "Brainstalks", "Clover", "Headstompers"];

    // Add common items
    for (let i = 0; i < Math.floor(Math.random() * 3) + 3; i++) {
      const item = commonItems[Math.floor(Math.random() * commonItems.length)];
      items[item] = Math.floor(Math.random() * 7) + 1;
    }

    // Add uncommon items
    for (let i = 0; i < Math.floor(Math.random() * 3) + 1; i++) {
      const item = uncommonItems[Math.floor(Math.random() * uncommonItems.length)];
      items[item] = Math.floor(Math.random() * 3) + 1;
    }

    // Maybe add legendary
    if (Math.random() < 0.3) {
      const item = legendaryItems[Math.floor(Math.random() * legendaryItems.length)];
      items[item] = 1;
    }

    return items;
  }

  function generateMockInteractables() {
    const interactables = [];
    const types = ["Chest1", "Chest2", "GoldChest", "EquipmentBarrel", "Scrapper", "Duplicator"];

    for (let i = 0; i < Math.floor(Math.random() * 7) + 8; i++) {
      interactables.push({
        Name: types[Math.floor(Math.random() * types.length)],
        Position: {
          x: Math.random() * 100 - 50,
          y: Math.random() * 20,
          z: Math.random() * 100 - 50
        },
        IsAvailable: Math.random() > 0.3,
        Cost: Math.floor(Math.random() * 175) + 25
      });
    }

    return interactables;
  }

  function generateMockMonsters() {
    const monsters = [];
    const types = ["Beetle", "BeetleGuard", "Wisp", "Lemurian", "Golem", "ClayBruiser"];

    for (let i = 0; i < Math.floor(Math.random() * 10) + 10; i++) {
      const maxHealth = Math.random() * 400 + 100;
      const health = Math.random() * maxHealth * 0.8 + maxHealth * 0.2;

      monsters.push({
        Name: types[Math.floor(Math.random() * types.length)],
        Position: {
          x: Math.random() * 200 - 100,
          y: Math.random() * 30,
          z: Math.random() * 200 - 100
        },
        Health: health,
        MaxHealth: maxHealth,
        IsElite: Math.random() < 0.2,
        IsBoss: Math.random() < 0.1
      });
    }

    return monsters;
  }

  function getCharacterIcon(characterBody) {
    const icons = {
      "CommandoBody": "🔫",
      "HuntressBody": "🏹", 
      "Bandit2Body": "🔪",
      "ToolbotBody": "🤖",
      "EngiBody": "🔧",
      "MageBody": "🔮",
      "MercBody": "⚔️",
      "TreebotBody": "🌱",
      "LoaderBody": "👊",
      "CrocoBody": "🦎",
      "CaptainBody": "⚓"
    };
    return icons[characterBody] || "👤";
  }
</script>

<div data-theme="dark" class="min-h-screen bg-base-100">
  {#if showOverlay}
    <RunRequiredOverlay {justEndedRun} />
  {:else}
    <div class="drawer lg:drawer-open">
      <input id="drawer-toggle" type="checkbox" class="drawer-toggle" />
      
      <!-- Main Content Area -->
      <div class="drawer-content flex flex-col min-h-screen">
        <!-- Top Navigation -->
        <Navigation activeView={currentView} on:viewChange={handleViewChange} />
        
        <!-- Main Content with proper spacing -->
        <main class="flex-1 p-4 lg:p-6 bg-base-100">
          <div class="max-w-7xl mx-auto animate-fade-in">
            {#if currentView === 'players'}
              <PlayerView />
            {:else if currentView === 'monsters'}
              <MonsterView />
            {:else if currentView === 'interactables'}
              <InteractableView />
            {:else if currentView === 'teleporter'}
              <TeleporterView />
            {:else if currentView === 'world'}
              <WorldView />
            {:else if currentView === 'network'}
              <NetworkView />
            {:else if currentView === 'settings'}
              <SettingsView />
            {/if}
          </div>
        </main>
        
        <!-- Footer -->
        <footer class="footer footer-center p-4 bg-base-300 text-base-content">
          <div>
            <p class="text-sm opacity-70">
              🎮 RoR2 Dev Tool - Risk of Rain 2 Development Assistant
            </p>
          </div>
        </footer>
      </div>
      
      <!-- Sidebar -->
      <div class="drawer-side">
        <label for="drawer-toggle" class="drawer-overlay"></label>
        <aside class="w-80 min-h-full bg-base-200 border-r border-base-300">
          <!-- Sidebar Header -->
          <div class="p-4 border-b border-base-300">
            <div class="flex items-center gap-3">
              <div class="avatar placeholder">
                <div class="bg-primary text-primary-content rounded-lg w-10">
                  <span class="text-xl">📊</span>
                </div>
              </div>
              <div>
                <h2 class="font-bold text-lg">Game State</h2>
                <p class="text-sm opacity-70">Live monitoring</p>
              </div>
            </div>
          </div>
          
          <!-- Network Status -->
          <div class="p-4 border-b border-base-300">
            <NetworkStatusIndicator />
          </div>

          <!-- Game State Panel -->
          <div class="p-4">
            <GameStatePanel />
          </div>
        </aside>
      </div>
    </div>
  {/if}
</div>

<!-- Toast notifications -->
<Toast bind:show={$toast.show} message={$toast.message} type={$toast.type} duration={$toast.duration || 3000} />

<style>
/* Custom animations for DaisyUI */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes slideIn {
  from { opacity: 0; transform: translateX(-20px); }
  to { opacity: 1; transform: translateX(0); }
}

.animate-fade-in { animation: fadeIn 0.3s ease-out; }

</style>
