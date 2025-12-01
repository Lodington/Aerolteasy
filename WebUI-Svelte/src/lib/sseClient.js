// SSE Client for real-time game state updates

const API_BASE_URL = 'http://localhost:8080/api';

export class SSEClient {
  constructor() {
    this.eventSource = null;
    this.onGameStateUpdate = null;
    this.onConnectionChange = null;
    this.isConnected = false;
  }

  /**
   * Connect to SSE endpoint
   * @param {Function} onGameStateUpdate - Callback when game state updates
   * @param {Function} onConnectionChange - Callback when connection status changes
   */
  connect(onGameStateUpdate, onConnectionChange) {
    this.onGameStateUpdate = onGameStateUpdate;
    this.onConnectionChange = onConnectionChange;

    try {
      console.log('[SSE] Connecting to event stream...');
      
      this.eventSource = new EventSource(`${API_BASE_URL}/events`);

      this.eventSource.addEventListener('gamestate', (event) => {
        try {
          const gameState = JSON.parse(event.data);
          if (this.onGameStateUpdate) {
            this.onGameStateUpdate(gameState);
          }
        } catch (error) {
          console.error('[SSE] Error parsing game state:', error);
        }
      });

      this.eventSource.addEventListener('status', (event) => {
        try {
          const status = JSON.parse(event.data);
          if (this.onConnectionChange) {
            this.onConnectionChange(status.connected && status.gameRunning);
          }
        } catch (error) {
          console.error('[SSE] Error parsing status:', error);
        }
      });

      this.eventSource.addEventListener('networkstatus', (event) => {
        try {
          const networkStatus = JSON.parse(event.data);
          if (typeof window !== 'undefined') {
            window.dispatchEvent(new CustomEvent('networkstatus', { detail: networkStatus }));
          }
        } catch (error) {
          console.error('[SSE] Error parsing network status:', error);
        }
      });

      this.eventSource.onopen = () => {
        console.log('[SSE] Connection established');
        this.isConnected = true;
        
        if (this.onConnectionChange) {
          this.onConnectionChange(true);
        }
      };

      this.eventSource.onerror = (error) => {
        console.error('[SSE] Connection error:', error);
        this.isConnected = false;
        
        if (this.onConnectionChange) {
          this.onConnectionChange(false);
        }

        // EventSource will automatically try to reconnect
        console.log('[SSE] Will attempt to reconnect...');
      };

    } catch (error) {
      console.error('[SSE] Failed to create EventSource:', error);
      this.isConnected = false;
      
      if (this.onConnectionChange) {
        this.onConnectionChange(false);
      }
    }
  }

  /**
   * Disconnect from SSE endpoint
   */
  disconnect() {
    if (this.eventSource) {
      console.log('[SSE] Disconnecting...');
      this.eventSource.close();
      this.eventSource = null;
      this.isConnected = false;
      
      if (this.onConnectionChange) {
        this.onConnectionChange(false);
      }
    }
  }

  /**
   * Check if currently connected
   * @returns {boolean}
   */
  getConnectionStatus() {
    return this.isConnected && this.eventSource?.readyState === EventSource.OPEN;
  }

  /**
   * Get detailed connection info for debugging
   * @returns {Object}
   */
  getDebugInfo() {
    return {
      isConnected: this.isConnected,
      readyState: this.eventSource?.readyState,
      readyStateText: this.eventSource?.readyState === 0 ? 'CONNECTING' :
                      this.eventSource?.readyState === 1 ? 'OPEN' :
                      this.eventSource?.readyState === 2 ? 'CLOSED' : 'UNKNOWN',
      url: this.eventSource?.url
    };
  }
}

export const sseClient = new SSEClient();
