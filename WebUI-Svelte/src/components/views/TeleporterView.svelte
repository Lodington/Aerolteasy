<script>
  import { api } from "../../lib/api.js";
  import StageSelection from "../teleporter/StageSelection.svelte";
  import StageControls from "../teleporter/StageControls.svelte";
  import TeleporterControls from "../teleporter/TeleporterControls.svelte";

  // Stage search and selection
  let stageSearch = "";
  let selectedStage = null;
  let isChangingStage = false;
  let statusMessage = "";

  // Teleporter controls
  let isProcessing = false;

  // Available stages organized by category
  const stages = [
    {
      category: "Stage 1 - Early Game",
      stages: [
        {
          value: "golemplains",
          label: "Titanic Plains",
          description: "Grassy plains with stone golems",
          emoji: "🌾",
          difficulty: "Easy",
        },
        {
          value: "blackbeach",
          label: "Distant Roost",
          description: "Coastal cliffs with flying enemies",
          emoji: "🏖️",
          difficulty: "Easy",
        },
        {
          value: "snowyforest",
          label: "Siphoned Forest",
          description: "Snowy forest with void corruption",
          emoji: "❄️",
          difficulty: "Easy",
        },
      ],
    },
    {
      category: "Stage 2 - Mid Game",
      stages: [
        {
          value: "goolake",
          label: "Abandoned Aqueduct",
          description: "Ancient aqueduct with tar pools",
          emoji: "🏛️",
          difficulty: "Medium",
        },
        {
          value: "foggyswamp",
          label: "Wetland Aspect",
          description: "Misty swampland with mushrooms",
          emoji: "🌫️",
          difficulty: "Medium",
        },
        {
          value: "frozenwall",
          label: "Rallypoint Delta",
          description: "Frozen military outpost",
          emoji: "🧊",
          difficulty: "Medium",
        },
      ],
    },
    {
      category: "Stage 3 - Advanced",
      stages: [
        {
          value: "wispgraveyard",
          label: "Scorched Acres",
          description: "Burned wasteland with fire enemies",
          emoji: "🔥",
          difficulty: "Hard",
        },
        {
          value: "sulfurpools",
          label: "Sulfur Pools",
          description: "Toxic pools with acid hazards",
          emoji: "☠️",
          difficulty: "Hard",
        },
      ],
    },
    {
      category: "Stage 4 - Late Game",
      stages: [
        {
          value: "dampcavesimple",
          label: "Abyssal Depths",
          description: "Deep underground caverns",
          emoji: "🕳️",
          difficulty: "Very Hard",
        },
        {
          value: "shipgraveyard",
          label: "Siren's Call",
          description: "Shipwreck with siren enemies",
          emoji: "🚢",
          difficulty: "Very Hard",
        },
      ],
    },
    {
      category: "Stage 5 - Final Approach",
      stages: [
        {
          value: "skymeadow",
          label: "Sky Meadow",
          description: "Floating islands in the sky",
          emoji: "☁️",
          difficulty: "Extreme",
        },
      ],
    },
    {
      category: "Final Stage",
      stages: [
        {
          value: "moon2",
          label: "Commencement",
          description: "The final battle on the moon",
          emoji: "🌙",
          difficulty: "Ultimate",
        },
      ],
    },
    {
      category: "Special Stages",
      stages: [
        {
          value: "goldshores",
          label: "Gilded Coast",
          description: "Golden beach with Aurelionite",
          emoji: "🏆",
          difficulty: "Special",
        },
        {
          value: "mysteryspace",
          label: "A Moment, Fractured",
          description: "Mysterious void space",
          emoji: "🌌",
          difficulty: "Special",
        },
        {
          value: "bazaar",
          label: "Bazaar Between Time",
          description: "Lunar shop dimension",
          emoji: "🛒",
          difficulty: "Peaceful",
        },
        {
          value: "arena",
          label: "Void Fields",
          description: "Arena combat challenges",
          emoji: "⚔️",
          difficulty: "Challenge",
        },
      ],
    },
  ];



  async function changeStage() {
    if (!selectedStage || isChangingStage) return;

    isChangingStage = true;
    statusMessage = "Changing stage...";

    try {
      await api.sendCommand({
        Type: "changestage",
        Data: { stageName: selectedStage.value },
      });
      statusMessage = `Changed to ${selectedStage.label}! Check the game.`;
      setTimeout(() => {
        statusMessage = "";
      }, 4000);
    } catch (error) {
      console.error("Failed to change stage:", error);
      statusMessage = "Failed to change stage. Check console for details.";
      setTimeout(() => {
        statusMessage = "";
      }, 4000);
    } finally {
      isChangingStage = false;
    }
  }

  // Teleporter control functions
  async function chargeTeleporter() {
    if (isProcessing) return;

    isProcessing = true;
    statusMessage = "Charging teleporter...";

    try {
      await api.sendCommand({
        Type: "chargeteleporter",
        Data: {},
      });
      statusMessage = "Teleporter charged to 100%!";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } catch (error) {
      console.error("Failed to charge teleporter:", error);
      statusMessage = "Failed to charge teleporter.";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }

  async function activateTeleporter() {
    if (isProcessing) return;

    isProcessing = true;
    statusMessage = "Activating teleporter...";

    try {
      await api.sendCommand({
        Type: "activateteleporter",
        Data: {},
      });
      statusMessage = "Teleporter activated!";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } catch (error) {
      console.error("Failed to activate teleporter:", error);
      statusMessage = "Failed to activate teleporter.";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }

  async function skipTeleporterEvent() {
    if (isProcessing) return;

    isProcessing = true;
    statusMessage = "Skipping teleporter event...";

    try {
      await api.sendCommand({
        Type: "skipteleporterevent",
        Data: {},
      });
      statusMessage = "Teleporter event skipped!";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } catch (error) {
      console.error("Failed to skip teleporter event:", error);
      statusMessage = "Failed to skip teleporter event.";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }

  async function spawnTeleporter() {
    if (isProcessing) return;

    isProcessing = true;
    statusMessage = "Spawning teleporter...";

    try {
      await api.sendCommand({
        Type: "spawnteleporter",
        Data: {},
      });
      statusMessage = "Teleporter spawned at your location!";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } catch (error) {
      console.error("Failed to spawn teleporter:", error);
      statusMessage = "Failed to spawn teleporter.";
      setTimeout(() => {
        statusMessage = "";
      }, 3000);
    } finally {
      isProcessing = false;
    }
  }
</script>

<div class="container mx-auto p-4 sm:p-6 space-y-4 sm:space-y-6">
  <div class="text-center mb-6 sm:mb-8">
    <h1 class="text-2xl sm:text-3xl font-bold mb-2">🌀 Teleporter & Stage Management</h1>
    <p class="text-sm sm:text-base text-base-content/70 px-4">
      Manage teleporter state and stage progression with advanced controls
    </p>
  </div>

  <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
    <!-- Stage Selection Component -->
    <div class="xl:col-span-2">
      <StageSelection 
        bind:stageSearch
        bind:selectedStage
        {stages}
      />
    </div>

    <!-- Controls Column -->
    <div class="space-y-6">
      <!-- Stage Controls Component -->
      <StageControls
        {selectedStage}
        {isChangingStage}
        {statusMessage}
        onChangeStage={changeStage}
      />

      <!-- Teleporter Controls Component -->
      <TeleporterControls
        {isProcessing}
        {statusMessage}
        onChargeTeleporter={chargeTeleporter}
        onActivateTeleporter={activateTeleporter}
        onSkipTeleporterEvent={skipTeleporterEvent}
        onSpawnTeleporter={spawnTeleporter}
      />
    </div>
  </div>
</div>
