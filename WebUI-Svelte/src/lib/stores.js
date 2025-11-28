import { writable, derived } from 'svelte/store';

// Game state store
export const gameState = writable({
  Players: [],
  TeamMoney: 0,
  CurrentStage: '',
  StageNumber: 0,
  DifficultyCoefficient: 0,
  GameTime: 0,
  IsInRun: false
});

// Selected player ID for controls
export const selectedPlayerId = writable(null);

// Selected player derived from game state
export const selectedPlayer = derived(
  [gameState, selectedPlayerId],
  ([$gameState, $selectedPlayerId]) => {
    if (!$selectedPlayerId || !$gameState.Players) return null;
    return $gameState.Players.find(p => p.PlayerId === $selectedPlayerId) || null;
  }
);

// Connection status
export const isConnected = writable(false);

// Network status store
export const networkStatus = writable({
  isHost: false,
  isClient: false,
  isConnected: false,
  currentUserId: null,
  currentUserName: null,
  currentPermission: 'None',
  isCurrentUserHost: false,
  connectedPlayers: 0,
  players: []
});

// Current user permission level
export const userPermission = derived(
  networkStatus,
  $networkStatus => $networkStatus.currentPermission
);

// Toast notification store
export const toast = writable({
  show: false,
  message: '',
  type: 'info'
});

// Helper function to show toast
export function showToast(message, type = 'info', duration = 3000) {
  toast.set({ show: true, message, type, duration });
}



// Derived stores for computed values
export const formattedMoney = derived(
  gameState,
  $gameState => `$${$gameState.TeamMoney?.toLocaleString() || '0'}`
);

export const stageDisplay = derived(
  gameState,
  $gameState => `${getStageDisplayName($gameState.CurrentStage)} (Stage ${$gameState.StageNumber})`
);

export const formattedGameTime = derived(
  gameState,
  $gameState => {
    const totalSeconds = Math.floor($gameState.GameTime || 0);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }
);

export const alivePlayers = derived(
  gameState,
  $gameState => $gameState.Players?.filter(p => p.IsAlive) || []
);

export const totalPlayers = derived(
  gameState,
  $gameState => $gameState.Players?.length || 0
);

// Helper functions
export function getCharacterDisplayName(bodyName) {
  const characterNames = {
    'CommandoBody': 'Commando',
    'HuntressBody': 'Huntress',
    'Bandit2Body': 'Bandit',
    'ToolbotBody': 'MUL-T',
    'EngiBody': 'Engineer',
    'MageBody': 'Artificer',
    'MercBody': 'Mercenary',
    'TreebotBody': 'REX',
    'LoaderBody': 'Loader',
    'CrocoBody': 'Acrid',
    'CaptainBody': 'Captain',
    'RailgunnerBody': 'Railgunner',
    'VoidSurvivorBody': 'Void Fiend',
    'SeekerBody': 'Seeker',
    'FalseSonBody': 'False Son',
    'ChefBody': 'Chef'
  };
  return characterNames[bodyName] || bodyName;
}

export function getCharacterIcon(bodyName) {
  const characterIcons = {
    'CommandoBody': '🔫',
    'HuntressBody': '🏹',
    'Bandit2Body': '🔪',
    'ToolbotBody': '🤖',
    'EngiBody': '🔧',
    'MageBody': '🔮',
    'MercBody': '⚔️',
    'TreebotBody': '🌱',
    'LoaderBody': '👊',
    'CrocoBody': '🦎',
    'CaptainBody': '⚓',
    'RailgunnerBody': '🎯',
    'VoidSurvivorBody': '🕳️',
    'SeekerBody': '👁️',
    'FalseSonBody': '👤',
    'ChefBody': '👨‍🍳'
  };
  return characterIcons[bodyName] || '👤';
}

export function getStageDisplayName(stageName) {
  const stageNames = {
    'golemplains': 'Titanic Plains',
    'blackbeach': 'Distant Roost',
    'snowyforest': 'Siphoned Forest',
    'goolake': 'Abandoned Aqueduct',
    'foggyswamp': 'Wetland Aspect',
    'frozenwall': 'Rallypoint Delta',
    'wispgraveyard': 'Scorched Acres',
    'sulfurpools': 'Sulfur Pools',
    'dampcavesimple': 'Abyssal Depths',
    'shipgraveyard': "Siren's Call",
    'skymeadow': 'Sky Meadow',
    'moon2': 'Commencement'
  };
  return stageNames[stageName] || stageName;
}

export function getItemDisplayName(itemName) {
  const itemNames = {
    'Syringe': "Soldier's Syringe",
    'Bear': 'Tougher Times',
    'Crowbar': 'Crowbar',
    'Tooth': 'Monster Tooth',
    'Hoof': "Paul's Goat Hoof",
    'Mushroom': 'Bustling Fungus',
    'CritGlasses': "Lens-Maker's Glasses",
    'Feather': 'Hopoo Feather',
    'Whip': 'Elemental Band',
    'ChainLightning': 'Ukulele',
    'BehemothBody': 'Brilliant Behemoth',
    'Missile': 'AtG Missile Mk. 1',
    'ExtraLife': "Dio's Best Friend",
    'Jetpack': 'H3AD-5T v2',
    'Bandolier': 'Bandolier',
    'BounceNearby': 'Sentient Meat Hook',
    'Dagger': 'Ceremonial Dagger',
    'BeetleGland': "Queen's Gland",
    'ExtraLifeConsumed': '57 Leaf Clover',
    'Knurl': 'Titanic Knurl',
    'BFG': 'Preon Accumulator',
    'Behemoth': 'Brilliant Behemoth'
  };
  return itemNames[itemName] || itemName;
}