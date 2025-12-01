using System;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Models;
using RoR2DevTool.Services;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace RoR2DevTool.Networking
{
    public class DevToolNetworkManager
    {
        private readonly ManualLogSource logger;
        private readonly PermissionService permissionService;
        private readonly CommandProcessor commandProcessor;
        private bool isInitialized = false;

        public DevToolNetworkManager(ManualLogSource logger, PermissionService permissionService, CommandProcessor commandProcessor)
        {
            this.logger = logger;
            this.permissionService = permissionService;
            this.commandProcessor = commandProcessor;
        }

        public void Initialize()
        {
            if (isInitialized)
                return;

            try
            {
                // Register message handlers
                if (NetworkServer.active)
                {
                    // Host/Server handlers
                    NetworkServer.RegisterHandler(DevToolMessageTypes.Command, OnCommandReceived);
                    NetworkServer.RegisterHandler(DevToolMessageTypes.PermissionRequest, OnPermissionRequestReceived);
                    logger.LogInfo("Registered server-side network handlers");

                    // Set host permissions
                    SetHostPermissions();
                }

                if (NetworkClient.active)
                {
                    // Client handlers
                    NetworkManager.singleton.client.RegisterHandler(DevToolMessageTypes.CommandResponse, OnCommandResponseReceived);
                    NetworkManager.singleton.client.RegisterHandler(DevToolMessageTypes.PermissionGrant, OnPermissionGrantReceived);
                    NetworkManager.singleton.client.RegisterHandler(DevToolMessageTypes.PermissionUpdate, OnPermissionUpdateReceived);
                    NetworkManager.singleton.client.RegisterHandler(DevToolMessageTypes.GameStateSync, OnGameStateSyncReceived);
                    logger.LogInfo("Registered client-side network handlers");
                }

                isInitialized = true;
                logger.LogInfo("DevTool network manager initialized");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to initialize network manager: {ex.Message}");
            }
        }

        public void Shutdown()
        {
            if (!isInitialized)
                return;

            try
            {
                // Unregister handlers
                if (NetworkServer.active)
                {
                    NetworkServer.UnregisterHandler(DevToolMessageTypes.Command);
                    NetworkServer.UnregisterHandler(DevToolMessageTypes.PermissionRequest);
                }

                if (NetworkClient.active && NetworkManager.singleton?.client != null)
                {
                    NetworkManager.singleton.client.UnregisterHandler(DevToolMessageTypes.CommandResponse);
                    NetworkManager.singleton.client.UnregisterHandler(DevToolMessageTypes.PermissionGrant);
                    NetworkManager.singleton.client.UnregisterHandler(DevToolMessageTypes.PermissionUpdate);
                    NetworkManager.singleton.client.UnregisterHandler(DevToolMessageTypes.GameStateSync);
                }

                isInitialized = false;
                logger.LogInfo("DevTool network manager shutdown");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during network manager shutdown: {ex.Message}");
            }
        }

        // ========================================================================
        // Server-side handlers (Host)
        // ========================================================================

        private void OnCommandReceived(NetworkMessage netMsg)
        {
            try
            {
                var message = netMsg.ReadMessage<DevToolCommandMessage>();
                logger.LogInfo($"Received command from {message.senderName}: {message.commandType}");

                // Check permissions
                if (!permissionService.HasPermission(message.senderId, message.commandType))
                {
                    logger.LogWarning($"User {message.senderName} lacks permission for {message.commandType}");
                    SendCommandResponse(netMsg.conn, false, $"Insufficient permissions for {message.commandType}", message.commandType);
                    return;
                }

                // Deserialize and execute command
                var command = new DevCommand
                {
                    Type = message.commandType,
                    Data = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(message.commandDataJson)
                };

                commandProcessor.EnqueueCommand(command);
                SendCommandResponse(netMsg.conn, true, "Command executed", message.commandType);

                logger.LogInfo($"Executed command {message.commandType} from {message.senderName}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling command: {ex.Message}");
                SendCommandResponse(netMsg.conn, false, $"Error: {ex.Message}", "unknown");
            }
        }

        private void OnPermissionRequestReceived(NetworkMessage netMsg)
        {
            try
            {
                var message = netMsg.ReadMessage<PermissionRequestMessage>();
                logger.LogInfo($"Permission request from {message.userName}: {message.requestedLevel}");

                // Auto-approve Basic and ReadOnly
                bool autoApprove = message.requestedLevel <= PermissionLevel.Basic;

                if (autoApprove)
                {
                    permissionService.SetPermission(message.userId, message.requestedLevel);
                    SendPermissionGrant(netMsg.conn, message.userId, message.requestedLevel);
                    BroadcastPermissionUpdate(message.userId, message.requestedLevel);
                    logger.LogInfo($"Auto-approved {message.requestedLevel} for {message.userName}");
                }
                else
                {
                    logger.LogInfo($"Permission request for {message.requestedLevel} requires manual approval");
                    // TODO: Queue for manual approval via UI
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling permission request: {ex.Message}");
            }
        }

        // ========================================================================
        // Client-side handlers
        // ========================================================================

        private void OnCommandResponseReceived(NetworkMessage netMsg)
        {
            try
            {
                var message = netMsg.ReadMessage<DevToolCommandResponseMessage>();
                if (message.success)
                {
                    logger.LogInfo($"Command {message.commandType} executed successfully: {message.message}");
                }
                else
                {
                    logger.LogWarning($"Command {message.commandType} failed: {message.message}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling command response: {ex.Message}");
            }
        }

        private void OnPermissionGrantReceived(NetworkMessage netMsg)
        {
            try
            {
                var message = netMsg.ReadMessage<PermissionGrantMessage>();
                permissionService.SetPermission(message.userId, message.grantedLevel);
                logger.LogInfo($"Permission granted: {message.grantedLevel}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling permission grant: {ex.Message}");
            }
        }

        private void OnPermissionUpdateReceived(NetworkMessage netMsg)
        {
            try
            {
                var message = netMsg.ReadMessage<PermissionUpdateMessage>();
                permissionService.SetPermission(message.userId, message.level);
                logger.LogInfo($"Permission updated for {message.userId}: {message.level}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling permission update: {ex.Message}");
            }
        }

        private void OnGameStateSyncReceived(NetworkMessage netMsg)
        {
            try
            {
                var message = netMsg.ReadMessage<GameStateSyncMessage>();
                // TODO: Update local game state cache if needed
                logger.LogInfo("Game state sync received");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling game state sync: {ex.Message}");
            }
        }

        // ========================================================================
        // Sending methods
        // ========================================================================

        public void SendCommand(DevCommand command, string senderId, string senderName)
        {
            if (!NetworkClient.active)
            {
                logger.LogWarning("Cannot send command - not connected to network");
                return;
            }

            try
            {
                var message = new DevToolCommandMessage
                {
                    senderId = senderId,
                    senderName = senderName,
                    commandType = command.Type,
                    commandDataJson = JsonConvert.SerializeObject(command.Data)
                };

                NetworkManager.singleton.client.Send(DevToolMessageTypes.Command, message);
                logger.LogInfo($"Sent command to host: {command.Type}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error sending command: {ex.Message}");
            }
        }

        public void RequestPermission(string userId, string userName, PermissionLevel level)
        {
            if (!NetworkClient.active)
            {
                logger.LogWarning("Cannot request permission - not connected to network");
                return;
            }

            try
            {
                var message = new PermissionRequestMessage
                {
                    userId = userId,
                    userName = userName,
                    requestedLevel = level
                };

                NetworkManager.singleton.client.Send(DevToolMessageTypes.PermissionRequest, message);
                logger.LogInfo($"Sent permission request: {level}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error requesting permission: {ex.Message}");
            }
        }

        private void SendCommandResponse(NetworkConnection conn, bool success, string message, string commandType)
        {
            if (!NetworkServer.active)
                return;

            try
            {
                var response = new DevToolCommandResponseMessage
                {
                    success = success,
                    message = message,
                    commandType = commandType
                };

                NetworkServer.SendToClient(conn.connectionId, DevToolMessageTypes.CommandResponse, response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error sending command response: {ex.Message}");
            }
        }

        private void SendPermissionGrant(NetworkConnection conn, string userId, PermissionLevel level)
        {
            if (!NetworkServer.active)
                return;

            try
            {
                var message = new PermissionGrantMessage
                {
                    userId = userId,
                    grantedLevel = level
                };

                NetworkServer.SendToClient(conn.connectionId, DevToolMessageTypes.PermissionGrant, message);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error sending permission grant: {ex.Message}");
            }
        }

        private void BroadcastPermissionUpdate(string userId, PermissionLevel level)
        {
            if (!NetworkServer.active)
                return;

            try
            {
                var message = new PermissionUpdateMessage
                {
                    userId = userId,
                    level = level
                };

                NetworkServer.SendToAll(DevToolMessageTypes.PermissionUpdate, message);
                logger.LogInfo($"Broadcasted permission update for {userId}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error broadcasting permission update: {ex.Message}");
            }
        }

        // ========================================================================
        // Helper methods
        // ========================================================================

        private void SetHostPermissions()
        {
            try
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.currentNetworkUser != null)
                {
                    string hostId = localUser.currentNetworkUser.id.value.ToString();
                    permissionService.SetHost(hostId);
                    logger.LogInfo($"Set host permissions for {hostId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Could not set host permissions: {ex.Message}");
            }
        }

        public bool IsHost()
        {
            return NetworkServer.active;
        }

        public bool IsClient()
        {
            return NetworkClient.active && !NetworkServer.active;
        }
    }
}
