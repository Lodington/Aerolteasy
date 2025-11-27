<script>
  import { userPermission } from '../lib/stores.js';
  import { hasPermissionForCommand, COMMAND_PERMISSIONS } from '../lib/networkApi.js';

  export let commandType = '';
  export let disabled = false;
  export let className = 'btn btn-primary';
  export let title = '';

  $: hasPermission = commandType ? hasPermissionForCommand($userPermission, commandType) : true;
  $: isDisabled = disabled || !hasPermission;
  $: requiredPermission = COMMAND_PERMISSIONS[commandType?.toLowerCase()] || 'Admin';
  $: tooltipText = !hasPermission 
    ? `Requires ${requiredPermission} permission` 
    : title;
</script>

<div class="tooltip" data-tip={tooltipText}>
  <button 
    class={className}
    disabled={isDisabled}
    on:click
    {...$$restProps}
  >
    <slot />
    {#if !hasPermission}
      <span class="badge badge-xs badge-error ml-1">🔒</span>
    {/if}
  </button>
</div>
