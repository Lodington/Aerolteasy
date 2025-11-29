// Detect if running in Tauri (production) or browser (development)
const isTauri = window.__TAURI__ !== undefined;
const API_BASE_URL = isTauri 
  ? 'http://localhost:8080/api'  // Production: Tauri app on port 1420, connects to game on 8080
  : 'http://localhost:8080/api'; // Development: Vite on port 5173, connects to game on 8080

export class NetworkAPI {
  constructor() {
    this.baseUrl = API_BASE_URL;
    console.log(`Network API initialized in ${isTauri ? 'production (Tauri)' : 'development (Vite)'} mode`);
  }

  // Get network status and connected players
  async getNetworkStatus() {
    try {
      const response = await fetch(`${this.baseUrl}/network/status`);
      if (response.ok) {
        return await response.json();
      }
    } catch (error) {
      console.error('Failed to fetch network status:', error);
    }
    return null;
  }

  // Get all user permissions
  async getPermissions() {
    try {
      const response = await fetch(`${this.baseUrl}/permissions`);
      if (response.ok) {
        return await response.json();
      }
    } catch (error) {
      console.error('Failed to fetch permissions:', error);
    }
    return null;
  }

  // Request permission (client)
  async requestPermission(level) {
    try {
      const response = await fetch(`${this.baseUrl}/permissions/request`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ level })
      });
      return await response.json();
    } catch (error) {
      console.error('Failed to request permission:', error);
      throw error;
    }
  }

  // Grant permission (host only)
  async grantPermission(userId, level) {
    try {
      const response = await fetch(`${this.baseUrl}/permissions`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId, level })
      });
      return await response.json();
    } catch (error) {
      console.error('Failed to grant permission:', error);
      throw error;
    }
  }

  // Revoke permission (host only)
  async revokePermission(userId) {
    try {
      const response = await fetch(`${this.baseUrl}/permissions?userId=${userId}`, {
        method: 'DELETE'
      });
      return await response.json();
    } catch (error) {
      console.error('Failed to revoke permission:', error);
      throw error;
    }
  }
}

export const networkApi = new NetworkAPI();

// Permission level constants
export const PermissionLevels = {
  NONE: 'None',
  READ_ONLY: 'ReadOnly',
  BASIC: 'Basic',
  ADVANCED: 'Advanced',
  ADMIN: 'Admin',
  HOST: 'Host'
};

// Command permission requirements
export const COMMAND_PERMISSIONS = {
  // ReadOnly
  'refreshstate': 'ReadOnly',
  'debugplayeritems': 'ReadOnly',
  'debugitems': 'ReadOnly',
  'debuginteractables': 'ReadOnly',
  'debugmonsters': 'ReadOnly',
  
  // Basic
  'spawnitem': 'Basic',
  'setmoney': 'Basic',
  'sethealth': 'Basic',
  'setlevel': 'Basic',
  'configureespoverlay': 'Basic',
  'toggleespoverlay': 'Basic',
  
  // Advanced
  'godmode': 'Advanced',
  'changeplayer': 'Advanced',
  'teleportplayer': 'Advanced',
  'spawnmonster': 'Advanced',
  'spawninteractable': 'Advanced',
  'givemonsterbuff': 'Advanced',
  'givemonsteritem': 'Advanced',
  
  // Admin
  'changestage': 'Admin',
  'killplayer': 'Admin',
  'reviveplayer': 'Admin',
  'chargeteleporter': 'Admin',
  'activateteleporter': 'Admin',
  'skipteleporterevent': 'Admin',
  'spawnteleporter': 'Admin',
  'setplayerstats': 'Admin'
};

// Check if user has permission for a command
export function hasPermissionForCommand(userPermission, commandType) {
  const required = COMMAND_PERMISSIONS[commandType.toLowerCase()] || 'Admin';
  const levels = ['None', 'ReadOnly', 'Basic', 'Advanced', 'Admin', 'Host'];
  const userLevel = levels.indexOf(userPermission);
  const requiredLevel = levels.indexOf(required);
  return userLevel >= requiredLevel;
}

// Get permission level display info
export function getPermissionInfo(level) {
  const info = {
    'None': { color: 'badge-ghost', icon: '🚫', description: 'No permissions' },
    'ReadOnly': { color: 'badge-info', icon: '👁️', description: 'View only' },
    'Basic': { color: 'badge-success', icon: '✅', description: 'Basic commands' },
    'Advanced': { color: 'badge-warning', icon: '⚡', description: 'Advanced commands' },
    'Admin': { color: 'badge-error', icon: '⚠️', description: 'Admin commands' },
    'Host': { color: 'badge-primary', icon: '👑', description: 'Full control' }
  };
  return info[level] || info['None'];
}
