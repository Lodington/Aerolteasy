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
                logger.LogInfo("[RoR2DevTool] Clearing mock game state data...");

                // Force a refresh of the actual game state
                gameStateService.RefreshGameState();

                logger.LogInfo("[RoR2DevTool] Mock data cleared, returned to actual game state");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"[RoR2DevTool] Failed to clear mock data: {ex.Message}");
            }
        }
    }
}