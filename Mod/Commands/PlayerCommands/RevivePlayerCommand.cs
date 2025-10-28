using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class RevivePlayerCommand : BaseCommand
    {
        public override string CommandName => "reviveplayer";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetIntFromCommand(command, "playerId");
            
            logger.LogInfo($"Reviving player {playerId}");
            RevivePlayer(playerId, logger);
        }

        private void RevivePlayer(int playerId, ManualLogSource logger)
        {
            var networkUser = GetNetworkUserById(playerId);
            if (networkUser?.master != null)
            {
                if (networkUser.master.GetBody() == null)
                {
                    networkUser.master.Respawn(Vector3.zero, Quaternion.identity);
                    logger.LogInfo($"Player {playerId} revived");
                }
                else
                {
                    logger.LogInfo($"Player {playerId} is already alive");
                }
            }
            else
            {
                logger.LogWarning($"Player {playerId} not found");
            }
        }
    }
}