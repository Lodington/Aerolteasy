using System;
using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Models;
using UnityEngine.Networking;

namespace RoR2DevTool.Services
{
    public class NetworkingService
    {
        private readonly ManualLogSource logger;
        private readonly PermissionService permissionService;
        private readonly CommandProcessor commandProcessor;
        private bool isInitialized = false;

        // Network message types
        private const short MSG_COMMAND = 1000;
        private const short MSG_PERMISSION_REQUEST = 1001;
        private const short MSG_PERMISSION_RESPONSE = 1002;
        private const short MSG_PERMISSION_UPDATE = 1003;

        public NetworkingService(ManualLogSource logger, PermissionService permissionService, CommandProcessor commandProcessor)
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
                // Set host if we're the server
                if (NetworkServer.active)
                {
                    var localUser = LocalUserManager.GetFirstLocalUser();
                    if (localUser != null && localUser.currentNetworkUser != null)
                    {
                        string hostId = GetUserNetworkId(localUser.currentNetworkUser);
                        permissionService.SetHost(hostId);
                        logger.LogInfo($"Host permissions set for user: {hostId}");
                    }
                }

                isInitialized = true;
                logger.LogInfo("NetworkingService initialized");
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
            // Simplified: Just check permissions and execute locally
            // In a real multiplayer scenario, you would use RoR2's networking system
            // or a custom networking solution like R2API
            
            if (NetworkServer.active)
            {
                // We're the host, check permissions and execute
                if (permissionService.HasPermission(senderId, command.Type))
                {
                    logger.LogInfo($"Executing command from {senderName}: {command.Type}");
                    commandProcessor.EnqueueCommand(command);
                }
                else
                {
                    logger.LogWarning($"User {senderName} ({senderId}) lacks permission for command: {command.Type}");
                }
            }
            else
            {
                // We're a client or single player, execute locally
                logger.LogInfo($"Executing command locally: {command.Type}");
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
            
            // Simplified: Auto-approve Basic and ReadOnly, log others for manual approval
            bool autoApprove = level <= PermissionLevel.Basic;

            if (autoApprove)
            {
                permissionService.SetPermission(userId, level);
                logger.LogInfo($"Auto-approved {level} permission for {localUser.currentNetworkUser.userName}");
            }
            else
            {
                logger.LogInfo($"Permission request for {level} from {localUser.currentNetworkUser.userName} requires manual approval");
                logger.LogInfo($"Use the API to grant: POST /api/permissions with userId={userId} and level={level}");
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
