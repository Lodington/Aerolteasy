using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class RefreshStateCommand : BaseCommand
    {
        public override string CommandName => "refreshstate";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Game state refresh requested");
            // The game state service will handle the refresh automatically
        }
    }
}