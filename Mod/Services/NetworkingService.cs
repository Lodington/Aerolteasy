using System;
using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Models;
using RoR2DevTool.Networking;
using UnityEngine.Networking;

namespace RoR2DevTool.Services
{
    public class NetworkingService
    {
        private readonly ManualLogSource logger;
        private readonly PermissionService permissionService;
        private readonly CommandProcessor commandProcessor;
        private readonly DevToolNetworkManager networkManager;
        private bool isInitialized = false;

        public NetworkingService(ManualLogSource logger, PermissionService permissionService, CommandProcessor commandProcessor)
        {
            this.logger = logger;
            this.permissionService = permissionService;
            this.commandProcessor = commandProcessor;
            this.networkManager = new DevToolNetworkManager(logger, permissionService, commandProcessor);
        }

        public void Initialize()
        {
            if (isInitialized)
                return;

            try
            {
                // Initialize the network manager
                networkManager.Initialize();

                isInitialized = true;
                logger.LogInfo("NetworkingService initialized with full multiplayer support");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to initialize NetworkingService: {ex.Message}");
            }
        }

        public void Shutdown()
        {
            if (!isInitialized)
                return;

            try
            {
                networkManager.Shutdown();
                isInitialized = false;
                logger.LogInfo("NetworkingService shutdown");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during NetworkingService shutdown: {ex.Message}");
            }
        }

        public void SendCommand(DevCommand command, string senderId, string senderName)
        {
            if (networkManager.IsHost())
            {
                // We're the host, check permissions and execute locally
                if (permissionService.HasPermission(senderId, command.Type))
                {
                    logger.LogInfo($"[Host] Executing command from {senderName}: {command.Type}");
                    commandProcessor.EnqueueCommand(command);
                }
                else
                {
                    logger.LogWarning($"[Host] User {senderName} ({senderId}) lacks permission for command: {command.Type}");
                }
            }
            else if (networkManager.IsClient())
            {
                // We're a client, send command to host for validation and execution
                logger.LogInfo($"[Client] Sending command to host: {command.Type}");
                networkManager.SendCommand(command, senderId, senderName);
            }
            else
            {
                // Single player, execute locally
                logger.LogInfo($"[SinglePlayer] Executing command locally: {command.Type}");
                commandProcessor.EnqueueCommand(command);
            }
        }



        public void RequestPermission(PermissionLevel level)
        {
            var localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser?.currentNetworkUser == null)
            {
                logger.LogWarning("No local user found");
                return;
            }

            string userId = GetUserNetworkId(localUser.currentNetworkUser);
            string userName = localUser.currentNetworkUser.userName;

            if (networkManager.IsHost())
            {
                // Host can grant themselves any permission
                permissionService.SetPermission(userId, level);
                logger.LogInfo($"[Host] Granted self {level} permission");
            }
            else if (networkManager.IsClient())
            {
                // Client must request from host
                logger.LogInfo($"[Client] Requesting {level} permission from host");
                networkManager.RequestPermission(userId, userName, level);
            }
            else
            {
                // Single player - auto-approve
                permissionService.SetPermission(userId, level);
                logger.LogInfo($"[SinglePlayer] Auto-approved {level} permission");
            }
        }

        public void GrantPermission(string userId, PermissionLevel level)
        {
            if (!NetworkServer.active && NetworkClient.active)
            {
                logger.LogWarning("Only host can grant permissions");
                return;
            }

            permissionService.SetPermission(userId, level);
            logger.LogInfo($"Granted {level} permission to {userId}");
        }

        private string GetUserNetworkId(NetworkUser networkUser)
        {
            // Use Steam ID or network ID as unique identifier
            return networkUser.id.value.ToString();
        }
    }
}
