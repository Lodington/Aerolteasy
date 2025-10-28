using System;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.ItemCommands
{
    public class SpawnItemCommand : BaseCommand
    {
        public override string CommandName => "spawnitem";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var itemName = GetStringFromCommand(command, "itemName");
            var count = GetIntFromCommand(command, "count");
            
            if (string.IsNullOrEmpty(itemName))
            {
                logger.LogWarning("Item name is required for spawnitem command");
                return;
            }

            logger.LogDebug($"SpawnItem called: itemName={itemName}, count={count}, playerId={playerId}");
            SpawnItem(itemName, count, playerId, logger);
        }

        private void SpawnItem(string itemName, int count, int playerId, ManualLogSource logger)
        {
            var itemIndex = ItemCatalog.FindItemIndex(itemName);
            if (itemIndex == ItemIndex.None) 
            {
                logger.LogWarning($"Item not found: {itemName}");
                return;
            }
            
            logger.LogDebug($"Found item index: {itemIndex} for {itemName}");
            
            Inventory targetInventory = null;
            
            if (playerId == -1)
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                targetInventory = localUser?.cachedBody?.inventory;
                if (targetInventory == null)
                {
                    logger.LogWarning("Local player inventory not found");
                    return;
                }
            }
            else
            {
                var networkUser = GetNetworkUserById(playerId);
                targetInventory = networkUser?.master?.GetBody()?.inventory;
                if (targetInventory == null)
                {
                    logger.LogWarning($"Player {playerId} inventory not found");
                    return;
                }
            }
            
            // Handle negative counts (item removal)
            if (count < 0)
            {
                int currentCount = targetInventory.GetItemCount(itemIndex);
                int removeCount = Mathf.Min(currentCount, -count);
                if (removeCount > 0)
                {
                    targetInventory.RemoveItem(itemIndex, removeCount);
                    logger.LogInfo($"Removed {removeCount} {itemName} from player {playerId}");
                }
                else
                {
                    logger.LogWarning($"Cannot remove {-count} {itemName}, player only has {currentCount}");
                }
            }
            else if (count > 0)
            {
                targetInventory.GiveItem(itemIndex, count);
                logger.LogInfo($"Gave {count} {itemName} to player {playerId}");
            }
            
            // Force inventory synchronization
            try
            {
                CharacterMaster master = null;
                
                if (playerId == -1)
                {
                    var localUser = LocalUserManager.GetFirstLocalUser();
                    master = localUser?.cachedMaster;
                }
                else
                {
                    var networkUser = GetNetworkUserById(playerId);
                    master = networkUser?.master;
                }
                
                if (master != null)
                {
                    master.inventory.SetDirtyBit(1U);
                    logger.LogDebug("Forced inventory sync");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to sync inventory: {ex.Message}");
            }
        }
    }
}