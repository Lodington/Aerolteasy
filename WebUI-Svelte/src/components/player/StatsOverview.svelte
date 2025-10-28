<script>
  import { selectedPlayer, getCharacterDisplayName } from '../../lib/stores.js';
  import { User, Crown, Heart, Package, TrendingUp, Shield } from 'lucide-svelte';

  function getHealthPercentage(player) {
    return player.MaxHealth > 0 ? (player.Health / player.MaxHealth) * 100 : 0;
  }

  function getHealthColor(percentage) {
    if (percentage > 75) return '#4ade80';
    if (percentage > 50) return '#fbbf24';
    if (percentage > 25) return '#fb923c';
    return '#ef4444';
  }

  function getTotalItems(player) {
    return Object.values(player.Items || {}).reduce((sum, count) => sum + count, 0);
  }
</script>

<div class="stats shadow">
  <div class="stat">
    <div class="stat-figure text-primary">
      <User size={24} />
    </div>
    <div class="stat-title">Character</div>
    <div class="stat-value text-lg">{getCharacterDisplayName($selectedPlayer.CurrentCharacter) || 'None'}</div>
  </div>

  <div class="stat">
    <div class="stat-figure text-secondary">
      <Crown size={24} />
    </div>
    <div class="stat-title">Level</div>
    <div class="stat-value">{$selectedPlayer.Level?.toFixed(0) || '0'}</div>
  </div>

  {#if $selectedPlayer.IsAlive}
    <div class="stat">
      <div class="stat-figure text-error">
        <Heart size={24} />
      </div>
      <div class="stat-title">Health</div>
      <div class="stat-value text-sm">
        {Math.round($selectedPlayer.Health)}/{Math.round($selectedPlayer.MaxHealth)}
      </div>
      <div class="stat-desc">
        <progress 
          class="progress progress-error w-20" 
          value={getHealthPercentage($selectedPlayer)} 
          max="100"
        ></progress>
        {Math.round(getHealthPercentage($selectedPlayer))}%
      </div>
    </div>
  {/if}

  <div class="stat">
    <div class="stat-figure text-accent">
      <Package size={24} />
    </div>
    <div class="stat-title">Items</div>
    <div class="stat-value">{getTotalItems($selectedPlayer)}</div>
  </div>

  <div class="stat">
    <div class="stat-figure text-warning">
      <TrendingUp size={24} />
    </div>
    <div class="stat-title">Damage</div>
    <div class="stat-value text-lg">{$selectedPlayer.BaseDamage?.toFixed(1) || '0'}</div>
  </div>

  <div class="stat">
    <div class="stat-figure text-info">
      <Shield size={24} />
    </div>
    <div class="stat-title">Armor</div>
    <div class="stat-value text-lg">{$selectedPlayer.Armor?.toFixed(1) || '0'}</div>
  </div>
</div>

<!-- DaisyUI handles all the styling -->