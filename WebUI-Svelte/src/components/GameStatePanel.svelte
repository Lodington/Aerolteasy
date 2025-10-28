<script>
  import { gameState, formattedMoney, stageDisplay, formattedGameTime, alivePlayers, totalPlayers } from '../lib/stores.js';

  import { Clock, Users, DollarSign, Map, TrendingUp, Play, Pause, Zap, Target } from 'lucide-svelte';
</script>

<div class="space-y-4">
  <!-- Status Card -->
  <div class="card bg-base-100 shadow-md">
    <div class="card-body p-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-2">
          {#if $gameState.IsInRun}
            <Play size={16} class="text-success" />
          {:else}
            <Pause size={16} class="text-error" />
          {/if}
          <span class="font-medium">Status:</span>
        </div>
        <div class="badge {$gameState.IsInRun ? 'badge-success' : 'badge-error'}">
          {$gameState.IsInRun ? 'In Run' : 'Not in Run'}
        </div>
      </div>
    </div>
  </div>

  {#if $gameState.IsInRun}
    <!-- Game Stats -->
    <div class="space-y-3">
      <!-- Time -->
      <div class="card bg-base-100 shadow-sm">
        <div class="card-body p-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <Clock size={14} class="text-info" />
              <span class="text-sm font-medium">Time:</span>
            </div>
            <span class="text-sm font-mono">{$formattedGameTime}</span>
          </div>
        </div>
      </div>
      
      <!-- Players -->
      <div class="card bg-base-100 shadow-sm">
        <div class="card-body p-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <Users size={14} class="text-primary" />
              <span class="text-sm font-medium">Players:</span>
            </div>
            <div class="flex items-center gap-1">
              <span class="text-sm">{$alivePlayers.length}/{$totalPlayers}</span>
              <div class="badge badge-xs badge-success">Alive</div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Money -->
      <div class="card bg-base-100 shadow-sm">
        <div class="card-body p-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <DollarSign size={14} class="text-warning" />
              <span class="text-sm font-medium">Team Money:</span>
            </div>
            <span class="text-sm font-mono text-warning">{$formattedMoney}</span>
          </div>
        </div>
      </div>
      
      <!-- Stage -->
      <div class="card bg-base-100 shadow-sm">
        <div class="card-body p-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <Map size={14} class="text-accent" />
              <span class="text-sm font-medium">Stage:</span>
            </div>
            <span class="text-sm">{$stageDisplay || '-'}</span>
          </div>
        </div>
      </div>
      
      <!-- Difficulty -->
      <div class="card bg-base-100 shadow-sm">
        <div class="card-body p-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <TrendingUp size={14} class="text-secondary" />
              <span class="text-sm font-medium">Difficulty:</span>
            </div>
            <span class="text-sm font-mono">{$gameState.DifficultyCoefficient?.toFixed(2) || '0.00'}x</span>
          </div>
        </div>
      </div>

      <!-- Teleporter -->
      {#if $gameState.Teleporter}
        <div class="card bg-base-100 shadow-sm">
          <div class="card-body p-3">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <Zap size={14} class={$gameState.Teleporter.IsActive ? 'text-primary' : 'text-neutral'} />
                <span class="text-sm font-medium">Teleporter:</span>
              </div>
              <div class="flex items-center gap-2">
                {#if $gameState.Teleporter.IsCharged}
                  <div class="badge badge-success badge-sm">Charged</div>
                {:else if $gameState.Teleporter.IsActive}
                  <div class="flex items-center gap-1">
                    <progress class="progress progress-primary w-12 h-2" value={$gameState.Teleporter.ChargeProgress * 100} max="100"></progress>
                    <span class="text-xs">{Math.round($gameState.Teleporter.ChargeProgress * 100)}%</span>
                  </div>
                {:else}
                  <div class="badge badge-neutral badge-sm">Inactive</div>
                {/if}
              </div>
            </div>
          </div>
        </div>
      {/if}

      <!-- Enemies -->
      <div class="card bg-base-100 shadow-sm">
        <div class="card-body p-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <Target size={14} class="text-error" />
              <span class="text-sm font-medium">Enemies:</span>
            </div>
            <div class="flex items-center gap-1">
              <span class="text-sm">{$gameState.TotalEnemiesAlive || 0}</span>
              <div class="badge badge-xs badge-error">Alive</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  {:else}
    <!-- No Run State -->
    <div class="card bg-base-100 shadow-md">
      <div class="card-body text-center p-6">
        <div class="avatar placeholder mb-4">
          <div class="bg-neutral text-neutral-content rounded-full w-12">
            <Pause size={24} />
          </div>
        </div>
        <h3 class="font-bold text-lg mb-2">No Active Run</h3>
        <p class="text-sm opacity-70">Start a run in Risk of Rain 2 to see live game information</p>
      </div>
    </div>
  {/if}
</div>

<!-- DaisyUI handles all the styling -->