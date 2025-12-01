using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using BepInEx.Logging;
using Newtonsoft.Json;

namespace RoR2DevTool.Services
{
    public class SSEService
    {
        private readonly ManualLogSource logger;
        private readonly GameStateService gameStateService;
        private readonly PermissionService permissionService;
        private readonly List<HttpListenerResponse> connectedClients = new List<HttpListenerResponse>();
        private readonly object clientLock = new object();
        private Thread updateThread;
        private bool isRunning = false;
        private string lastGameStateJson = "";
        private bool lastInRunState = false;
        private string lastNetworkStatusJson = "";
        private int updateCounter = 0;

        public SSEService(ManualLogSource logger, GameStateService gameStateService, PermissionService permissionService)
        {
            this.logger = logger;
            this.gameStateService = gameStateService;
            this.permissionService = permissionService;
        }

        public void Start()
        {
            isRunning = true;
            updateThread = new Thread(UpdateLoop)
            {
                IsBackground = true
            };
            updateThread.Start();
            logger.LogInfo("SSE Service started");
        }

        public void Stop()
        {
            isRunning = false;
            
            lock (clientLock)
            {
                foreach (var client in connectedClients)
                {
                    try
                    {
                        client.Close();
                    }
                    catch { }
                }
                connectedClients.Clear();
            }
            
            updateThread?.Join(1000);
            logger.LogInfo("SSE Service stopped");
        }

        public void HandleSSEConnection(HttpListenerResponse response)
        {
            try
            {
                // Set SSE headers (CORS headers are already set by HttpServer)
                response.ContentType = "text/event-stream";
                response.Headers.Add("Cache-Control", "no-cache");
                response.Headers.Add("Connection", "keep-alive");
                response.StatusCode = 200;

                // Flush headers to establish connection
                response.OutputStream.Flush();

                lock (clientLock)
                {
                    connectedClients.Add(response);
                    logger.LogInfo($"SSE client connected. Total clients: {connectedClients.Count}");
                }

                // Send initial game state immediately
                SendGameStateToClient(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling SSE connection: {ex.Message}");
                RemoveClient(response);
            }
        }

        private void UpdateLoop()
        {
            while (isRunning)
            {
                try
                {
                    // Get current game state
                    var gameState = gameStateService.GetGameState();
                    string currentJson = JsonConvert.SerializeObject(gameState);

                    // Check if run state changed
                    bool currentInRunState = gameState.IsInRun;
                    if (currentInRunState != lastInRunState)
                    {
                        lastInRunState = currentInRunState;
                        BroadcastStatus(currentInRunState);
                        logger.LogInfo($"Run state changed: {(currentInRunState ? "In Run" : "Not in Run")}");
                    }

                    // Only send game state if it has changed
                    if (currentJson != lastGameStateJson)
                    {
                        lastGameStateJson = currentJson;
                        BroadcastGameState(currentJson);
                    }

                    // Get and broadcast network status (less frequently - every 1 second)
                    updateCounter++;
                    if (updateCounter >= 10) // 10 * 100ms = 1 second
                    {
                        updateCounter = 0;
                        var networkStatus = GetNetworkStatus();
                        string networkJson = JsonConvert.SerializeObject(networkStatus);
                        if (networkJson != lastNetworkStatusJson)
                        {
                            lastNetworkStatusJson = networkJson;
                            BroadcastNetworkStatus(networkJson);
                        }
                    }

                    // Check every 100ms for changes
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error in SSE update loop: {ex.Message}");
                }
            }
        }

        private void BroadcastGameState(string jsonData)
        {
            BroadcastEvent("gamestate", jsonData);
        }

        private void BroadcastStatus(bool isInRun)
        {
            var statusData = JsonConvert.SerializeObject(new
            {
                connected = true,
                gameRunning = true,
                isInRun = isInRun
            });
            BroadcastEvent("status", statusData);
        }

        private void BroadcastNetworkStatus(string jsonData)
        {
            BroadcastEvent("networkstatus", jsonData);
        }

        private object GetNetworkStatus()
        {
            try
            {
                var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
                var networkUser = localUser?.currentNetworkUser;

                bool isMultiplayer = UnityEngine.Networking.NetworkServer.active || UnityEngine.Networking.NetworkClient.active;
                bool isHost = UnityEngine.Networking.NetworkServer.active;
                int playerCount = RoR2.PlayerCharacterMasterController.instances.Count;

                string userId = "N/A";
                string userName = "Unknown";
                var permissionLevel = Models.PermissionLevel.None;

                if (networkUser != null)
                {
                    userId = networkUser.id.value.ToString();
                    userName = networkUser.userName;
                    permissionLevel = permissionService.GetPermission(userId);
                }

                return new
                {
                    isHost,
                    isClient = !isHost && isMultiplayer,
                    isConnected = true,
                    currentUserId = userId,
                    currentUserName = userName,
                    currentPermission = permissionLevel.ToString(),
                    isCurrentUserHost = isHost,
                    connectedPlayers = playerCount,
                    players = GetConnectedPlayers()
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"Error getting network status: {ex.Message}");
                return new
                {
                    isHost = false,
                    isClient = false,
                    isConnected = false,
                    currentUserId = "N/A",
                    currentUserName = "Unknown",
                    currentPermission = "None",
                    isCurrentUserHost = false,
                    connectedPlayers = 0,
                    players = new object[0]
                };
            }
        }

        private object[] GetConnectedPlayers()
        {
            try
            {
                var players = new System.Collections.Generic.List<object>();
                
                foreach (var pcmc in RoR2.PlayerCharacterMasterController.instances)
                {
                    if (pcmc.networkUser != null)
                    {
                        string userId = pcmc.networkUser.id.value.ToString();
                        players.Add(new
                        {
                            userId,
                            userName = pcmc.networkUser.userName,
                            permissionLevel = permissionService.GetPermission(userId).ToString()
                        });
                    }
                }

                return players.ToArray();
            }
            catch
            {
                return new object[0];
            }
        }

        private void BroadcastEvent(string eventType, string jsonData)
        {
            lock (clientLock)
            {
                var disconnectedClients = new List<HttpListenerResponse>();

                foreach (var client in connectedClients)
                {
                    try
                    {
                        SendEventToClient(client, eventType, jsonData);
                    }
                    catch (Exception ex)
                    {
                        // Client disconnected or connection aborted
                        logger.LogWarning($"Client disconnected during {eventType} broadcast: {ex.Message}");
                        disconnectedClients.Add(client);
                    }
                }

                // Remove disconnected clients
                foreach (var client in disconnectedClients)
                {
                    connectedClients.Remove(client);
                    try
                    {
                        client.Close();
                    }
                    catch { }
                    logger.LogInfo($"SSE client removed. Total clients: {connectedClients.Count}");
                }
            }
        }

        private void SendGameStateToClient(HttpListenerResponse client)
        {
            try
            {
                var gameState = gameStateService.GetGameState();
                
                // Send initial status
                var statusData = JsonConvert.SerializeObject(new
                {
                    connected = true,
                    gameRunning = true,
                    isInRun = gameState.IsInRun
                });
                SendEventToClient(client, "status", statusData);
                
                // Send initial game state
                string jsonData = JsonConvert.SerializeObject(gameState);
                SendEventToClient(client, "gamestate", jsonData);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error sending initial game state: {ex.Message}");
                throw;
            }
        }

        private void SendEventToClient(HttpListenerResponse client, string eventType, string data)
        {
            if (!client.OutputStream.CanWrite)
            {
                throw new Exception("Output stream is not writable");
            }

            var stream = client.OutputStream;
            var message = $"event: {eventType}\ndata: {data}\n\n";
            var bytes = Encoding.UTF8.GetBytes(message);
            
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }

        private void RemoveClient(HttpListenerResponse client)
        {
            lock (clientLock)
            {
                if (connectedClients.Contains(client))
                {
                    connectedClients.Remove(client);
                    logger.LogInfo($"SSE client removed. Total clients: {connectedClients.Count}");
                }
            }
        }

        public int GetConnectedClientCount()
        {
            lock (clientLock)
            {
                return connectedClients.Count;
            }
        }
    }
}
