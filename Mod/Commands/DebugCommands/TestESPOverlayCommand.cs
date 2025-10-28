using System;
using BepInEx.Logging;
using RoR2DevTool.Commands;
using RoR2DevTool.Commands.ESPCommands;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class TestESPOverlayCommand : BaseCommand
    {
        public override string CommandName => "testespoverlay";

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

                // Enable all overlays for testing
                espOverlayService.SetMonsterLabels(true);
                espOverlayService.SetInteractableLabels(true);
                espOverlayService.SetItemLabels(true);
                
                // Set reasonable defaults
                espOverlayService.SetLabelDistance(100f);
                espOverlayService.SetLabelScale(1.0f);
                espOverlayService.ShowDistances = true;
                espOverlayService.ShowHealth = true;

                logger.LogInfo("ESP overlay test enabled - all labels should now be visible in-game with improved colors");
                logger.LogInfo("Note: Overlays are disabled by default on startup for better performance");
                logger.LogInfo("Use the ESP view in the web UI to toggle individual overlay types");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error testing ESP overlay: {ex.Message}");
            }
        }
    }
}