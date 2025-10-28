using System.Linq;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using RoR2DevTool.Utils;

namespace RoR2DevTool.Commands.TeleporterCommands
{
    public class SkipTeleporterEventCommand : BaseCommand
    {
        public override string CommandName => "skipteleporterevent";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Skipping teleporter event");
            SkipTeleporterEvent(logger);
        }

        private void SkipTeleporterEvent(ManualLogSource logger)
        {
            var teleporterInteraction = TeleporterInteraction.instance;
            if (teleporterInteraction == null)
            {
                logger.LogWarning("No teleporter found in current stage");
                return;
            }
            var mask = TeamMask.AllExcept(TeamIndex.Player);
            var mobs = CharacterMaster.instancesList.Where(x => mask.HasTeam(x.teamIndex)).ToArray();
            foreach (var characterMaster in mobs)
            {
                characterMaster.TrueKill();
            }
            // Skip to charged state
            teleporterInteraction.SetFieldValue("activationState", TeleporterInteraction.ActivationState.Charged);
            teleporterInteraction.SetFieldValue("chargeFraction", 1f);

            // Update holdout zone
            if (teleporterInteraction.holdoutZoneController != null)
            {
                teleporterInteraction.holdoutZoneController.charge = 1f;
                teleporterInteraction.holdoutZoneController.enabled = false;
            }

            logger.LogInfo("Teleporter event skipped");
        }
    }
}