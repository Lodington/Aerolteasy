using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class ChangePlayerCommand : BaseCommand
    {
        public override string CommandName => "changeplayer";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var bodyName = GetStringFromCommand(command, "bodyName");
            
            if (string.IsNullOrEmpty(bodyName))
            {
                logger.LogWarning("Body name is required for changeplayer command");
                return;
            }

            logger.LogInfo($"Changing player {playerId} to character: {bodyName}");
            ChangePlayerCharacter(bodyName, playerId, logger);
        }

        private void ChangePlayerCharacter(string bodyName, int playerId, ManualLogSource logger)
        {
            var bodyIndex = BodyCatalog.FindBodyIndex(bodyName);
            if (bodyIndex == BodyIndex.None) 
            {
                logger.LogWarning($"Body not found: {bodyName}");
                return;
            }
            
            if (playerId == -1)
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.cachedBody != null)
                {
                    var master = localUser.cachedMaster;
                    if (master)
                    {
                        master.bodyPrefab = BodyCatalog.GetBodyPrefab(bodyIndex);
                        master.Respawn(localUser.cachedBody.transform.position, localUser.cachedBody.transform.rotation);
                        logger.LogInfo($"Changed local player to {bodyName}");
                    }
                }
            }
            else
            {
                var networkUser = GetNetworkUserById(playerId);
                if (networkUser?.master != null)
                {
                    var currentBody = networkUser.master.GetBody();
                    if (currentBody)
                    {
                        networkUser.master.bodyPrefab = BodyCatalog.GetBodyPrefab(bodyIndex);
                        networkUser.master.Respawn(currentBody.transform.position, currentBody.transform.rotation);
                        logger.LogInfo($"Changed player {playerId} to {bodyName}");
                    }
                }
            }
        }
    }
}