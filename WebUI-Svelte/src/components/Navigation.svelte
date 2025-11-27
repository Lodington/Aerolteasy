<script>
  import { createEventDispatcher } from 'svelte';
  import { User, Zap, Package, Settings, Cog, Eye, Menu, Network } from 'lucide-svelte';
  
  export let activeView = 'players';
  
  const dispatch = createEventDispatcher();
  
  const navItems = [
    { id: 'players', label: 'Player Management', icon: User, color: 'text-primary' },
    { id: 'world', label: 'ESP & Tracking', icon: Eye, color: 'text-secondary' },
    { id: 'monsters', label: 'Monster Spawning', icon: Zap, color: 'text-warning' },
    { id: 'interactables', label: 'Interactables', icon: Package, color: 'text-accent' },
    { id: 'teleporter', label: 'Teleporter Controls', icon: Settings, color: 'text-info' },
    { id: 'network', label: 'Network & Permissions', icon: Network, color: 'text-success' },
    { id: 'settings', label: 'Settings & Debug', icon: Cog, color: 'text-neutral' }
  ];
  
  function setActiveView(viewId) {
    activeView = viewId;
    dispatch('viewChange', viewId);
  }
</script>

<!-- Top Navigation Bar -->
<div class="navbar bg-base-300 shadow-lg sticky top-0 z-50">
  <!-- Mobile Menu Button -->
  <div class="navbar-start">
    <div class="dropdown lg:hidden">
      <div tabindex="0" role="button" class="btn btn-ghost">
        <Menu size={20} />
      </div>
      <ul class="menu menu-sm dropdown-content mt-3 z-[1] p-2 shadow bg-base-100 rounded-box w-52">
        {#each navItems as item}
          <li>
            <a 
              href="#{item.id}"
              class="flex items-center gap-2 {activeView === item.id ? 'active' : ''}"
              on:click|preventDefault={() => setActiveView(item.id)}
            >
              <svelte:component this={item.icon} size={16} class={item.color} />
              {item.label}
            </a>
          </li>
        {/each}
      </ul>
    </div>
    
    <!-- Brand -->
    <div class="flex items-center gap-2 ml-2">
      <div class="avatar placeholder">
        <div class="bg-primary text-primary-content rounded-lg w-8">
          <span class="text-lg">🎮</span>
        </div>
      </div>
      <span class="text-xl font-bold hidden sm:block">RoR2 Dev Tool</span>
      <span class="text-lg font-bold sm:hidden">RoR2</span>
    </div>
  </div>

  <!-- Desktop Navigation -->
  <div class="navbar-center hidden lg:flex">
    <ul class="menu menu-horizontal px-1 gap-1">
      {#each navItems as item}
        <li>
          <a 
            href="#{item.id}"
            class="tooltip tooltip-bottom flex items-center gap-2 {activeView === item.id ? 'active' : ''}"
            data-tip={item.label}
            on:click|preventDefault={() => setActiveView(item.id)}
          >
            <svelte:component this={item.icon} size={18} class={item.color} />
            <span class="hidden xl:block">{item.label}</span>
          </a>
        </li>
      {/each}
    </ul>
  </div>

  <!-- Right Side Actions -->
  <div class="navbar-end">
    <!-- Theme Toggle (placeholder for future) -->
    <div class="dropdown dropdown-end">
      <div tabindex="0" role="button" class="btn btn-ghost btn-circle">
        <div class="indicator">
          <Settings size={20} />
        </div>
      </div>
    </div>
    
    <!-- Mobile Drawer Toggle -->
    <label for="drawer-toggle" class="btn btn-ghost btn-circle lg:hidden">
      <Menu size={20} />
    </label>
  </div>
</div>

<!-- DaisyUI handles all the styling -->