<script>
  import { fade } from 'svelte/transition';
  
  export let message = '';
  export let type = 'info'; // 'info', 'success', 'warning', 'error'
  export let duration = 3000;
  export let show = false;
  
  let visible = false;
  
  $: if (show && message) {
    visible = true;
    setTimeout(() => {
      visible = false;
      show = false;
    }, duration);
  }
</script>

{#if visible}
  <div class="toast" data-type={type} transition:fade={{ duration: 300 }}>
    <div class="flex items-center gap-2">
      <span class="toast-icon">
        {#if type === 'success'}
          ✅
        {:else if type === 'warning'}
          ⚠️
        {:else if type === 'error'}
          ❌
        {:else}
          ℹ️
        {/if}
      </span>
      <span class="toast-message">{message}</span>
    </div>
  </div>
{/if}

<style>
/* Toast notifications using Pico CSS with data attributes */
.toast {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 2000;
  padding: 0.75rem 1rem;
  border-radius: var(--pico-border-radius);
  backdrop-filter: blur(10px);
  max-width: 400px;
  min-width: 250px;
  width: fit-content;
  border: 1px solid;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}

.toast[data-type="info"] {
  background: var(--pico-primary-background);
  color: var(--pico-primary-inverse);
  border-color: var(--pico-primary);
}

.toast[data-type="success"] {
  background: rgba(34, 197, 94, 0.9);
  color: white;
  border-color: #22c55e;
}

.toast[data-type="warning"] {
  background: rgba(245, 158, 11, 0.9);
  color: white;
  border-color: #f59e0b;
}

.toast[data-type="error"] {
  background: rgba(239, 68, 68, 0.9);
  color: white;
  border-color: #ef4444;
}

.toast-message {
  font-size: 0.875rem;
  font-weight: 500;
  word-break: break-word;
}

@media (max-width: 768px) {
  .toast {
    top: 10px;
    right: 10px;
    left: auto;
    max-width: calc(100vw - 20px);
    min-width: 200px;
  }
}
</style>