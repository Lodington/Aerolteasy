using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class SetVerboseLoggingCommand : BaseCommand
    {
        public override string CommandName => "setverboselogging";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var enabled = GetBoolFromCommand(command, "enabled");
            
            logger.LogInfo($"Setting verbose logging to: {enabled}");
            gameStateService.SetVerboseLogging(enabled);
        }
    }
}