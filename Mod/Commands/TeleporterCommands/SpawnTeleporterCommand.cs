using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.TeleporterCommands
{
    public class SpawnTeleporterCommand : BaseCommand
    {
        public override string CommandName => "spawnteleporter";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Spawning teleporter");
            SpawnTeleporter(logger);
        }

        private void SpawnTeleporter(ManualLogSource logger)
        {
            // Get spawn position
            var localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser?.cachedBody == null)
            {
                logger.LogWarning("Local player not found for spawn position");
                return;
            }

            var playerPosition = localUser.cachedBody.transform.position;
            var spawnPosition = playerPosition + Vector3.forward * 10f; // Spawn 10 units in front

            // Try to find teleporter spawn card
            InteractableSpawnCard teleporterSpawnCard = null;
            
            foreach (var card in Resources.LoadAll<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard"))
            {
                if (card.name.ToLower().Contains("teleporter"))
                {
                    teleporterSpawnCard = card;
                    break;
                }
            }

            if (teleporterSpawnCard == null)
            {
                logger.LogWarning("Teleporter spawn card not found");
                return;
            }

            // Create spawn request
            var spawnRequest = new DirectorSpawnRequest(teleporterSpawnCard, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                position = spawnPosition,
                minDistance = 0f,
                maxDistance = 0f
            }, RoR2Application.rng);

            // Spawn the teleporter
            DirectorCore.instance?.TrySpawnObject(spawnRequest);
        }
    }
}