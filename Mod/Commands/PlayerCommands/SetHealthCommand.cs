using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class SetHealthCommand : BaseCommand
    {
        public override string CommandName => "sethealth";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var percentage = GetFloatFromCommand(command, "percentage");
            
            logger.LogInfo($"Setting health for player {playerId} to {percentage}%");
            SetHealth(percentage, playerId, logger);
        }

        private void SetHealth(float percentage, int playerId, ManualLogSource logger)
        {
            if (playerId == -1)
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.cachedBody?.healthComponent != null)
                {
                    var healthComponent = localUser.cachedBody.healthComponent;
                    healthComponent.health = healthComponent.fullHealth * (percentage / 100f);
                    logger.LogInfo($"Set local player health to {percentage}%");
                }
                else
                {
                    logger.LogWarning("Local player health component not found");
                }
            }
            else
            {
                var networkUser = GetNetworkUserById(playerId);
                if (networkUser?.master?.GetBody()?.healthComponent != null)
                {
                    var healthComponent = networkUser.master.GetBody().healthComponent;
                    healthComponent.health = healthComponent.fullHealth * (percentage / 100f);
                    logger.LogInfo($"Set player {playerId} health to {percentage}%");
                }
                else
                {
                    logger.LogWarning($"Player {playerId} health component not found");
                }
            }
        }
    }
}