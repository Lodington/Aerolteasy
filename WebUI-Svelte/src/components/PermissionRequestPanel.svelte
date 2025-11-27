<script>
  import { networkStatus } from '../lib/stores.js';
  import { networkApi, PermissionLevels } from '../lib/networkApi.js';
  import Panel from './Panel.svelte';

  let requestedLevel = PermissionLevels.BASIC;
  let isRequesting = false;
  let requestMessage = '';
  let requestSuccess = false;

  const permissionOptions = [
    { value: PermissionLevels.READ_ONLY, label: 'Read Only', description: 'View game state only', autoApprove: true },
    { value: PermissionLevels.BASIC, label: 'Basic', description: 'Spawn items, set money/health', autoApprove: true },
    { value: PermissionLevels.ADVANCED, label: 'Advanced', description: 'Teleport, spawn monsters', autoApprove: false },
    { value: PermissionLevels.ADMIN, label: 'Admin', description: 'Change stage, kill/revive', autoApprove: false }
  ];

  async function handleRequest() {
    isRequesting = true;
    requestMessage = '';
    requestSuccess = false;

    try {
      const result = await networkApi.requestPermission(requestedLevel);
      if (result.success) {
        requestSuccess = true;
        const option = permissionOptions.find(o => o.value === requestedLevel);
        if (option?.autoApprove) {
          requestMessage = `✅ ${requestedLevel} permission granted!`;
        } else {
          requestMessage = `📨 Request sent to host. Waiting for approval...`;
        }
      } else {
        requestMessage = `❌ Request failed: ${result.message}`;
      }
    } catch (error) {
      requestMessage = `❌ Error: ${error.message}`;
    } finally {
      isRequesting = false;
    }
  }

  $: showPanel = !$networkStatus.isHost && $networkStatus.currentPermission === PermissionLevels.NONE;
</script>

{#if showPanel}
  <Panel title="🔐 Request Permissions" icon="🔑">
    <div class="space-y-4">
      <div class="alert alert-info">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" class="stroke-current shrink-0 w-6 h-6">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
        </svg>
        <span class="text-sm">You need permissions from the host to use dev tools.</span>
      </div>

      <div class="form-control">
        <label class="label">
          <span class="label-text font-semibold">Select Permission Level</span>
        </label>
        
        <div class="space-y-2">
          {#each permissionOptions as option}
            <label class="label cursor-pointer bg-base-200 rounded-lg p-3 hover:bg-base-300 transition-colors">
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <span class="font-semibold">{option.label}</span>
                  {#if option.autoApprove}
                    <span class="badge badge-success badge-sm">Auto-approve</span>
                  {:else}
                    <span class="badge badge-warning badge-sm">Requires approval</span>
                  {/if}
                </div>
                <p class="text-xs opacity-70 mt-1">{option.description}</p>
              </div>
              <input 
                type="radio" 
                name="permission-level" 
                class="radio radio-primary" 
                value={option.value}
                bind:group={requestedLevel}
              />
            </label>
          {/each}
        </div>
      </div>

      <button 
        class="btn btn-primary w-full"
        class:loading={isRequesting}
        disabled={isRequesting}
        on:click={handleRequest}
      >
        {isRequesting ? 'Requesting...' : 'Request Permission'}
      </button>

      {#if requestMessage}
        <div class="alert" class:alert-success={requestSuccess} class:alert-error={!requestSuccess}>
          <span class="text-sm">{requestMessage}</span>
        </div>
      {/if}
    </div>
  </Panel>
{/if}
