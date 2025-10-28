using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class SetLevelCommand : BaseCommand
    {
        public override string CommandName => "setlevel";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var level = GetIntFromCommand(command, "level");
            
            logger.LogInfo($"Setting level for player {playerId} to {level}");
            SetLevel(level, playerId, logger);
        }

        private void SetLevel(int level, int playerId, ManualLogSource logger)
        {
            if (playerId == -1)
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.cachedBody != null)
                {
                    var characterMaster = localUser.cachedMaster;
                    if (characterMaster)
                    {
                        TeamManager.instance.SetTeamLevel(characterMaster.teamIndex, (uint)level);
                        logger.LogInfo($"Set local player level to {level}");
                    }
                }
                else
                {
                    logger.LogWarning("Local player not found");
                }
            }
            else
            {
                var networkUser = GetNetworkUserById(playerId);
                if (networkUser?.master != null)
                {
                    TeamManager.instance.SetTeamLevel(networkUser.master.teamIndex, (uint)level);
                    logger.LogInfo($"Set player {playerId} level to {level}");
                }
                else
                {
                    logger.LogWarning($"Player {playerId} not found");
                }
            }
        }
    }
}