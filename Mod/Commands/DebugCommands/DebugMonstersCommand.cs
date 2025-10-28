using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using System.Linq;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugMonstersCommand : BaseCommand
    {
        public override string CommandName => "debugmonsters";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("=== Available Monster Master Prefabs ===");
            
            var masterPrefabs = MasterCatalog.allMasters
                .Where(master => master != null && master.name != null)
                .OrderBy(master => master.name)
                .ToList();

            logger.LogInfo($"Found {masterPrefabs.Count} master prefabs:");
            
            foreach (var master in masterPrefabs)
            {
                var teamComponent = master.GetComponent<CharacterMaster>()?.teamIndex;
                var isMonster = teamComponent == TeamIndex.Monster || teamComponent == TeamIndex.Void || teamComponent == TeamIndex.Lunar;
                
                if (isMonster || master.name.Contains("Master"))
                {
                    logger.LogInfo($"  - {master.name} (Team: {teamComponent})");
                }
            }
            
            logger.LogInfo("=== End Monster List ===");
        }
    }
}