using System;
using BepInEx.Logging;
using RoR2DevTool.Commands;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.ESPCommands
{
    public class ConfigureESPOverlayCommand : BaseCommand
    {
        public override string CommandName => "configureespoverlay";

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

                var maxDistance = GetFloatFromCommand(command, "maxDistance", -1f);
                if (maxDistance >= 0)
                {
                    espOverlayService.SetLabelDistance(maxDistance);
                }

                var labelScale = GetFloatFromCommand(command, "labelScale", -1f);
                if (labelScale >= 0)
                {
                    espOverlayService.SetLabelScale(labelScale);
                }

                if (command.Data.ContainsKey("showDistances"))
                {
                    bool showDistances = GetBoolFromCommand(command, "showDistances");
                    espOverlayService.ShowDistances = showDistances;
                    logger.LogInfo($"ESP distance display {(showDistances ? "enabled" : "disabled")}");
                }

                if (command.Data.ContainsKey("showHealth"))
                {
                    bool showHealth = GetBoolFromCommand(command, "showHealth");
                    espOverlayService.ShowHealth = showHealth;
                    logger.LogInfo($"ESP health display {(showHealth ? "enabled" : "disabled")}");
                }

                logger.LogInfo("ESP overlay configuration updated");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error configuring ESP overlay: {ex.Message}");
            }
        }
    }
}