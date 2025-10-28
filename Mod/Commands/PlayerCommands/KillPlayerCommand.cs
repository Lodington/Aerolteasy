using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class KillPlayerCommand : BaseCommand
    {
        public override string CommandName => "killplayer";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetIntFromCommand(command, "playerId");
            
            logger.LogInfo($"Killing player {playerId}");
            KillPlayer(playerId, logger);
        }

        private void KillPlayer(int playerId, ManualLogSource logger)
        {
            var networkUser = GetNetworkUserById(playerId);
            if (networkUser?.master?.GetBody() != null)
            {
                var body = networkUser.master.GetBody();
                if (body.healthComponent != null)
                {
                    body.healthComponent.Suicide();
                    logger.LogInfo($"Player {playerId} killed");
                }
                else
                {
                    logger.LogWarning($"Player {playerId} health component not found");
                }
            }
            else
            {
                logger.LogWarning($"Player {playerId} not found or has no body");
            }
        }
    }
}