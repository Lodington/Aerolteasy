<script>
  import { Zap, Target, SkipForward, MapPin } from "lucide-svelte";

  export let isProcessing = false;
  export let statusMessage = "";
  export let onChargeTeleporter = () => {};
  export let onActivateTeleporter = () => {};
  export let onSkipTeleporterEvent = () => {};
  export let onSpawnTeleporter = () => {};
</script>

<div class="card bg-base-100 shadow-lg">
  <div class="card-body">
    <h3 class="card-title text-lg mb-4">⚡ Teleporter Management</h3>
    
    <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
      <button
        class="btn btn-outline"
        on:click={onChargeTeleporter}
        disabled={isProcessing}
      >
        <Zap size={16} />
        Charge Teleporter
      </button>

      <button
        class="btn btn-outline"
        on:click={onActivateTeleporter}
        disabled={isProcessing}
      >
        <Target size={16} />
        Activate Teleporter
      </button>

      <button
        class="btn btn-outline"
        on:click={onSkipTeleporterEvent}
        disabled={isProcessing}
      >
        <SkipForward size={16} />
        Skip Event
      </button>

      <button
        class="btn btn-outline"
        on:click={onSpawnTeleporter}
        disabled={isProcessing}
      >
        <MapPin size={16} />
        Spawn Teleporter
      </button>
    </div>

    <!-- Status Message -->
    {#if statusMessage}
      <div
        class="alert mt-4"
        class:alert-success={!statusMessage.includes("Failed")}
        class:alert-error={statusMessage.includes("Failed")}
      >
        <div>
          {statusMessage}
        </div>
      </div>
    {/if}

    <!-- Help Text -->
    <div class="mt-4 p-3 bg-base-200 rounded-lg">
      <h4 class="font-semibold text-sm mb-2">💡 Teleporter Actions</h4>
      <div class="text-xs space-y-1 opacity-80">
        <div><strong>Charge:</strong> Instantly charge teleporter to 100%</div>
        <div><strong>Activate:</strong> Start the teleporter event</div>
        <div><strong>Skip Event:</strong> Complete the current teleporter event</div>
        <div><strong>Spawn:</strong> Create a new teleporter at your location</div>
      </div>
    </div>
  </div>
</div>