using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using System.Collections.Generic;
using System.Linq;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class ItemCatalogEntry
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Tier { get; set; }
        public string[] Tags { get; set; }
    }

    public class DebugItemCatalogCommand : BaseCommand
    {
        public override string CommandName => "debugitemcatalog";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Getting item catalog...");
            
            var items = new List<ItemCatalogEntry>();

            // Check if ItemCatalog is available and initialized
            if (ItemCatalog.availability.available)
            {
                logger.LogInfo("ItemCatalog is available, using ItemCatalog.allItems");
                
                // Use ItemCatalog.allItems when available for complete coverage
                foreach (var itemIndex in ItemCatalog.allItems)
                {
                    var itemDef = ItemCatalog.GetItemDef(itemIndex);
                    if (itemDef != null)
                    {
                        ProcessItemDef(itemDef, items, logger);
                    }
                }
            }
            else
            {
                logger.LogInfo("ItemCatalog not available, using itemCount iteration");
                
                // Fallback to traditional iteration
                for (ItemIndex itemIndex = 0; itemIndex < (ItemIndex)ItemCatalog.itemCount; itemIndex++)
                {
                    var itemDef = ItemCatalog.GetItemDef(itemIndex);
                    if (itemDef != null)
                    {
                        ProcessItemDef(itemDef, items, logger);
                    }
                }
            }

            // Also get equipment items if available
            if (EquipmentCatalog.availability.available)
            {
                logger.LogInfo("Adding equipment items from EquipmentCatalog");
                
                foreach (var equipmentIndex in EquipmentCatalog.allEquipment)
                {
                    var equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
                    if (equipmentDef != null)
                    {
                        ProcessEquipmentDef(equipmentDef, items, logger);
                    }
                }
            }
            else
            {
                logger.LogInfo("EquipmentCatalog not available, skipping equipment items");
            }

            // Sort items by category and then by name
            var sortedItems = items
                .OrderBy(item => GetCategoryOrder(item.Category))
                .ThenBy(item => item.Label)
                .Cast<object>()
                .ToList();

            logger.LogInfo($"Found {sortedItems.Count} total items (including equipment) in catalog");
            
            // Store the result in game state for the web UI to access
            gameStateService.SetItemCatalog(sortedItems);
        }

        private void ProcessItemDef(ItemDef itemDef, List<ItemCatalogEntry> items, ManualLogSource logger)
        {
            try
            {
                var category = GetItemCategory(itemDef.tier);
                var displayName = Language.GetString(itemDef.nameToken);
                var description = Language.GetString(itemDef.descriptionToken);
                
                // Fallback to internal name if display name is empty
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = itemDef.name;
                }
                
                // Include hidden items but mark them as such
                var finalCategory = itemDef.hidden ? $"{category} (Hidden)" : category;
                
                items.Add(new ItemCatalogEntry
                {
                    Value = itemDef.name,
                    Label = displayName,
                    Description = description,
                    Category = finalCategory,
                    Tier = itemDef.tier.ToString(),
                    Tags = itemDef.tags?.Select(tag => tag.ToString()).ToArray() ?? new string[0]
                });
                
                if (itemDef.hidden)
                {
                    logger.LogDebug($"Added hidden item: {displayName} ({itemDef.name})");
                }
            }
            catch (System.Exception ex)
            {
                logger.LogWarning($"Error processing item {itemDef?.name}: {ex.Message}");
            }
        }

        private void ProcessEquipmentDef(EquipmentDef equipmentDef, List<ItemCatalogEntry> items, ManualLogSource logger)
        {
            try
            {
                var displayName = Language.GetString(equipmentDef.nameToken);
                var description = Language.GetString(equipmentDef.descriptionToken);
                
                // Fallback to internal name if display name is empty
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = equipmentDef.name;
                }
                
                // Determine equipment category
                var category = equipmentDef.isLunar ? "Lunar Equipment (Blue)" : "Equipment Items (Orange)";
                
                items.Add(new ItemCatalogEntry
                {
                    Value = equipmentDef.name,
                    Label = displayName,
                    Description = description,
                    Category = category,
                    Tier = "Equipment",
                    Tags = new string[] { "Equipment" }
                });
                
                logger.LogDebug($"Added equipment: {displayName} ({equipmentDef.name})");
            }
            catch (System.Exception ex)
            {
                logger.LogWarning($"Error processing equipment {equipmentDef?.name}: {ex.Message}");
            }
        }

        private string GetItemCategory(ItemTier tier)
        {
            return tier switch
            {
                ItemTier.Tier1 => "Common Items (White)",
                ItemTier.Tier2 => "Uncommon Items (Green)", 
                ItemTier.Tier3 => "Legendary Items (Red)",
                ItemTier.Lunar => "Lunar Items (Blue)",
                ItemTier.Boss => "Boss Items (Yellow)",
                ItemTier.VoidTier1 => "Void Items (Purple)",
                ItemTier.VoidTier2 => "Void Items (Purple)",
                ItemTier.VoidTier3 => "Void Items (Purple)",
                ItemTier.VoidBoss => "Void Items (Purple)",
                ItemTier.NoTier => "Other Items",
                _ => "Other Items"
            };
        }

        private int GetCategoryOrder(string category)
        {
            // Handle hidden items by checking base category
            var baseCategory = category.Replace(" (Hidden)", "");
            
            return baseCategory switch
            {
                "Common Items (White)" => 1,
                "Uncommon Items (Green)" => 2,
                "Legendary Items (Red)" => 3,
                "Boss Items (Yellow)" => 4,
                "Lunar Items (Blue)" => 5,
                "Void Items (Purple)" => 6,
                "Equipment Items (Orange)" => 7,
                "Lunar Equipment (Blue)" => 8,
                "Other Items" => 9,
                _ => 10
            };
        }
    }
}