using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using System.Linq;

namespace RoR2DevTool.Commands.MonsterCommands
{
    public class GiveMonsterItemCommand : BaseCommand
    {
        public override string CommandName => "givemonsteritem";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                string itemName = GetStringFromCommand(command, "itemName");
                int count = GetIntFromCommand(command, "count", 1);

                if (string.IsNullOrEmpty(itemName))
                {
                    logger.LogWarning("Item name is required for givemonsteritem command");
                    return;
                }

                // Get the item definition
                var itemDef = ItemCatalog.FindItemIndex(itemName);
                if (itemDef == ItemIndex.None)
                {
                    logger.LogWarning($"Item '{itemName}' not found");
                    return;
                }

                // Find all monster masters (enemies)
                var monsterMasters = CharacterMaster.readOnlyInstancesList
                    .Where(master => master != null && 
                           master.teamIndex == TeamIndex.Monster && 
                           master.inventory != null)
                    .ToList();

                if (monsterMasters.Count == 0)
                {
                    logger.LogInfo("No monsters found to give items to");
                    return;
                }

                // Give the item to all monsters
                int monstersAffected = 0;
                foreach (var master in monsterMasters)
                {
                    try
                    {
                        if (master.inventory != null)
                        {
                            master.inventory.GiveItem(itemDef, count);
                            monstersAffected++;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        logger.LogWarning($"Failed to give item to monster: {ex.Message}");
                    }
                }

                logger.LogInfo($"Gave {count}x {itemName} to {monstersAffected} monster(s)");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error in GiveMonsterItemCommand: {ex.Message}");
            }
        }
    }
}