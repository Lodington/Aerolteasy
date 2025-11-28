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
  <div class="toast toast-{type}" transition:fade={{ duration: 300 }}>
    <div class="toast-content">
      <div class="toast-icon-wrapper">
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
      </div>
      <span class="toast-message">{message}</span>
    </div>
    <div class="toast-progress" style="animation-duration: {duration}ms;"></div>
  </div>
{/if}

<style>
/* Modern Toast Notifications */
.toast {
  position: fixed !important;
  top: 20px;
  right: 20px;
  z-index: 9999 !important;
  max-width: 420px;
  min-width: 280px;
  width: fit-content;
  height: auto;
  max-height: 200px;
  display: block;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3), 0 2px 8px rgba(0, 0, 0, 0.2);
  backdrop-filter: blur(16px);
  animation: slideInRight 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);
  pointer-events: auto;
}

@keyframes slideInRight {
  from {
    transform: translateX(120%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.toast-content {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 18px;
  position: relative;
  z-index: 1;
}

.toast-icon-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  flex-shrink: 0;
  animation: iconPop 0.5s cubic-bezier(0.68, -0.55, 0.265, 1.55) 0.2s backwards;
}

@keyframes iconPop {
  0% {
    transform: scale(0);
    opacity: 0;
  }
  50% {
    transform: scale(1.2);
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

.toast-icon {
  font-size: 20px;
  line-height: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.toast-message {
  font-size: 14px;
  font-weight: 500;
  word-break: break-word;
  line-height: 1.5;
  flex: 1;
  animation: fadeInText 0.4s ease-out 0.3s backwards;
}

@keyframes fadeInText {
  from {
    opacity: 0;
    transform: translateX(-10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Progress bar */
.toast-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  height: 3px;
  width: 100%;
  transform-origin: left;
  animation: shrinkProgress linear forwards;
}

@keyframes shrinkProgress {
  from {
    transform: scaleX(1);
  }
  to {
    transform: scaleX(0);
  }
}

/* Info Toast */
.toast-info {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.95), rgba(37, 99, 235, 0.95));
  border: 1px solid rgba(96, 165, 250, 0.3);
  color: white;
}

.toast-info .toast-icon-wrapper {
  background: rgba(255, 255, 255, 0.2);
}

.toast-info .toast-progress {
  background: linear-gradient(90deg, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0.4));
}

/* Success Toast */
.toast-success {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.95), rgba(22, 163, 74, 0.95));
  border: 1px solid rgba(74, 222, 128, 0.3);
  color: white;
}

.toast-success .toast-icon-wrapper {
  background: rgba(255, 255, 255, 0.2);
}

.toast-success .toast-progress {
  background: linear-gradient(90deg, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0.4));
}

/* Warning Toast */
.toast-warning {
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.95), rgba(217, 119, 6, 0.95));
  border: 1px solid rgba(251, 191, 36, 0.3);
  color: white;
}

.toast-warning .toast-icon-wrapper {
  background: rgba(255, 255, 255, 0.2);
}

.toast-warning .toast-progress {
  background: linear-gradient(90deg, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0.4));
}

/* Error Toast */
.toast-error {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.95), rgba(220, 38, 38, 0.95));
  border: 1px solid rgba(248, 113, 113, 0.3);
  color: white;
}

.toast-error .toast-icon-wrapper {
  background: rgba(255, 255, 255, 0.2);
}

.toast-error .toast-progress {
  background: linear-gradient(90deg, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0.4));
}

/* Hover effect */
.toast:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 48px rgba(0, 0, 0, 0.35), 0 4px 12px rgba(0, 0, 0, 0.25);
  transition: all 0.2s ease;
}

.toast:hover .toast-progress {
  animation-play-state: paused;
}

/* Mobile responsive */
@media (max-width: 768px) {
  .toast {
    top: 10px;
    right: 10px;
    left: 10px;
    max-width: none;
    min-width: 0;
  }
  
  @keyframes slideInRight {
    from {
      transform: translateY(-120%);
      opacity: 0;
    }
    to {
      transform: translateY(0);
      opacity: 1;
    }
  }
}
</style>