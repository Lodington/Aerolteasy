using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugInteractablesCommand : BaseCommand
    {
        public override string CommandName => "debuginteractables";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Debug all interactables requested");
            LogAllAvailableInteractables(logger);
        }

        private void LogAllAvailableInteractables(ManualLogSource logger)
        {
            logger.LogInfo("=== Available Interactable Spawn Cards ===");
            
            var allCards = Resources.LoadAll<InteractableSpawnCard>("");
            logger.LogInfo($"Found {allCards.Length} interactable spawn cards:");
            
            foreach (var card in allCards)
            {
                var prefabName = card.prefab?.name ?? "null";
                logger.LogInfo($"Card: {card.name} | Prefab: {prefabName}");
            }
            
            logger.LogInfo("=== End Available Interactables ===");
        }
    }
}