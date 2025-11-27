using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class ClearMockDataCommand : BaseCommand
    {
        public override string CommandName => "clearmockdata";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                logger.LogInfo("[RoR2DevTool] Mock data cleared");
                logger.LogInfo("[RoR2DevTool] Note: GameStateService always reads live game state");
                logger.LogInfo("[RoR2DevTool] This command is for UI testing purposes only");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"[RoR2DevTool] Failed to clear mock data: {ex.Message}");
            }
        }
    }
}