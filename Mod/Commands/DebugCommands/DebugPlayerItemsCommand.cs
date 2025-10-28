using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugPlayerItemsCommand : BaseCommand
    {
        public override string CommandName => "debugplayeritems";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            
            logger.LogInfo($"Debug player {playerId} items requested");
            DebugPlayerItems(playerId, logger);
        }

        private void DebugPlayerItems(int playerId, ManualLogSource logger)
        {
            logger.LogInfo($"=== Debug Player {playerId} Items ===");
            
            NetworkUser networkUser = null;
            CharacterMaster master = null;
            CharacterBody body = null;
            
            if (playerId == -1)
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                networkUser = localUser?.currentNetworkUser;
                master = localUser?.cachedMaster;
                body = localUser?.cachedBody;
            }
            else
            {
                networkUser = GetNetworkUserById(playerId);
                master = networkUser?.master;
                body = master?.GetBody();
            }
            
            logger.LogInfo($"NetworkUser: {networkUser != null}");
            logger.LogInfo($"Master: {master != null}");
            logger.LogInfo($"Body: {body != null}");
            
            // Try multiple inventory sources
            Inventory[] inventoriesToCheck = new Inventory[]
            {
                body?.inventory,
                master?.inventory,
                networkUser?.master?.inventory
            };
            
            for (int invIndex = 0; invIndex < inventoriesToCheck.Length; invIndex++)
            {
                var inventory = inventoriesToCheck[invIndex];
                if (inventory != null)
                {
                    logger.LogInfo($"Checking inventory source {invIndex}:");
                    int totalItems = 0;
                    
                    for (ItemIndex i = 0; i < (ItemIndex)ItemCatalog.itemCount; i++)
                    {
                        int count = inventory.GetItemCount(i);
                        if (count > 0)
                        {
                            totalItems += count;
                            var itemDef = ItemCatalog.GetItemDef(i);
                            if (itemDef != null)
                            {
                                logger.LogInfo($"  {count}x {itemDef.name}");
                            }
                        }
                    }
                    
                    logger.LogInfo($"  Total items in source {invIndex}: {totalItems}");
                    
                    if (totalItems > 0)
                    {
                        logger.LogInfo($"Found items in inventory source {invIndex}!");
                        break;
                    }
                }
                else
                {
                    logger.LogInfo($"Inventory source {invIndex} is null");
                }
            }
            
            logger.LogInfo("=== End Debug Player Items ===");
        }
    }
}