using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using RoR2DevTool.Utils;

namespace RoR2DevTool.Commands.TeleporterCommands
{
    public class ChargeTeleporterCommand : BaseCommand
    {
        public override string CommandName => "chargeteleporter";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Charging teleporter to 100%");
            ChargeTeleporter(logger);
        }

        private void ChargeTeleporter(ManualLogSource logger)
        {
            var teleporterInteraction = TeleporterInteraction.instance;
            if (teleporterInteraction == null)
            {
                logger.LogWarning("No teleporter found in current stage");
                return;
            }

            // Set teleporter charge to 100%
            teleporterInteraction.SetFieldValue("activationState", TeleporterInteraction.ActivationState.Charged);
            teleporterInteraction.SetFieldValue("chargeFraction", 1f);
            
            // Update the teleporter state
            if (teleporterInteraction.holdoutZoneController != null)
            {
                teleporterInteraction.holdoutZoneController.charge = 1f;
            }

            logger.LogInfo("Teleporter charged to 100%");
        }
    }
}