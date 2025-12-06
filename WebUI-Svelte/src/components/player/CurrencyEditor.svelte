<script>
  import { selectedPlayer } from '../../lib/stores.js';
  import { Coins, Moon, Skull } from 'lucide-svelte';
  import { api } from '../../lib/api.js';
  
  export let refreshGameState = () => {};

  let moneyAmount = '';
  let lunarCoinsAmount = '';
  let voidCoinsAmount = '';

  async function setMoney() {
    if (!$selectedPlayer || !moneyAmount) return;
    
    try {
      const amount = parseInt(moneyAmount);
      if (isNaN(amount) || amount < 0) return;
      
      await api.sendCommand({
        Type: 'setmoney',
        Data: { amount, playerId: $selectedPlayer.PlayerId }
      });
      
      moneyAmount = '';
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to set money:', error);
    }
  }

  async function setLunarCoins() {
    if (!$selectedPlayer || !lunarCoinsAmount) return;
    
    try {
      const amount = parseInt(lunarCoinsAmount);
      if (isNaN(amount) || amount < 0) return;
      
      await api.sendCommand({
        Type: 'setlunarcoins',
        Data: { amount, playerId: $selectedPlayer.PlayerId }
      });
      
      lunarCoinsAmount = '';
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to set lunar coins:', error);
    }
  }

  async function setVoidCoins() {
    if (!$selectedPlayer || !voidCoinsAmount) return;
    
    try {
      const amount = parseInt(voidCoinsAmount);
      if (isNaN(amount) || amount < 0) return;
      
      await api.sendCommand({
        Type: 'setvoidcoins',
        Data: { amount, playerId: $selectedPlayer.PlayerId }
      });
      
      voidCoinsAmount = '';
      setTimeout(refreshGameState, 50);
    } catch (error) {
      console.error('Failed to set void coins:', error);
    }
  }

  function handleKeydown(event, action) {
    if (event.key === 'Enter') {
      action();
    }
  }
</script>

<div class="card bg-base-200 shadow-lg">
  <div class="card-body">
    <h3 class="card-title mb-4">💰 Currency Editor</h3>
    
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <!-- Money -->
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium flex items-center gap-2">
            <Coins size={16} class="text-warning" />
            Money
          </span>
        </label>
        <div class="join">
          <input 
            type="number" 
            placeholder="Amount"
            class="input input-bordered join-item flex-1"
            bind:value={moneyAmount}
            on:keydown={(e) => handleKeydown(e, setMoney)}
            min="0"
          />
          <button 
            class="btn btn-warning join-item"
            on:click={setMoney}
            disabled={!moneyAmount}
          >
            Set
          </button>
        </div>
        <label class="label">
          <span class="label-text-alt">Press Enter or click Set</span>
        </label>
      </div>

      <!-- Lunar Coins -->
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium flex items-center gap-2">
            <Moon size={16} class="text-info" />
            Lunar Coins
          </span>
        </label>
        <div class="join">
          <input 
            type="number" 
            placeholder="Amount"
            class="input input-bordered join-item flex-1"
            bind:value={lunarCoinsAmount}
            on:keydown={(e) => handleKeydown(e, setLunarCoins)}
            min="0"
          />
          <button 
            class="btn btn-info join-item"
            on:click={setLunarCoins}
            disabled={!lunarCoinsAmount}
          >
            Set
          </button>
        </div>
        <label class="label">
          <span class="label-text-alt">Press Enter or click Set</span>
        </label>
      </div>

      <!-- Void Coins -->
      <div class="form-control">
        <label class="label">
          <span class="label-text font-medium flex items-center gap-2">
            <Skull size={16} class="text-secondary" />
            Void Coins
          </span>
        </label>
        <div class="join">
          <input 
            type="number" 
            placeholder="Amount"
            class="input input-bordered join-item flex-1"
            bind:value={voidCoinsAmount}
            on:keydown={(e) => handleKeydown(e, setVoidCoins)}
            min="0"
          />
          <button 
            class="btn btn-secondary join-item"
            on:click={setVoidCoins}
            disabled={!voidCoinsAmount}
          >
            Set
          </button>
        </div>
        <label class="label">
          <span class="label-text-alt">Press Enter or click Set</span>
        </label>
      </div>
    </div>

    <div class="alert alert-info mt-4">
      <div class="flex items-center gap-2">
        <Coins size={16} />
        <span class="text-sm">
          Enter the exact amount you want to set for <strong>{$selectedPlayer?.PlayerName}</strong>
        </span>
      </div>
    </div>
  </div>
</div>
