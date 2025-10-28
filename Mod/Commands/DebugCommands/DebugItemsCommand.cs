using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugItemsCommand : BaseCommand
    {
        public override string CommandName => "debugitems";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Debug all items requested");
            LogAllAvailableItems(logger);
        }

        private void LogAllAvailableItems(ManualLogSource logger)
        {
            logger.LogInfo("=== Available Items ===");
            for (ItemIndex i = 0; i < (ItemIndex)ItemCatalog.itemCount; i++)
            {
                var itemDef = ItemCatalog.GetItemDef(i);
                if (itemDef != null)
                {
                    logger.LogInfo($"Item: {itemDef.name} - {itemDef.nameToken}");
                }
            }
            logger.LogInfo("=== End Available Items ===");
        }
    }
}