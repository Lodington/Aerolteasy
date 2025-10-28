using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2DevTool.Commands.SpawningCommands
{
    public class SpawnInteractableCommand : BaseCommand
    {
        public override string CommandName => "spawninteractable";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var interactableName = GetStringFromCommand(command, "interactableName");
            var distance = GetFloatFromCommand(command, "distance", 5f);
            
            if (string.IsNullOrEmpty(interactableName))
            {
                logger.LogWarning("Interactable name is required for spawninteractable command");
                return;
            }

            logger.LogInfo($"Spawning {interactableName} at distance {distance}");
            SpawnInteractable(interactableName, distance, logger);
        }

        private void SpawnInteractable(string interactableName, float distance, ManualLogSource logger)
        {
            // Get spawn position
            var localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser?.cachedBody == null)
            {
                logger.LogWarning("Local player not found for spawn position");
                return;
            }

            var playerPosition = localUser.cachedBody.transform.position;
            var playerForward = localUser.cachedBody.transform.forward;
            var spawnPosition = playerPosition + playerForward * distance;

            // Map frontend names to actual RoR2 spawn card names
            var nameMapping = GetInteractableNameMapping();
            string actualName = nameMapping.ContainsKey(interactableName)
                ? nameMapping[interactableName]
                : interactableName;

            // Try to find the interactable spawn card using multiple approaches
            InteractableSpawnCard spawnCard = null;
            
            // First try InteractableSpawnCard
            var interactableCards = Resources.LoadAll<InteractableSpawnCard>("");
            logger.LogDebug($"Looking for interactable: {actualName} (original: {interactableName})");
            logger.LogDebug($"Found {interactableCards.Length} InteractableSpawnCard resources");
            
            // Also try SpawnCard (more generic)
            var allSpawnCards = Resources.LoadAll<SpawnCard>("");
            logger.LogDebug($"Found {allSpawnCards.Length} total SpawnCard resources");
            
            // Filter to only interactable spawn cards
            var allCards = allSpawnCards.OfType<InteractableSpawnCard>().ToArray();
            logger.LogDebug($"Found {allCards.Length} InteractableSpawnCard from SpawnCard resources");

            // Try multiple search strategies with common RoR2 patterns
            string[] searchTerms = { 
                actualName, 
                "isc" + actualName, 
                actualName + "Interactable", 
                "Interactable" + actualName,
                actualName.Replace("Shop", ""),
                actualName.Replace("Triple", "Multi"),
                actualName.Replace("Multi", "Triple"),
                actualName + "SpawnCard",
                "SpawnCard" + actualName,
                // Shrine-specific alternatives
                actualName.Replace("ShrineBoss", "ShrineHalcyonite"),
                actualName.Replace("ShrineBoss", "ShrineMountain"),
                actualName.Replace("ShrineHealing", "ShrineWoods"),
                actualName.Replace("ShrineRestack", "ShrineOrder"),
                // Remove prefixes/suffixes
                actualName.Replace("Shrine", ""),
                "Shrine" + actualName.Replace("Shrine", "")
            };
            
            foreach (var searchTerm in searchTerms)
            {
                // First try exact match
                foreach (var card in allCards)
                {
                    if (card.name.Equals(searchTerm, System.StringComparison.OrdinalIgnoreCase))
                    {
                        spawnCard = card;
                        logger.LogDebug($"Found exact match: {card.name} with search term: {searchTerm}");
                        break;
                    }
                }
                
                if (spawnCard != null) break;
            }
            
            // If still no match, try partial matching
            if (spawnCard == null)
            {
                foreach (var searchTerm in searchTerms)
                {
                    foreach (var card in allCards)
                    {
                        if (card.name.ToLower().Contains(searchTerm.ToLower()) ||
                            (card.prefab != null && card.prefab.name.ToLower().Contains(searchTerm.ToLower())))
                        {
                            spawnCard = card;
                            logger.LogDebug($"Found partial match: {card.name} with search term: {searchTerm}");
                            break;
                        }
                    }
                    
                    if (spawnCard != null) break;
                }
            }

            // Last resort: try to find any card that contains key parts of the name
            if (spawnCard == null)
            {
                string baseName = interactableName.ToLower();
                foreach (var card in allCards)
                {
                    string cardName = card.name.ToLower();
                    if (cardName.Contains(baseName) || baseName.Contains(cardName.Replace("isc", "")))
                    {
                        spawnCard = card;
                        logger.LogDebug($"Found fallback match: {card.name} for {interactableName}");
                        break;
                    }
                }
            }

            if (spawnCard == null)
            {
                logger.LogWarning($"Interactable spawn card not found: {actualName} (original: {interactableName})");
                logger.LogInfo("Available interactable spawn cards:");
                foreach (var card in allCards)
                {
                    logger.LogInfo($"  - {card.name} (prefab: {card.prefab?.name ?? "null"})");
                }

                return;
            }

            // Try to spawn using DirectorCore
            try
            {
                var spawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Direct,
                    position = spawnPosition,
                    minDistance = 0f,
                    maxDistance = 0f
                }, RoR2Application.rng);

                var result = DirectorCore.instance?.TrySpawnObject(spawnRequest);
                
               
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Exception while spawning {interactableName}: {ex.Message}");
                
                // Fallback: try direct prefab instantiation
                if (spawnCard.prefab != null)
                {
                    try
                    {
                        var instance = Object.Instantiate(spawnCard.prefab, spawnPosition, Quaternion.identity);
                        if (instance != null)
                        {
                            logger.LogInfo($"Successfully spawned {interactableName} using direct prefab instantiation");
                            NetworkServer.Spawn(instance);
                        }
                    }
                    catch (System.Exception prefabEx)
                    {
                        logger.LogError($"Failed to spawn {interactableName} via prefab instantiation: {prefabEx.Message}");
                    }
                }
            }

        }

        private System.Collections.Generic.Dictionary<string, string> GetInteractableNameMapping()
        {
            return new System.Collections.Generic.Dictionary<string, string>
            {
                // Chests - using common RoR2 spawn card patterns
                { "Chest1", "iscChest1" },
                { "Chest2", "iscChest2" },
                { "GoldChest", "iscGoldChest" },
                { "EquipmentBarrel", "iscEquipmentBarrel" },
                
                // Containers
                { "Barrel1", "iscBarrel1" },
                { "Pot", "iscPot" },
                { "Lockbox", "iscLockbox" },
                
                // Machines
                { "Scrapper", "iscScrapper" },
                { "Duplicator", "iscDuplicator" },
                { "DuplicatorLarge", "iscDuplicatorLarge" },
                { "DuplicatorMilitary", "iscDuplicatorMilitary" },
                { "DuplicatorWild", "iscDuplicatorWild" },
                { "FreeChestTerminal", "iscFreeChestTerminal" },
                
                // Shrines - try multiple common names
                { "ShrineBoss", "ShrineHalcyonite" }, // Mountain shrine might be called this
                { "ShrineChance", "ShrineChance" },
                { "ShrineCombat", "ShrineCombat" },
                { "ShrineHealing", "ShrineHealing" },
                { "ShrineRestack", "ShrineRestack" },
                { "ShrineCleanse", "ShrineCleanse" },
                { "ShrineGoldshoresAccess", "ShrineGoldshoresAccess" },
                
                // Shops - these might have different names
                { "MultiShop", "iscTripleShop" },
                { "TripleShop", "iscTripleShop" },
                { "TripleShopLarge", "iscTripleShopLarge" },
                { "TripleShopEquipment", "iscTripleShopEquipment" },
                { "CategoryChestDamage", "iscCategoryChestDamage" },
                { "CategoryChestHealing", "iscCategoryChestHealing" },
                { "CategoryChestUtility", "iscCategoryChestUtility" },
                
                // Lunar & Special
                { "LunarChest", "iscLunarChest" },
                { "ShrineBoss_Lunar", "iscShrineBoss_Lunar" },
                { "ShrineRestack_Lunar", "iscShrineRestack_Lunar" },
                { "NewtStatue", "iscNewtStatue" },
                { "VoidChest", "iscVoidChest" },
                { "VoidTriple", "iscVoidTriple" }
            };
        }
    }
}