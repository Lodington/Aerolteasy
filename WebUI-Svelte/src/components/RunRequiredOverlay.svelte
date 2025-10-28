<script>
  import { isConnected, gameState } from '../lib/stores.js';
  import { Zap, Play, AlertCircle, Wifi, WifiOff, RotateCcw } from 'lucide-svelte';
  
  export let justEndedRun = false;
  
  // Debug info for troubleshooting
  $: debugInfo = {
    connected: $isConnected,
    isInRun: $gameState.IsInRun,
    playerCount: $gameState.Players?.length || 0,
    currentStage: $gameState.CurrentStage || 'None'
  };
</script>

<!-- Modern DaisyUI Overlay -->
<div class="fixed inset-0 bg-gradient-to-br from-base-200 via-base-100 to-base-300 z-50 overflow-y-auto">
  <div class="min-h-screen flex items-center justify-center p-4">
    <div class="max-w-4xl w-full space-y-8 animate-fade-in">
      
      <!-- Header Section -->
      <div class="text-center space-y-4">
        <div class="avatar placeholder">
          <div class="bg-gradient-to-br from-primary to-secondary text-primary-content rounded-full w-24 h-24 shadow-2xl flex items-center justify-center">
            <Zap size={48} />
          </div>
        </div>
        <div class="space-y-2">
          <h1 class="text-4xl font-bold bg-gradient-to-r from-primary to-secondary bg-clip-text text-transparent">
            🎮 Risk of Rain 2 Dev Tool
          </h1>
          <p class="text-lg opacity-70">Advanced development and debugging interface</p>
        </div>
      </div>

      <!-- Connection Status Card -->
      <div class="card {$isConnected ? 'bg-success/10 border-success/20' : 'bg-error/10 border-error/20'} border-2 shadow-xl">
        <div class="card-body">
          <div class="flex items-center gap-4">
            <div class="avatar placeholder">
              <div class="bg-{$isConnected ? 'success' : 'error'} text-{$isConnected ? 'success' : 'error'}-content rounded-full w-12 h-12 flex items-center justify-center">
                {#if $isConnected}
                  <Wifi size={24} />
                {:else}
                  <WifiOff size={24} />
                {/if}
              </div>
            </div>
            <div class="flex-1">
              <h3 class="card-title text-{$isConnected ? 'success' : 'error'}">
                {$isConnected ? 'Connected to Game' : 'Not Connected'}
              </h3>
              <p class="opacity-70">
                {$isConnected ? 'Mod is running and ready' : 'Make sure the mod is installed and game is running'}
              </p>
            </div>
            <div class="badge {$isConnected ? 'badge-success' : 'badge-error'} gap-2">
              {#if $isConnected}
                <div class="w-2 h-2 bg-success-content rounded-full animate-pulse"></div>
                Online
              {:else}
                <div class="w-2 h-2 bg-error-content rounded-full"></div>
                Offline
              {/if}
            </div>
          </div>
        </div>
      </div>

      <!-- Main Message Card -->
      <div class="card bg-base-100 shadow-2xl">
        <div class="card-body text-center space-y-6">
          <div class="flex justify-center">
            <div class="bg-{justEndedRun ? 'warning' : 'info'} text-{justEndedRun ? 'warning' : 'info'}-content rounded-full w-20 h-20 flex items-center justify-center {justEndedRun ? 'animate-bounce' : 'animate-pulse'} shadow-lg">
              {#if justEndedRun}
                <RotateCcw size={40} />
              {:else}
                <div class="flex items-center justify-center" style="margin-left: 2px;">
                  <Play size={40} />
                </div>
              {/if}
            </div>
          </div>
          
          {#if justEndedRun}
            <div class="space-y-2">
              <h2 class="card-title text-2xl justify-center text-warning">Run Ended - Interface Reset</h2>
              <p class="opacity-70">The run has ended and the interface has been reset. Start a new run to continue using the dev tools.</p>
            </div>
          {:else}
            <div class="space-y-2">
              <h2 class="card-title text-2xl justify-center text-info">Start a Run to Continue</h2>
              <p class="opacity-70">The dev tool interface will become available once you start a Risk of Rain 2 run.</p>
            </div>
          {/if}
        </div>
      </div>

      <!-- Features Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="card bg-gradient-to-br from-primary/10 to-primary/5 shadow-lg hover:shadow-xl transition-all">
          <div class="card-body">
            <h3 class="card-title text-primary">🎮 Player Management</h3>
            <p class="text-sm opacity-70">Control player stats, items, characters, and abilities with advanced search and filtering.</p>
            <div class="card-actions justify-end">
              <div class="badge badge-primary badge-outline">Enhanced</div>
            </div>
          </div>
        </div>
        
        <div class="card bg-gradient-to-br from-secondary/10 to-secondary/5 shadow-lg hover:shadow-xl transition-all">
          <div class="card-body">
            <h3 class="card-title text-secondary">👹 Monster Spawning</h3>
            <p class="text-sm opacity-70">Spawn any monster with custom items, buffs, and team assignments for testing scenarios.</p>
            <div class="card-actions justify-end">
              <div class="badge badge-secondary badge-outline">Advanced</div>
            </div>
          </div>
        </div>
        
        <div class="card bg-gradient-to-br from-accent/10 to-accent/5 shadow-lg hover:shadow-xl transition-all">
          <div class="card-body">
            <h3 class="card-title text-accent">📦 Interactable Control</h3>
            <p class="text-sm opacity-70">Place chests, shrines, and other interactables anywhere on the map instantly.</p>
            <div class="card-actions justify-end">
              <div class="badge badge-accent badge-outline">Instant</div>
            </div>
          </div>
        </div>
        
        <div class="card bg-gradient-to-br from-info/10 to-info/5 shadow-lg hover:shadow-xl transition-all">
          <div class="card-body">
            <h3 class="card-title text-info">🌀 Teleporter Management</h3>
            <p class="text-sm opacity-70">Control teleporter state and travel between any stage with search-based selection.</p>
            <div class="card-actions justify-end">
              <div class="badge badge-info badge-outline">Smart</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Instructions Card -->
      <div class="card bg-base-100 shadow-xl">
        <div class="card-body">
          <div class="flex items-center gap-3 mb-4">
            <AlertCircle size={24} class="text-warning" />
            <h3 class="card-title text-warning">Getting Started</h3>
          </div>
          <div class="steps steps-vertical lg:steps-horizontal">
            <div class="step step-primary">
              <div class="text-left">
                <div class="font-semibold">Install Mod</div>
                <div class="text-sm opacity-70">Make sure RoR2DevTool mod is installed and enabled</div>
              </div>
            </div>
            <div class="step step-primary">
              <div class="text-left">
                <div class="font-semibold">Launch Game</div>
                <div class="text-sm opacity-70">Start Risk of Rain 2 and begin any run</div>
              </div>
            </div>
            <div class="step step-primary">
              <div class="text-left">
                <div class="font-semibold">Auto-Unlock</div>
                <div class="text-sm opacity-70">Interface unlocks automatically when run starts</div>
              </div>
            </div>
            <div class="step step-primary">
              <div class="text-left">
                <div class="font-semibold">Navigate</div>
                <div class="text-sm opacity-70">Use tabs to access development features</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Debug Info (Collapsible) -->
      {#if !$isConnected}
        <div class="collapse collapse-arrow bg-base-200 shadow-lg">
          <input type="checkbox" />
          <div class="collapse-title text-lg font-medium flex items-center gap-2">
            🔍 Debug Information
          </div>
          <div class="collapse-content">
            <div class="stats stats-vertical lg:stats-horizontal shadow">
              <div class="stat">
                <div class="stat-title">Connection</div>
                <div class="stat-value text-{debugInfo.connected ? 'success' : 'error'} text-lg">
                  {debugInfo.connected ? 'Connected' : 'Disconnected'}
                </div>
              </div>
              <div class="stat">
                <div class="stat-title">In Run</div>
                <div class="stat-value text-lg">{debugInfo.isInRun ? 'Yes' : 'No'}</div>
              </div>
              <div class="stat">
                <div class="stat-title">Players</div>
                <div class="stat-value text-lg">{debugInfo.playerCount}</div>
              </div>
              <div class="stat">
                <div class="stat-title">Current Stage</div>
                <div class="stat-value text-sm">{debugInfo.currentStage || 'None'}</div>
              </div>
            </div>
          </div>
        </div>
      {/if}

      <!-- Footer -->
      <div class="text-center space-y-2 opacity-60">
        <p class="text-sm">🔧 Advanced development tools for Risk of Rain 2 modding and testing</p>
        <div class="badge badge-outline">Version 2.0 - Enhanced Edition</div>
      </div>
    </div>
  </div>
</div>

<style>
  /* Custom animations for the overlay */
  @keyframes fadeIn {
    from { 
      opacity: 0; 
      transform: translateY(30px) scale(0.95);
    }
    to { 
      opacity: 1; 
      transform: translateY(0) scale(1);
    }
  }

  .animate-fade-in {
    animation: fadeIn 0.8s cubic-bezier(0.4, 0, 0.2, 1);
  }

  /* Enhanced hover effects for feature cards */
  .card:hover {
    transform: translateY(-4px);
  }

  /* Gradient text support for older browsers */
  .bg-clip-text {
    -webkit-background-clip: text;
    background-clip: text;
  }

  /* Custom step styling */
  .steps .step::before {
    transition: all 0.3s ease;
  }

  /* Smooth transitions for all interactive elements */
  .card, .badge, .btn {
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  }

  /* Force icon centering in avatar placeholders */
  .avatar.placeholder > div {
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
  }

  /* Ensure SVG icons are properly centered */
  .avatar.placeholder svg {
    display: block;
    margin: auto;
  }

  /* Special adjustment for Play icon visual centering */
  .avatar.placeholder svg[data-lucide="play"] {
    transform: translateX(2px);
  }
</style>