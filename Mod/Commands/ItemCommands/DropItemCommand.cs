using System;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Networking.Messages;
using RoR2DevTool.Services;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2DevTool.Commands.ItemCommands
{
    public class DropItemCommand : BaseCommand
    {
        public override string CommandName => "dropitem";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var itemName = GetStringFromCommand(command, "itemName");
            var count = GetIntFromCommand(command, "count", 1);
            
            if (string.IsNullOrEmpty(itemName))
            {
                logger.LogWarning("Item name is required for dropitem command");
                return;
            }

            logger.LogDebug($"DropItem called: itemName={itemName}, count={count}, playerId={playerId}");
            DropItem(itemName, count, playerId, logger);
        }

        private void DropItem(string itemName, int count, int playerId, ManualLogSource logger)
        {
            var itemIndex = ItemCatalog.FindItemIndex(itemName);
            if (itemIndex == ItemIndex.None) 
            {
                logger.LogWarning($"Item not found: {itemName}");
                return;
            }
            
            logger.LogDebug($"Found item index: {itemIndex} for {itemName}");
            
            CharacterBody targetBody = null;
            
            if (playerId == -1)
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                targetBody = localUser?.cachedBody;
                if (targetBody == null)
                {
                    logger.LogWarning("Local player body not found");
                    return;
                }
            }
            else
            {
                var networkUser = GetNetworkUserById(playerId);
                targetBody = networkUser?.master?.GetBody();
                if (targetBody == null)
                {
                    logger.LogWarning($"Player {playerId} body not found");
                    return;
                }
            }
            
            // Get player position and forward direction
            Vector3 playerPosition = targetBody.transform.position;
            Vector3 playerForward = targetBody.transform.forward;

            // Use networked message to ensure all clients see the dropped items
            if (NetworkServer.active || NetworkClient.active)
            {
                var message = new DropItemMessage(itemIndex, count, playerPosition, playerForward);
                message.SendToEveryone();
                logger.LogInfo($"Sent networked drop item message: {count} {itemName} for player {playerId}");
            }
            else
            {
                // Fallback for offline/single player
                Vector3 dropPosition = playerPosition + (playerForward * 2f) + (Vector3.up * 1f);
                
                for (int i = 0; i < count; i++)
                {
                    Vector3 offset = new Vector3(
                        UnityEngine.Random.Range(-0.5f, 0.5f),
                        0,
                        UnityEngine.Random.Range(-0.5f, 0.5f)
                    );
                    
                    Vector3 finalPosition = dropPosition + offset;
                    
                    PickupDropletController.CreatePickupDroplet(
                        PickupCatalog.FindPickupIndex(itemIndex),
                        finalPosition,
                        Vector3.up * 10f
                    );
                }
                
                logger.LogInfo($"Dropped {count} {itemName} in front of player {playerId}");
            }
        }
    }
}
