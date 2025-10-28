<script>
  import { createEventDispatcher } from 'svelte';
  
  export let variant = 'primary'; // primary, secondary, success, error, warning, info, accent
  export let size = 'normal'; // xs, sm, normal, lg
  export let disabled = false;
  export let outline = false;
  export let ghost = false;
  export let glass = false;
  export let wide = false;
  export let block = false;
  
  const dispatch = createEventDispatcher();
  
  function handleClick() {
    if (!disabled) {
      dispatch('click');
    }
  }

  // Map variants to DaisyUI classes
  $: variantClass = (() => {
    if (outline) {
      switch (variant) {
        case 'primary': return 'btn-outline btn-primary';
        case 'secondary': return 'btn-outline btn-secondary';
        case 'success': return 'btn-outline btn-success';
        case 'error': return 'btn-outline btn-error';
        case 'warning': return 'btn-outline btn-warning';
        case 'info': return 'btn-outline btn-info';
        case 'accent': return 'btn-outline btn-accent';
        default: return 'btn-outline';
      }
    } else if (ghost) {
      return 'btn-ghost';
    } else {
      switch (variant) {
        case 'primary': return 'btn-primary';
        case 'secondary': return 'btn-secondary';
        case 'success': return 'btn-success';
        case 'error': return 'btn-error';
        case 'warning': return 'btn-warning';
        case 'info': return 'btn-info';
        case 'accent': return 'btn-accent';
        default: return '';
      }
    }
  })();

  $: sizeClass = (() => {
    switch (size) {
      case 'xs': return 'btn-xs';
      case 'sm': return 'btn-sm';
      case 'small': return 'btn-sm'; // backward compatibility
      case 'lg': return 'btn-lg';
      default: return '';
    }
  })();

  $: modifierClasses = [
    glass && 'glass',
    wide && 'btn-wide',
    block && 'btn-block'
  ].filter(Boolean).join(' ');

  $: buttonClass = [
    'btn',
    variantClass,
    sizeClass,
    modifierClasses
  ].filter(Boolean).join(' ');
</script>

<button 
  class={buttonClass}
  on:click={handleClick}
  {disabled}
>
  <slot />
</button>