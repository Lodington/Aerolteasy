using System;
using BepInEx.Logging;
using RoR2DevTool.Commands;
using RoR2DevTool.Commands.ESPCommands;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DisableESPOverlayCommand : BaseCommand
    {
        public override string CommandName => "disableespoverlay";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                var espOverlayService = ESPOverlayServiceHolder.Instance;
                if (espOverlayService == null)
                {
                    logger.LogError("ESP Overlay Service not available");
                    return;
                }

                espOverlayService.DisableAllOverlays();
                logger.LogInfo("All ESP overlays disabled and cleared");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error disabling ESP overlay: {ex.Message}");
            }
        }
    }
}