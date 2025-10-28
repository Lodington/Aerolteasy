using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using System.Linq;
using UnityEngine;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugInteractableSpawnCardsCommand : BaseCommand
    {
        public override string CommandName => "debuginteractablecards";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("=== Available Interactable Spawn Cards ===");
            
            // Try InteractableSpawnCard directly
            var interactableCards = Resources.LoadAll<InteractableSpawnCard>("")
                .Where(card => card != null && card.name != null)
                .OrderBy(card => card.name)
                .ToList();

            logger.LogInfo($"Found {interactableCards.Count} InteractableSpawnCard resources:");
            foreach (var card in interactableCards)
            {
                var prefabName = card.prefab?.name ?? "null";
                logger.LogInfo($"  - {card.name} (prefab: {prefabName})");
            }
            
            // Try all SpawnCards and filter for InteractableSpawnCard
            var allSpawnCards = Resources.LoadAll<SpawnCard>("")
                .OfType<InteractableSpawnCard>()
                .Where(card => card != null && card.name != null)
                .OrderBy(card => card.name)
                .ToList();

            logger.LogInfo($"Found {allSpawnCards.Count} InteractableSpawnCard from SpawnCard resources:");
            foreach (var card in allSpawnCards)
            {
                var prefabName = card.prefab?.name ?? "null";
                logger.LogInfo($"  - {card.name} (prefab: {prefabName})");
            }
            
            // Also check for any spawn cards with "interactable" in the name
            var allCards = Resources.LoadAll<SpawnCard>("")
                .Where(card => card != null && card.name != null && 
                       card.name.ToLower().Contains("interactable"))
                .OrderBy(card => card.name)
                .ToList();

            logger.LogInfo($"Found {allCards.Count} SpawnCards with 'interactable' in name:");
            foreach (var card in allCards)
            {
                var prefabName = card.prefab?.name ?? "null";
                logger.LogInfo($"  - {card.name} (prefab: {prefabName}) - Type: {card.GetType().Name}");
            }
            
            logger.LogInfo("=== End Interactable Spawn Cards List ===");
        }
    }
}