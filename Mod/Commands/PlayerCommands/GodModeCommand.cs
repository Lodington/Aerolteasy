using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class GodModeCommand : BaseCommand
    {
        public override string CommandName => "godmode";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var enabled = GetBoolFromCommand(command, "enabled");
            
            logger.LogInfo($"Setting god mode for player {playerId}: {enabled}");
            gameStateService.SetPlayerGodMode(playerId, enabled);
        }
    }
}