using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Models;
using RoR2DevTool.Services;
using System;

namespace RoR2DevTool.Commands.PermissionCommands
{
    public class GrantPermissionCommand : BaseCommand
    {
        public override string CommandName => "grantpermission";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            string userId = GetStringFromCommand(command, "userId");
            string levelStr = GetStringFromCommand(command, "level");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(levelStr))
            {
                logger.LogWarning("GrantPermission requires userId and level");
                return;
            }

            if (Enum.TryParse<PermissionLevel>(levelStr, true, out var level))
            {
                // This would need access to NetworkingService
                // For now, log the intent
                logger.LogInfo($"Grant {level} permission to user {userId}");
                logger.LogInfo("Use the API endpoint /api/permissions to grant permissions");
            }
            else
            {
                logger.LogWarning($"Invalid permission level: {levelStr}");
            }
        }
    }
}
