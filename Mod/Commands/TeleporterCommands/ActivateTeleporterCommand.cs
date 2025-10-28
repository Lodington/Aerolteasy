using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using RoR2DevTool.Utils;

namespace RoR2DevTool.Commands.TeleporterCommands
{
    public class ActivateTeleporterCommand : BaseCommand
    {
        public override string CommandName => "activateteleporter";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Activating teleporter");
            ActivateTeleporter(logger);
        }

        private void ActivateTeleporter(ManualLogSource logger)
        {
            var teleporterInteraction = TeleporterInteraction.instance;
            if (teleporterInteraction == null)
            {
                logger.LogWarning("No teleporter found in current stage");
                return;
            }

            // Start the teleporter charging process
            teleporterInteraction.SetFieldValue("activationState", TeleporterInteraction.ActivationState.Charging);
            
            // Trigger the activation
            if (teleporterInteraction.holdoutZoneController != null)
            {
                teleporterInteraction.holdoutZoneController.enabled = true;
            }

            logger.LogInfo("Teleporter activation started");
        }
    }
}