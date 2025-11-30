// Detect if running in Tauri (production) or browser (development)
const isTauri = window.__TAURI__ !== undefined;
const API_BASE_URL = isTauri 
  ? 'http://localhost:8080/api'  // Production: Tauri app on port 1420, connects to game on 8080
  : 'http://localhost:8080/api'; // Development: Vite on port 5173, connects to game on 8080

export class RoR2API {
  constructor() {
    this.baseUrl = API_BASE_URL;
    console.log(`API initialized in ${isTauri ? 'production (Tauri)' : 'development (Vite)'} mode`);
    console.log(`Connecting to game server at: ${this.baseUrl}`);
  }

  async getGameState() {
    try {
      const response = await fetch(`${this.baseUrl}/gamestate`);
      if (response.ok) {
        return await response.json();
      }
    } catch (error) {
      console.error('Failed to fetch game state:', error);
    }
    return null;
  }

  async sendCommand(command) {
    try {
      const response = await fetch(`${this.baseUrl}/command`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(command)
      });

      const result = await response.json();

      if (response.status === 403) {
        // Permission denied
        throw new Error(`Insufficient permissions: ${result.message || 'Permission denied'}`);
      }

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return result;
    } catch (error) {
      console.error('Error sending command:', error);
      throw error;
    }
  }

  // Helper methods for multi-player commands
  async toggleGodMode(playerId = -1, enabled = true) {
    return this.sendCommand({
      Type: 'godmode',
      Data: { playerId, enabled }
    });
  }

  async changeCharacter(bodyName, playerId = -1) {
    return this.sendCommand({
      Type: 'changeplayer',
      Data: { bodyName, playerId }
    });
  }

  async spawnItem(itemName, count, playerId = -1) {
    return this.sendCommand({
      Type: 'spawnitem',
      Data: { itemName, count, playerId }
    });
  }

  async setHealth(percentage, playerId = -1) {
    return this.sendCommand({
      Type: 'sethealth',
      Data: { percentage, playerId }
    });
  }

  async setLevel(level, playerId = -1) {
    return this.sendCommand({
      Type: 'setlevel',
      Data: { level, playerId }
    });
  }

  async setMoney(amount) {
    return this.sendCommand({
      Type: 'setmoney',
      Data: { amount }
    });
  }

  async killPlayer(playerId) {
    return this.sendCommand({
      Type: 'killplayer',
      Data: { playerId }
    });
  }

  async revivePlayer(playerId) {
    return this.sendCommand({
      Type: 'reviveplayer',
      Data: { playerId }
    });
  }

  async debugItems() {
    return this.sendCommand({
      Type: 'debugitems',
      Data: {}
    });
  }

  async refreshGameState() {
    return this.sendCommand({
      Type: 'refreshstate',
      Data: {}
    });
  }

  async debugPlayerItems(playerId) {
    return this.sendCommand({
      Type: 'debugplayeritems',
      Data: { playerId }
    });
  }



  async changeStage(stageName) {
    return this.sendCommand({
      Type: 'changestage',
      Data: { stageName }
    });
  }

  async checkStatus() {
    try {
      console.log(`[API] Checking status at: ${this.baseUrl}/status`);
      const response = await fetch(`${this.baseUrl}/status`);
      console.log(`[API] Status response:`, response.status, response.ok);
      
      if (response.ok) {
        const status = await response.json();
        console.log(`[API] Status data:`, status);
        return status.connected && status.gameRunning;
      }
    } catch (error) {
      console.error('[API] Connection check failed:', error.message);
      console.error('[API] Full error:', error);
    }
    return false;
  }

  async getItemCatalog() {
    try {
      const response = await fetch(`${this.baseUrl}/itemcatalog`);
      if (response.ok) {
        const data = await response.json();
        return data.items || [];
      }
    } catch (error) {
      console.error('Failed to fetch item catalog:', error);
    }
    return [];
  }

  async refreshItemCatalog() {
    return this.sendCommand({
      Type: 'debugitemcatalog',
      Data: {}
    });
  }

  async setPlayerStat(statType, value, playerId = -1) {
    return this.sendCommand({
      Type: 'setplayerstats',
      Data: { statType, value, playerId }
    });
  }

  async getCharacterDefaults() {
    try {
      const response = await fetch(`${this.baseUrl}/characterdefaults`);
      if (response.ok) {
        const data = await response.json();
        return data.characters || [];
      }
    } catch (error) {
      console.error('Failed to fetch character defaults:', error);
    }
    return [];
  }

  async refreshCharacterDefaults() {
    return this.sendCommand({
      Type: 'debugcharacterdefaults',
      Data: {}
    });
  }

  async debugESPData() {
    return this.sendCommand({
      Type: 'debugespdata',
      Data: {}
    });
  }

  async testESPData() {
    return this.sendCommand({
      Type: 'testespdata',
      Data: {}
    });
  }

  async teleportPlayer(x, y, z, playerId = -1, yOffset = 2.5) {
    return this.sendCommand({
      Type: 'teleportplayer',
      Data: { x, y, z, playerId, yOffset }
    });
  }

  // ESP Overlay Controls
  async toggleESPOverlay(overlayType = 'all', enabled = true) {
    return this.sendCommand({
      Type: 'toggleespoverlay',
      Data: { overlayType, enabled }
    });
  }

  async configureESPOverlay(config) {
    return this.sendCommand({
      Type: 'configureespoverlay',
      Data: config
    });
  }

  async testESPOverlay() {
    return this.sendCommand({
      Type: 'testespoverlay',
      Data: {}
    });
  }

  async disableESPOverlay() {
    return this.sendCommand({
      Type: 'disableespoverlay',
      Data: {}
    });
  }
}

export const api = new RoR2API();