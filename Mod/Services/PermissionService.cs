using System;
using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Models;

namespace RoR2DevTool.Services
{
    public class PermissionService
    {
        private readonly ManualLogSource logger;
        private readonly Dictionary<string, PermissionLevel> userPermissions = new Dictionary<string, PermissionLevel>();
        private readonly Dictionary<string, PermissionLevel> commandPermissions = new Dictionary<string, PermissionLevel>();
        private string hostUserId;

        public PermissionService(ManualLogSource logger)
        {
            this.logger = logger;
            InitializeCommandPermissions();
        }

        private void InitializeCommandPermissions()
        {
            // ReadOnly commands
            commandPermissions["refreshstate"] = PermissionLevel.ReadOnly;
            commandPermissions["debugplayeritems"] = PermissionLevel.ReadOnly;
            commandPermissions["debugitems"] = PermissionLevel.ReadOnly;
            commandPermissions["debuginteractables"] = PermissionLevel.ReadOnly;
            commandPermissions["debugmonsters"] = PermissionLevel.ReadOnly;

            // Basic commands
            commandPermissions["spawnitem"] = PermissionLevel.Basic;
            commandPermissions["setmoney"] = PermissionLevel.Basic;
            commandPermissions["sethealth"] = PermissionLevel.Basic;
            commandPermissions["setlevel"] = PermissionLevel.Basic;

            // Advanced commands
            commandPermissions["godmode"] = PermissionLevel.Advanced;
            commandPermissions["changeplayer"] = PermissionLevel.Advanced;
            commandPermissions["teleportplayer"] = PermissionLevel.Advanced;
            commandPermissions["spawnmonster"] = PermissionLevel.Advanced;
            commandPermissions["spawninteractable"] = PermissionLevel.Advanced;
            commandPermissions["givemonsterbuff"] = PermissionLevel.Advanced;
            commandPermissions["givemonsteritem"] = PermissionLevel.Advanced;

            // Admin commands
            commandPermissions["changestage"] = PermissionLevel.Admin;
            commandPermissions["killplayer"] = PermissionLevel.Admin;
            commandPermissions["reviveplayer"] = PermissionLevel.Admin;
            commandPermissions["chargeteleporter"] = PermissionLevel.Admin;
            commandPermissions["activateteleporter"] = PermissionLevel.Admin;
            commandPermissions["skipteleporterevent"] = PermissionLevel.Admin;
            commandPermissions["spawnteleporter"] = PermissionLevel.Admin;
            commandPermissions["setplayerstats"] = PermissionLevel.Admin;

            // ESP commands
            commandPermissions["toggleespoverlay"] = PermissionLevel.ReadOnly;
            commandPermissions["configureespoverlay"] = PermissionLevel.Basic;

            logger.LogInfo($"Initialized {commandPermissions.Count} command permissions");
        }

        public void SetHost(string userId)
        {
            hostUserId = userId;
            userPermissions[userId] = PermissionLevel.Host;
            logger.LogInfo($"Host set: {userId}");
        }

        public bool IsHost(string userId)
        {
            return userId == hostUserId;
        }

        public string GetHostUserId()
        {
            return hostUserId;
        }

        public void SetPermission(string userId, PermissionLevel level)
        {
            if (level == PermissionLevel.Host && !string.IsNullOrEmpty(hostUserId) && userId != hostUserId)
            {
                logger.LogWarning($"Cannot grant Host permission to {userId} - host already exists");
                return;
            }

            userPermissions[userId] = level;
            logger.LogInfo($"Set permission for {userId}: {level}");
        }

        public PermissionLevel GetPermission(string userId)
        {
            if (userPermissions.TryGetValue(userId, out var level))
            {
                return level;
            }
            return PermissionLevel.None;
        }

        public bool HasPermission(string userId, string commandType)
        {
            var userLevel = GetPermission(userId);
            
            if (userLevel == PermissionLevel.None)
                return false;

            if (!commandPermissions.TryGetValue(commandType.ToLower(), out var requiredLevel))
            {
                // Unknown commands require Admin level
                requiredLevel = PermissionLevel.Admin;
            }

            return userLevel >= requiredLevel;
        }

        public Dictionary<string, PermissionLevel> GetAllPermissions()
        {
            return new Dictionary<string, PermissionLevel>(userPermissions);
        }

        public void RevokePermission(string userId)
        {
            if (userId == hostUserId)
            {
                logger.LogWarning("Cannot revoke host permissions");
                return;
            }

            userPermissions.Remove(userId);
            logger.LogInfo($"Revoked permissions for {userId}");
        }

        public void ClearAllPermissions()
        {
            var host = hostUserId;
            userPermissions.Clear();
            if (!string.IsNullOrEmpty(host))
            {
                SetHost(host);
            }
            logger.LogInfo("Cleared all non-host permissions");
        }
    }
}
