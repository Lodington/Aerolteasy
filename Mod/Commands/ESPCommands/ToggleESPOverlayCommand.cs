using System;
using BepInEx.Logging;
using RoR2DevTool.Commands;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.ESPCommands
{
    public class ToggleESPOverlayCommand : BaseCommand
    {
        public override string CommandName => "toggleespoverlay";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                // Get ESP overlay service from command processor
                var espOverlayService = GetESPOverlayService();
                if (espOverlayService == null)
                {
                    logger.LogError("ESP Overlay Service not available");
                    return;
                }

                string overlayType = GetStringFromCommand(command, "overlayType") ?? "all";
                bool enabled = GetBoolFromCommand(command, "enabled", true);

                switch (overlayType.ToLower())
                {
                    case "monsters":
                        espOverlayService.SetMonsterLabels(enabled);
                        break;
                    case "interactables":
                        espOverlayService.SetInteractableLabels(enabled);
                        break;
                    case "items":
                        espOverlayService.SetItemLabels(enabled);
                        break;
                    case "all":
                        espOverlayService.SetMonsterLabels(enabled);
                        espOverlayService.SetInteractableLabels(enabled);
                        espOverlayService.SetItemLabels(enabled);
                        break;
                    default:
                        logger.LogWarning($"Unknown overlay type: {overlayType}");
                        return;
                }

                logger.LogInfo($"ESP overlay {overlayType} {(enabled ? "enabled" : "disabled")}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error toggling ESP overlay: {ex.Message}");
            }
        }

        private ESPOverlayService GetESPOverlayService()
        {
            // This will be set by the command processor
            return ESPOverlayServiceHolder.Instance;
        }
    }

    // Helper class to hold ESP overlay service reference
    public static class ESPOverlayServiceHolder
    {
        public static ESPOverlayService Instance { get; set; }
    }
}