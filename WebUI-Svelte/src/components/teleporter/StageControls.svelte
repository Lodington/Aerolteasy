<script>
  import { Map } from "lucide-svelte";

  export let selectedStage = null;
  export let isChangingStage = false;
  export let statusMessage = "";
  export let onChangeStage = () => {};
</script>

{#if selectedStage}
  <div class="card bg-base-100 shadow-lg">
    <div class="card-body">
      <h3 class="card-title text-lg mb-4">🚀 Stage Controls</h3>

      <div class="space-y-4">
        <!-- Stage Preview -->
        <div class="p-3 bg-base-200 rounded-lg">
          <h4 class="font-semibold text-sm mb-2">📋 Stage Change Preview</h4>
          <div class="text-sm space-y-1 opacity-80">
            <div class="flex justify-between">
              <span>Destination:</span>
              <span class="font-medium truncate ml-2">{selectedStage.label}</span>
            </div>
            <div class="flex justify-between">
              <span>Category:</span>
              <span class="font-medium">{selectedStage.category}</span>
            </div>
            <div class="flex justify-between">
              <span>Difficulty:</span>
              <span class="font-medium">{selectedStage.difficulty}</span>
            </div>
          </div>
        </div>

        <!-- Change Stage Button -->
        <button
          class="btn btn-primary w-full"
          on:click={onChangeStage}
          disabled={isChangingStage}
        >
          <Map size={16} />
          {isChangingStage ? "Changing Stage..." : `Travel to ${selectedStage.label}`}
        </button>

        <!-- Status Message -->
        {#if statusMessage}
          <div
            class="alert"
            class:alert-success={!statusMessage.includes("Failed")}
            class:alert-error={statusMessage.includes("Failed")}
          >
            <div>
              {statusMessage}
            </div>
          </div>
        {/if}
      </div>
    </div>
  </div>
{/if}